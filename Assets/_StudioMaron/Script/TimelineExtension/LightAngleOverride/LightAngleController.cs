using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���[�r���O���C�g�̊p�x���ꊇ�R���g���[������
/// </summary>
namespace StudioMaron
{
    public class LightAngleController : MonoBehaviour
    {
        // Inspector�ɕ\������p�����[�^��Light Color Parameter�Ɠ����ɂ��Ă���
        // �܂肱�̃R���g���[���̃C���X�y�N�^�Őݒ肷��̂͏����ݒ�p
        // �G�f�B�^�g����Operation Mode�ɉ������p�����[�^�����\������
        public List<LightAngle> lightAngleList = new List<LightAngle>();
        public Transform lookAtTarget;

        [Header("-----Parameter--------------------------------------------------------------------------------")]
        [HideInInspector] public LightAngleParameter.OperationMode operationMode = LightAngleParameter.OperationMode.None;
        [Space(10)]

        [Header("-----All Fixed-----------------------------------------------------------------------------")]
        [HideInInspector] public bool isInstant = false;
        [HideInInspector] [Range(-90, 90)] public float allFixed_Tilt = 0;
        [HideInInspector] [Range(-180, 180)] public float allFixed_Pan = 0;
        [HideInInspector] [Range(0, 1)] public float allFixed_NoiseRange = 0f;
        [HideInInspector] [Range(0, 1)] public float allFixed_NoiseSpeed = 0f;
        [Space(10)]

        [Header("-----Line Fixed-----------------------------------------------------------------------------")]
        [HideInInspector] [Range(-90, 90)] public float lineFixed_Tilt = 0;
        [HideInInspector] public bool lineFixed_Asymmetry = false;
        [HideInInspector] public AnimationCurve lineFixed_TiltCurve = AnimationCurve.Linear(timeStart: 0f, valueStart: 0f, timeEnd: 1f, valueEnd: 1f);
        [Space(10)]
        [HideInInspector] [Range(-180, 180)] public float lineFixed_Pan = 0;
        [HideInInspector] [Range(0, 1)] public float lineFixed_NoiseRange = 0f;
        [HideInInspector] [Range(0, 1)] public float lineFixed_NoiseSpeed = 0f;
        [Space(10)]

        [Header("-----Random Fixed-----------------------------------------------------------------------------")]
        [HideInInspector] [Range(-90, 90)] public float randomFixed_MaxTilt = 90;
        [HideInInspector] [Range(-90, 90)] public float randomFixed_MinTilt = -90;
        [Space(10)]
        [HideInInspector] [Range(-180, 180)] public float randomFixed_MaxPan = 180;
        [HideInInspector] [Range(-180, 180)] public float randomFixed_MinPan = -180;
        [Space(10)]
        [HideInInspector] public int randomFixed_RandomSeed = 1234;
        [HideInInspector] [Range(0, 1)] public float randomFixed_NoiseRange = 0f;
        [HideInInspector] [Range(0, 1)] public float randomFixed_NoiseSpeed = 0f;
        [Space(10)]

        [Header("-----Rotaion-----------------------------------------------------------------------------")]
        [HideInInspector] [Range(0, 90)] public float rotation_Tilt = 15;
        [HideInInspector] [Min(0.001f)] public float rotation_CycleTime = 3;
        [HideInInspector] [Range(0, 1)] public float rotation_Sync = 0;
        [HideInInspector] [Range(0, 1)] public float rotation_ReverseRate = 0;
        [HideInInspector] public int rotation_RandomSeed = 1234;
        [Space(10)]

        [Header("-----Tilt Wave-----------------------------------------------------------------------------")]
        [HideInInspector] [Range(-90, 90)] public float tiltWave_MaxTilt = 30;
        [HideInInspector] [Range(-90, 90)] public float tiltWave_MinTilt = -30;
        [HideInInspector] [Min(0.001f)] public float tiltWave_CycleTime = 5;
        [HideInInspector] [Range(0, 1)] public float tiltWave_Sync = 0;
        [HideInInspector] [Range(-180, 180)] public float tiltWave_Pan = 0;
        [Space(10)]

        [Header("-----Search Light-----------------------------------------------------------------------------")]
        [HideInInspector] [Range(0.01f, 1)] public float searchLight_Range = 0.5f;
        [HideInInspector] [Range(0.01f, 1)] public float searchLight_Speed = 0.5f;
        [HideInInspector] public int searchLight_RandomSeed = 1234;
        [Space(10)]

        [Header("-----Look At Target-----------------------------------------------------------------------------")]
        [HideInInspector] public Vector3 lookAtTarget_LookAtWorldOffset = Vector3.zero;
        [HideInInspector] [Range(0, 1)] public float lookAtTarget_NoiseRange = 0f;
        [HideInInspector] [Range(0, 1)] public float lookAtTarget_NoiseSpeed = 0f;
        [Space(10)]

        [Header("-----Option-----------------------------------------------------------------------------")]
        // Timeline clip����Use Reference�AMix Reference���[�h���g���ꍇ�ɐݒ肷��
        [HideInInspector] public LightAngleReference reference;
        [Space(10)]
        [HideInInspector] public int noiseRandomSeed = 1234;


        // �t���[�����[�g��max60�ɂ��ĕ��ׂ����炷�p�̕ϐ�
        float fps = 60;
        float deltaTime;
        int count;

        // Timeline����p�����[�^�̍X�V���������t���O
        bool parameterChangedByTimeline;

        // Light Angle Parameter�̃��X�g
        // Timeline���炱�̃��X�g���X�V���AUpdate���Ɍv�Z����
        [HideInInspector] public List<LightAngleParameter> previousList = new List<LightAngleParameter>();

        // Light Angle Info�̃��X�g
        // ���̃��X�g���eLight Angle�ɑ���ALight Angle���p�x���v�Z����
        List<LightAngleInfo> infoList = new List<LightAngleInfo>();
        List<LightAngleInfo> info;

        // �v�Z�p�̕ϐ�
        float coef;
        float sign;
        float ref_weight;

        // Start����Light Color Parameter������ď�����
        private void Start()
        {
            CreateParameter();
        }
        // ���Light Color Parameter�������Inspector�̍��ڂ𔽉f���ApreviousList�ɓ����
        void CreateParameter()
        {
            var param = new LightAngleParameter();
            param.weight = 1;
            param.clipTime = 0;
            param.operationMode = operationMode;

            param.isInstant = isInstant;
            param.allFixed_Pan = allFixed_Pan;
            param.allFixed_Tilt = allFixed_Tilt;
            param.allFixed_NoiseRange = allFixed_NoiseRange;
            param.allFixed_NoiseSpeed = allFixed_NoiseSpeed;

            param.lineFixed_Pan = lineFixed_Pan;
            param.lineFixed_Tilt = lineFixed_Tilt;
            param.lineFixed_TiltCurve = lineFixed_TiltCurve;
            param.lineFixed_Asymmetry = lineFixed_Asymmetry;
            param.lineFixed_NoiseRange = lineFixed_NoiseRange;
            param.lineFixed_NoiseSpeed = lineFixed_NoiseSpeed;

            param.randomFixed_MaxPan = randomFixed_MaxPan;
            param.randomFixed_MinPan = randomFixed_MinPan;
            param.randomFixed_MaxTilt = randomFixed_MaxTilt;
            param.randomFixed_MinTilt = randomFixed_MinTilt;
            param.randomFixed_RandomSeed = randomFixed_RandomSeed;
            param.randomFixed_NoiseRange = randomFixed_NoiseRange;
            param.randomFixed_NoiseSpeed = randomFixed_NoiseSpeed;

            param.rotation_Tilt = rotation_Tilt;
            param.rotation_CycleTime = rotation_CycleTime;
            param.rotation_Sync = rotation_Sync;
            param.rotation_ReverseRate = rotation_ReverseRate;
            param.rotation_RandomSeed = rotation_RandomSeed;

            param.tiltWave_MaxTilt = tiltWave_MaxTilt;
            param.tiltWave_MinTilt = tiltWave_MinTilt;
            param.tiltWave_CycleTime = tiltWave_CycleTime;
            param.tiltWave_Sync = tiltWave_Sync;
            param.tiltWave_Pan = tiltWave_Pan;

            param.searchLight_Range = searchLight_Range;
            param.searchLight_Speed = searchLight_Speed;
            param.searchLight_RandomSeed = searchLight_RandomSeed;

            param.lookAtTarget_LookAtWorldOffset = lookAtTarget_LookAtWorldOffset;
            param.lookAtTarget_NoiseRange = lookAtTarget_NoiseRange;
            param.lookAtTarget_NoiseSpeed = lookAtTarget_NoiseSpeed;

            param.noiseRandomSeed = noiseRandomSeed;

            SetParameter(new List<LightAngleParameter>() { param });
        }
        // Light Angle Parameter�̃��X�g���쐬����
        void SetParameter(List<LightAngleParameter> list)
        {
            previousList.Clear();
            previousList.AddRange(list);
        }
        // Inspector�̐��l��ҏW�����烉�C�g�ɔ��f������
        public void OnValidate()
        {
            CreateParameter();
            UpdateAngle();
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

            // Angle�̍X�V
            UpdateAngle();
        }

        // Angle�̍X�V
        void UpdateAngle()
        {
            // Use Reference���[�h�p��Weight�̏�����
            ref_weight = 0;

            // ���ꂩ��v�Z����p�����[�^���X�g����Use Reference���[�h������΁A�v�Z�p��Weight�ɒǉ�����
            for (int j = 0; j < previousList.Count; j++)
            {
                if (previousList[j].operationMode == LightAngleParameter.OperationMode.UseReference)
                {
                    ref_weight += previousList[j].weight;
                }
            }

            // ���C�g���Ɋp�x���v�Z����infoList�ɓ����
            for (int i = 0; i < lightAngleList.Count; i++)
            {
                if (lightAngleList[i] == null) continue;

                // infoList�̃N���A
                infoList.Clear();

                // Use Reference��Mix Reference������΁AReference�̃p�����[�^���擾��infoList�ɓ����
                if (ref_weight != 0)
                {
                    if (reference != null)
                    {
                        infoList.AddRange(CalcAngle(reference.GetReferenceAngleParameter(), i, lightAngleList.Count, ref_weight));
                    }
                }

                // �p�����[�^���X�g����p�x���v�Z��infoList�ɓ����
                infoList.AddRange(CalcAngle(previousList, i, lightAngleList.Count));

                // �eLight Angle��infoList�𑗂�
                lightAngleList[i].SetPanTiltSlerp(infoList);
            }
        }
        // �p�x�̌v�Z
        List<LightAngleInfo> CalcAngle(List<LightAngleParameter> paramList, int n, int length, float weightCoef = 1)
        {
            info = new List<LightAngleInfo>();

            // �p�����[�^���Ƃ�info�����AinfoList�ɒǉ�����
            for (int k = 0; k < paramList.Count; k++)
            {
                info.Add(new LightAngleInfo());
                info[k].weight = paramList[k].weight * weightCoef;
                info[k].noiseRandomSeed = paramList[k].noiseRandomSeed + n;

                // Operation Mode���Ƃ̏���
                switch (paramList[k].operationMode)
                {
                    // None���[�h�ł�info�𖳌������ɂ���
                    case LightAngleParameter.OperationMode.None:
                        info[k].isInvalid = true;
                        break;
                    // All Fixed���[�h�͑S�Ẵ��C�g�������p�x
                    case LightAngleParameter.OperationMode.All_Fixed:
                        info[k].pan = paramList[k].allFixed_Pan;
                        info[k].tilt = paramList[k].allFixed_Tilt;

                        info[k].noiseRange = paramList[k].allFixed_NoiseRange;
                        info[k].noiseSpeed = paramList[k].allFixed_NoiseSpeed;
                        break;
                    // Line Fixed���[�h�ł͍��E�Ώ̂�V���Ɋp�x������
                    case LightAngleParameter.OperationMode.Line_Fixed:

                        if(length <= 2)
                        {
                            if(n == 0)
                                coef = -1;
                            else
                                coef = 1;
                            info[k].pan = paramList[k].lineFixed_Pan * coef;

                            if (paramList[k].lineFixed_Asymmetry)
                                coef = paramList[k].lineFixed_TiltCurve.Evaluate(n / (length - 1f));
                            else
                                coef = paramList[k].lineFixed_TiltCurve.Evaluate(1);
                            info[k].tilt = paramList[k].lineFixed_Tilt * coef;
                        }
                        else
                        {
                            // Pan
                            // �z��̑O��
                            if (n < (length - 1) / 2f)
                                coef = -1 * (1 - n / ((length - 1) / 2f));
                            // �z��̐^��
                            else if (n == (length - 1) / 2f)
                                coef = 0;
                            // �z��̌㔼
                            else
                                coef = (n - (length - 1) / 2f) / ((length - 1) / 2f);

                            info[k].pan = paramList[k].lineFixed_Pan * coef;

                            // Tilt
                            // ���E��Ώ̂ɂ���1��ɂ���ꍇ
                            if (paramList[k].lineFixed_Asymmetry)
                                coef = paramList[k].lineFixed_TiltCurve.Evaluate(n / (length - 1f));
                            // ���E�Ώ�
                            else
                            {
                                // �z�񂪋���
                                if (length % 2 == 0)
                                {
                                    // �z��̑O��
                                    if (n < length / 2f)
                                        coef = paramList[k].lineFixed_TiltCurve.Evaluate(1 - n / ((length / 2f) - 1));
                                    // �z��̌㔼
                                    else
                                        coef = paramList[k].lineFixed_TiltCurve.Evaluate((n - (length / 2f)) / ((length / 2f) - 1));
                                }
                                // �z�񂪊
                                else
                                {
                                    // �z��̑O��
                                    if (n < (length - 1) / 2f)
                                        coef = paramList[k].lineFixed_TiltCurve.Evaluate(1 - n / ((length - 1) / 2f));
                                    // �z��̐^��
                                    else if (n == (length - 1) / 2f)
                                        coef = paramList[k].lineFixed_TiltCurve.Evaluate(0);
                                    // �z��̌㔼
                                    else
                                        coef = paramList[k].lineFixed_TiltCurve.Evaluate((n - (length - 1) / 2f) / ((length - 1) / 2f));
                                }
                            }
                            info[k].tilt = paramList[k].lineFixed_Tilt * coef;
                        }

                        info[k].noiseRange = paramList[k].lineFixed_NoiseRange;
                        info[k].noiseSpeed = paramList[k].lineFixed_NoiseSpeed;
                        break;
                    // Random Fixed���[�h�ł̓V�[�h�l�ŗ���������ă����_���Ȋp�x�Ń��C�g��z�u����
                    case LightAngleParameter.OperationMode.Random_Fixed:
                        coef = RandomFloat(paramList[k].randomFixed_RandomSeed + n);
                        info[k].pan = Mathf.Lerp(paramList[k].randomFixed_MinPan, paramList[k].randomFixed_MaxPan, coef);
                        coef = RandomFloat(paramList[k].randomFixed_RandomSeed * 2 + n);
                        info[k].tilt = Mathf.Lerp(paramList[k].randomFixed_MinTilt, paramList[k].randomFixed_MaxTilt, coef);

                        info[k].noiseRange = paramList[k].randomFixed_NoiseRange;
                        info[k].noiseSpeed = paramList[k].randomFixed_NoiseSpeed;
                        break;
                    // Rotation���[�h�͈���Tilt�p�ŃO���O����
                    case LightAngleParameter.OperationMode.Rotation:
                        coef = RandomFloat(paramList[k].rotation_RandomSeed + n) * (1 - paramList[k].rotation_Sync);
                        sign = RandomFloat(paramList[k].rotation_RandomSeed + n) < paramList[k].rotation_ReverseRate ? -1 : 1;
                        info[k].pan = 360 * frac((Time.time / paramList[k].rotation_CycleTime) + coef) * sign;
                        info[k].tilt = paramList[k].rotation_Tilt;
                        break;
                    // Tilt Wave���[�h��Tilt�̏グ�������[�v
                    case LightAngleParameter.OperationMode.Tilt_Wave:
                        // Pan
                        // �z��̑O��
                        if (n < (length - 1) / 2f)
                        {
                            coef = -1 * (1 - n / ((length - 1) / 2f));
                        }
                        // �z��̐^��
                        else if (n == (length - 1) / 2f)
                        {
                            coef = 0;
                        }
                        // �z��̌㔼
                        else
                        {
                            coef = (n - (length - 1) / 2f) / ((length - 1) / 2f);
                        }
                        info[k].pan = paramList[k].tiltWave_Pan * coef;

                        // Tilt
                        coef = Mathf.Sin(2 * Mathf.PI * Time.time / paramList[k].tiltWave_CycleTime - (2 * Mathf.PI * n / length) * (1 - paramList[k].tiltWave_Sync));
                        info[k].tilt = (paramList[k].tiltWave_MaxTilt + paramList[k].tiltWave_MinTilt) * 0.5f + (paramList[k].tiltWave_MaxTilt - paramList[k].tiltWave_MinTilt) * 0.5f * coef;
                        break;
                    // Search Light���[�h�̓T�[�`���C�g���ۂ�����
                    case LightAngleParameter.OperationMode.Search_Light:
                        // Pan
                        coef = RandomFloat(paramList[k].searchLight_RandomSeed + n);
                        sign = RandomFloat(paramList[k].searchLight_RandomSeed + n) < 0.5f ? -1 : 1;
                        info[k].pan = 360 * frac((Time.time * paramList[k].searchLight_Speed * 0.25f) + coef) * sign;

                        // Tilt
                        coef = Mathf.Sin(2 * Mathf.PI * Time.time * paramList[k].searchLight_Speed * 0.63f - (2 * Mathf.PI * n / length));
                        info[k].tilt = paramList[k].searchLight_Range * 60 * coef;

                        break;
                    // Look At Target
                    case LightAngleParameter.OperationMode.LookAt_Target:
                        if (lookAtTarget != null)
                        {
                            info[k].isLookAtTarget = true;
                            info[k].targetPosition = lookAtTarget.position + paramList[k].lookAtTarget_LookAtWorldOffset;
                            info[k].noiseRange = paramList[k].lookAtTarget_NoiseRange;
                            info[k].noiseSpeed = paramList[k].lookAtTarget_NoiseSpeed;
                        }
                        else
                            info[k].isInvalid = true;
                        break;
                    // Use Reference��Reference�̋������Q�Ƃ���B�����̃V�[�h�l��Reference�Ɠ����ɂȂ邱�Ƃɒ���
                    case LightAngleParameter.OperationMode.UseReference:
                        info[k].isInvalid = true;
                        break;
                }
            }
            return info;
        }

        // Timeline clip���甭�����鏈��
        public void SetParameterFromTimeline(List<LightAngleParameter> parameter)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                SetParameter(parameter);
            }
            else
            {
                SetParameter(parameter);
                UpdateAngle();
            }
#else
                SetParameter(parameter);
#endif
        }
        // Timeline�̃v���r���[�I�����̏���
        public void ReapplyParameter()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                CreateParameter();
                UpdateAngle();
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

        // �q�I�u�W�F�N�g����Light Angle���������Ď擾����
        // Inspector�̉E�ォ�甭���ł���
        [ContextMenu("Get LightAngle component from children")]
        public void GetLightListFromChildren()
        {
            var array = GetComponentsInChildren<LightAngle>();
            if (array.Length == 0) return;

            var list = new List<LightAngle>(array);

            lightAngleList.Clear();
            lightAngleList.AddRange(list);
        }

    }
}
