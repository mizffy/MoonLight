using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���C�g�̐F�E�������ꊇ�R���g���[������
/// </summary>
namespace StudioMaron
{
    [RequireComponent(typeof(LightFlashController), typeof(LightGoboController))]
    public class LightColorController : MonoBehaviour
    {
        // Inspector�ɕ\������p�����[�^��Light Color Parameter�Ɠ����ɂ��Ă���
        // �܂肱�̃R���g���[���̃C���X�y�N�^�Őݒ肷��̂͏����ݒ�p
        [Header("-----Target Light-----------------------------------------------------------------------------")]
        public List<Light> lightList = new List<Light>();

        [Header("-----Parameter--------------------------------------------------------------------------------")]
        public float baseIntensity = 1f;
        public LightColorParameter.GradientMode gradientMode;
        [GradientUsage(true)] public Gradient gradientColor;
        [Range(0, 1)] public float gradientValue = 0.5f;
        [Space(10)]

        [Header("-----Random Seed------------------------------------------------------------------------------")]
        public int randomSeed = 1234;
        [Space(10)]
        
        [Header("-----Option--------------------------------------------------------------------------")]
        // Timeline clip����Use Reference�AMix Reference���[�h���g���ꍇ�ɐݒ肷��
        public LightColorReference reference;

        [Header("-----Flashing Parameters-----------------------------------------------------------------")]
        public bool enableBeatFlash = true;
        public float flashCycleDuration = 4f; // 4秒（= 1小節）で1周期（フェードイン→アウト）
        private float flashTimer = 0f;
        private float flashIntensity = 1f;


        // �t���[�����[�g��max60�ɂ��ĕ��ׂ����炷�p�̕ϐ�
        float fps = 60;
        float deltaTime;
        int count;

        // Timeline����p�����[�^�̍X�V���������t���O
        bool parameterChangedByTimeline;

        // Light Color Parameter�̃��X�g
        // Timeline���炱�̃��X�g���X�V���AUpdate���Ɍv�Z����
        [HideInInspector] public List<LightColorParameter> previousList = new List<LightColorParameter>();

        // �ꏏ�ɃA�^�b�`����Ă���Light Flash Controller(�X�N���v�g���Ŏ����擾����)
        [HideInInspector] public LightFlashController flashController;

        // �v�Z�p�̕ϐ�
        float m_Intensity;
        float m_Value;
        Color m_Color;
        float m_CycleDuration;
        float m_CycleProgress;
        float r_Intensity;
        Color r_Color;
        float r_Weight;
        float x_Weight;

        // Start����Light Color Parameter������ď�����
        private void Start()
        {
            CreateParameter();
        }
        // ���Light Color Parameter�������Inspector�̍��ڂ𔽉f���ApreviousList�ɓ����
        void CreateParameter()
        {
            var param = new LightColorParameter();
            param.gradientMode = gradientMode;
            param.gradientColor = gradientColor;
            param.gradientValue = gradientValue;
            param.colorScrollBPM = 0;
            param.weight = 1;
            SetParameter(new List<LightColorParameter>() { param });
        }
        // Light Color Parameter�̃��X�g���쐬����
        void SetParameter(List<LightColorParameter> list)
        {
            previousList.Clear();
            previousList.AddRange(list);
        }
        // Inspector�̐��l��ҏW�����烉�C�g�ɔ��f������
        void OnValidate()
        {
            CreateParameter();
            UpdateColor();
        }
        // �t���[�����[�g60(1.6ms����)�ɏ������s��
        void Update()
        {
            // フレーム時間調整
            deltaTime += Time.deltaTime;
            if (deltaTime < 1 / fps) return;

            count = (int)(deltaTime * fps);
            deltaTime -= count / fps;

            if (!parameterChangedByTimeline)
            {
                foreach (var o in previousList)
                    o.clipTime += count / fps;
            }
            parameterChangedByTimeline = false;

            // 🔸 点滅ロジックの追加
            if (enableBeatFlash && flashCycleDuration > 0f)
            {
                flashTimer += count / fps;
                float normalizedTime = (flashTimer % flashCycleDuration) / flashCycleDuration;
                flashIntensity = Mathf.Sin(normalizedTime * Mathf.PI); // 0→1→0 に変化
            }
            else
            {
                flashIntensity = 1f;
            }

            UpdateColor();
        }

        // Color�̍X�V
        void UpdateColor()
        {
            // Use Reference���[�h�p��Weight�̏�����
            r_Weight = 0;
            // Mix Reference���[�h�p��Weight�̏�����
            x_Weight = 0;

            // ���ꂩ��v�Z����p�����[�^���X�g����Use Reference��Mix Reference���[�h������΁A�v�Z�p��Weight�ɒǉ�����
            for (int j = 0; j < previousList.Count; j++)
            {
                if (previousList[j].operationMode == LightColorParameter.OperationMode.UseReference)
                {
                    r_Weight += previousList[j].weight;
                }
                if (previousList[j].operationMode == LightColorParameter.OperationMode.MixReference)
                {
                    x_Weight += previousList[j].mixWeight * previousList[j].weight;
                }
            }
            // ���C�g���ɐF�Ƌ��x���v�Z���ēK�p
            for (int i = 0; i < lightList.Count; i++)
            {
                if (lightList[i] == null) continue;

                // �v�Z�p�̕ϐ��̏�����
                m_Intensity = 0f;
                m_Color = Color.black;

                // �p�����[�^�ƃ��C�g�ԍ����g���Čv�Z���Am_Intensity��m_Color�ɐ��l������
                CalcColor(previousList, i, ref m_Intensity, ref m_Color);

                // Reference�̌v�Z�p�̕ϐ��̏�����
                r_Intensity = 0f;
                r_Color = Color.black;

                // Use Reference��Mix Reference������΁AReference�̃p�����[�^���v�Z����
                if ((r_Weight != 0) || (x_Weight != 0))
                {
                    if (reference != null)
                    {
                        CalcColor(reference.GetReferenceColorParameter(), i, ref r_Intensity, ref r_Color);
                    }
                }

                // �W�����g���ă��C�g���x���Z�o���ēK�p
                lightList[i].intensity = baseIntensity * (m_Intensity * (1 - x_Weight) + r_Intensity * (x_Weight + r_Weight));

                // Light Flash Controller������ꍇ�͂��̌W�����|����
                if (flashController != null)
                {
                    lightList[i].intensity *= flashController.IntensityCoef(i, lightList.Count);
                }
                else
                {
                    if (TryGetComponent(out flashController))
                        lightList[i].intensity *= flashController.IntensityCoef(i, lightList.Count);
                        // Light Flash Controller が null でない場合は既に coef 乗算済みなのでその後に
                        lightList[i].intensity *= flashIntensity;
                }

                // �W�����g���ă��C�g�J���[���Z�o���ēK�p
                lightList[i].color = m_Color * (1 - x_Weight) + r_Color * (x_Weight + r_Weight);
            }
        }
        // �p�����[�^���g�����v�Z
        void CalcColor(List<LightColorParameter> paramList, int n, ref float Intensity, ref Color Color)
        {
            // �p�����[�^���ƂɌW�����Z�o���AIntensity��Color�ɑ������킹��
            for (int j = 0; j < paramList.Count; j++)
            {
                if (paramList[j] == null) continue;

                // Use Reference�̏ꍇ�͕ʂŌv�Z����̂ŏȗ�����
                if (paramList[j].operationMode == LightColorParameter.OperationMode.UseReference) continue;

                // BPM�ɉ������i�s�x�v�Z
                if (paramList[j].colorScrollBPM == 0)
                {
                    m_CycleDuration = 0;
                    m_CycleProgress = 0;
                    m_Value = paramList[j].gradientValue;
                }
                else
                {
                    m_CycleDuration = 60f * paramList[j].colorScrollBeat / paramList[j].colorScrollBPM;

                    if (m_CycleDuration == 0)
                        m_CycleProgress = 0;
                    else
                        m_CycleProgress = (paramList[j].clipTime % m_CycleDuration) / m_CycleDuration;

                    // m_Value��BPM�i�s�x��\��
                    m_Value = paramList[j].gradientValue + (paramList[j].reverseScroll ? 1 - m_CycleProgress : m_CycleProgress);
                }

                // Gradient Mode���ƂɐF�̊��蓖��
                switch (paramList[j].gradientMode)
                {
                    case LightColorParameter.GradientMode.Line:
                        m_Value = frac(m_Value + 0.5f + (n + 0.5f) / lightList.Count);
                        break;
                    case LightColorParameter.GradientMode.Sync:
                        break;
                    case LightColorParameter.GradientMode.Random:
                        m_Value = frac(m_Value + RandomFloat(paramList[j].overrideRamdomSeed ? paramList[j].randomSeed + n : randomSeed + n));
                        break;
                }

                // Intensity��Color�ɑ������킹��
                Intensity += paramList[j].intensityMultiplier * paramList[j].gradientColor.Evaluate(m_Value).a * paramList[j].weight;
                Color += paramList[j].gradientColor.Evaluate(m_Value) * paramList[j].weight;
            }
        }
        // Timeline clip���甭�����鏈��
        public void SetParameterFromTimeline(List<LightColorParameter> parameter)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                SetParameter(parameter);
            }
            else
            {
                SetParameter(parameter);
                UpdateColor();
            }
#else
                SetParameter(parameter);
#endif
            // Timeline�ɂ��X�V�t���O
            parameterChangedByTimeline = true;
        }

        // Timeline�̃v���r���[�I�����̏���
        public void ReapplyParameter()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                CreateParameter();
                UpdateColor();
            }
#endif
        }

        /// <summary>
        /// �^�����������
        /// </summary>
        Vector2 st = new Vector2();
        float RandomFloat(int seed)
        {
            st = new Vector2(seed, seed);
            return frac(Mathf.Sin(Vector2.Dot(st, new Vector2(12.9898f, 78.233f))) * 43758.5453f);
        }
        float frac(float f)
        {
            return f - Mathf.Floor(f);
        }

        // �q�I�u�W�F�N�g���烉�C�g���������Ď擾����
        // Inspector�̉E�ォ�甭���ł���
        [ContextMenu("Get light component from children")]
        public void GetLightListFromChildren()
        {
            var array = GetComponentsInChildren<Light>();
            if (array.Length == 0) return;

            var list = new List<Light>(array);

            lightList.Clear();
            lightList.AddRange(list);
        }
    }
}
