using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�~�b�V�����̐F�E�������ꊇ�R���g���[������
/// </summary>
namespace StudioMaron
{
    [RequireComponent(typeof(LightFlashController))]
    public class EmissionColorController : MonoBehaviour
    {
        // Inspector�ɕ\������p�����[�^��Light Color Parameter�Ɠ����ɂ��Ă���
        // �܂肱�̃R���g���[���̃C���X�y�N�^�Őݒ肷��̂͏����ݒ�p
        [Header("-----Target Material--------------------------------------------------------------------------")]
        public List<Material> materialList = new List<Material>();

        [Header("-----Parameter--------------------------------------------------------------------------------")]
        public string shaderKeyWord = "_EmissionColor";
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
        float b_Intensity;

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
            // �t���[�����[�g60�ɐ������鏈��
            deltaTime += Time.deltaTime;
            if (deltaTime < 1 / fps) return;

            count = (int)(deltaTime * fps);
            deltaTime -= count / fps;

            // Timeline�ɂ��ύX���Ȃ��ꍇ�A�O��g����LightColorParameter�̓������Ԃ�i�߂čė��p����
            if (!parameterChangedByTimeline)
            {
                foreach (var o in previousList)
                    o.clipTime += count / fps;
            }
            parameterChangedByTimeline = false;

            // Color�̍X�V
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
            for (int i = 0; i < materialList.Count; i++)
            {
                if (materialList[i] == null) continue;

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
                // Light Flash Controller������ꍇ�͂��̌W�����|����
                b_Intensity = 1;
                float flashIntensity = 1f;
                if (flashController != null)
                {
                    b_Intensity = flashController.IntensityCoef(i, materialList.Count);
                }
                else
                {
                    if (TryGetComponent(out flashController))
                    {
                        b_Intensity = flashController.IntensityCoef(i, materialList.Count);
                    }
                }

                // EmissionColorに反映（flashIntensityを新たに追加）
                materialList[i].SetColor(shaderKeyWord, baseIntensity * b_Intensity * flashIntensity * (m_Intensity * m_Color * (1 - x_Weight) + r_Intensity * r_Color * (x_Weight + r_Weight)));
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
                        m_Value = frac(m_Value + 0.5f + (n + 0.5f) / materialList.Count);
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
    }
}
