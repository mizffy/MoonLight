using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// ���C�g�̋����E�G�~�b�V�����̋����̌W����ێ�����
/// Light Color Controller�ALight Emission Controller�Ɉꏏ�ɂ��Ă��āA�����ɎQ�Ƃ����B
/// Timeline�Ő��䂷��p�Ȃ̂ŁA���䂳��Ȃ����͌W��=1��Ԃ��A�v�Z�����܂肵�Ă͂����Ȃ�
/// </summary>
namespace StudioMaron
{
    public class LightFlashController : MonoBehaviour, INotificationReceiver
    {
        // �R���g���[���̎Q�ƁB�����Q�Ƃ���̂�Use Reference���[�h�̎������B
        LightColorController light_controller;
        EmissionColorController emission_controller;

        // �t���[�����[�g��max60�ɂ��ĕ��ׂ����炷�p�̕ϐ�
        float fps = 60;
        float deltaTime;
        int count;

        // Timeline����p�����[�^�̍X�V���������t���O
        bool parameterChangedByTimeline;

        // Marker����̒ʒm�ƕϐ�
        bool isMarkerControl;
        float markerControlIntensity;
        float markerControlDuration;
        float markerControlTime;

        // Light Flash Parameter�̃��X�g
        // Timeline���炱�̃��X�g���X�V���AUpdate���Ɍv�Z����
        [HideInInspector] public List<LightFlashParameter> previousList = new List<LightFlashParameter>();

        // �v�Z�p�̕ϐ�
        float m_Intensity;
        float m_Value;
        float m_CycleDuration;
        float m_CycleProgress;

        // �Q�[���I�u�W�F�N�g�ɕt�^�������ALight Color Controller�܂���Light Emission Controller���擾
        void Reset() 
        {
            light_controller = GetComponent<LightColorController>();
            if(light_controller == null)
                emission_controller = GetComponent<EmissionColorController>();
        }

        // Start����Light Color Parameter������ď�����
        private void Start()
        {
            CreateParameter();
        }
        // ���Light Color Parameter�������previousList�ɓ����
        void CreateParameter()
        {
            var param = new LightFlashParameter();
            param.flashBpm = 0;
            param.weight = 1;
            SetParameter(new List<LightFlashParameter>() { param });
        }
        // Light Flash Parameter�̃��X�g���쐬����
        void SetParameter(List<LightFlashParameter> list)
        {
            previousList.Clear();
            previousList.AddRange(list);
        }

        // �t���[�����[�g60(1.6ms����)�ɏ������s��
        void Update()
        {
            // Marker�ɂ�鐧��̎��Ԍo�ߏ���
            if (0 < markerControlTime)
                markerControlTime -= Time.deltaTime;

            // Marker�ɂ�鐧�䂪�I�������t���O��false�ɂ���
            if (markerControlTime < 0)
            {
                markerControlTime = 0;
                isMarkerControl = false;
            }

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
        }

        // �O�����琔�l���Q�Ƃ���֐�
        // Light Color Controller�܂���Light Emission Controller���A�N�Z�X���ɂ���
        // isReference��Light Flash Controller����Use Reference���[�h�ŕʂ�Light Flash Controller���Q�Ƃ���ꍇ�ɖ������[�v��h�����߂̃t���O
        public float IntensityCoef(int n, int length, bool isReference = false)
        {
            // Marker�ɂ�鏈�����c���Ă���ꍇ�A�ʏ�̏�����Marker�̏�����Lerp�ŕ⊮����
            if (isMarkerControl)
                return Mathf.Lerp(TimelineClipIntensity(n, length, isReference), MarkerIntensity(n, length), markerControlTime / markerControlDuration);
            else
                return TimelineClipIntensity(n, length, isReference);
        }
        // Marker�ɂ��W���Z�o
        float MarkerIntensity(int n, int length)
        {
            return markerControlIntensity * Mathf.Pow(markerControlTime / markerControlDuration, 4);
        }
        // Timeline clip�ɂ��W���Z�o
        float TimelineClipIntensity(int n, int length, bool isReference)
        {
            // �v�Z�p�̕ϐ��̏�����
            m_Intensity = 0;

            // �p�����[�^�̃��X�g���Ȃ��ꍇ�͌W����1��Ԃ�
            if (previousList.Count == 0)
                m_Intensity = 1;

            // �p�����[�^���ɌW�����v�Z���đ������킹��
            for (int j = 0; j < previousList.Count; j++)
            {
                // ���g����Ȃ玟��
                if (previousList[j] == null) continue;

                // BPM��0���������̏���
                if (previousList[j].flashBpm == 0)
                {
                    m_CycleDuration = 0;
                    m_CycleProgress = 0;
                    m_Value = previousList[j].timeAdjust;
                }
                else
                {
                    // BPM�ɉ����Đi�s�x(0.0�`1.0)���v�Z
                    m_CycleDuration = 60f * previousList[j].flashEveryBeatNumber / previousList[j].flashBpm;

                    if (m_CycleDuration == 0)
                        m_CycleProgress = 0;
                    else
                        m_CycleProgress = (previousList[j].clipTime % m_CycleDuration) / m_CycleDuration;

                    // m_Value��BPM�ɉ������i�s�x��\��
                    m_Value = previousList[j].timeAdjust + m_CycleProgress;
                }

                switch (previousList[j].flashMode)
                {
                    // None���[�h��weight���W���Ƃ��ĕԂ��悤�ɂ���
                    case LightFlashParameter.FlashMode.None:
                        m_Intensity += previousList[j].weight;
                        break;

                    // All���[�h��Animation Curve�̃T���v�����O�����đS�Ẵ��C�g�œ����W�����o�� 
                    case LightFlashParameter.FlashMode.All:
                        m_Value = frac(m_Value);
                        m_Intensity += previousList[j].flashCurve.Evaluate(m_Value) * previousList[j].weight;
                        break;

                    // ���C�����[�h��Animation Curve�̃T���v�����O�����C�g���Ƃɏ��Ԃɂ��炷
                    case LightFlashParameter.FlashMode.Line:
                        m_Value = frac(m_Value + 0.5f + (n + 0.5f) / length);
                        m_Intensity += previousList[j].flashCurve.Evaluate(m_Value) * previousList[j].weight;
                        break;

                    // �X�C�b�`���[�h
                    case LightFlashParameter.FlashMode.Switch:

                        // ���C�g�������ԖڂƊ�Ԗڂŕ����A���݂ɓ_������
                        if (m_CycleProgress < 0.5f)
                        {
                            if (n % 2 == 0)
                            {
                                m_Intensity += 0;
                            }
                            else
                            {
                                m_Value *= 2;
                                m_Value = frac(m_Value);
                                m_Intensity += previousList[j].flashCurve.Evaluate(m_Value) * previousList[j].weight;
                            }
                        }
                        else
                        {
                            if (n % 2 == 0)
                            {
                                m_Value *= 2;
                                m_Value = frac(m_Value);
                                m_Intensity += previousList[j].flashCurve.Evaluate(m_Value) * previousList[j].weight;
                            }
                            else
                            {
                                m_Intensity += 0;
                            }
                        }
                        break;

                    // �O�������[�h
                    case LightFlashParameter.FlashMode.Split_3:

                        // ���C�g�������_����3�O���[�v�ɕ����A���Ԃ��ƂɌ��点��O���[�v��ς���
                        if (m_CycleProgress < 0.3333f)
                        {
                            if (RandomFloat(previousList[j].randomSeed + n) < 0.3333f)
                            {
                                m_Value *= 3;
                                m_Value = frac(m_Value);
                                m_Intensity += previousList[j].flashCurve.Evaluate(m_Value) * previousList[j].weight;
                            }
                            else
                            {
                                m_Intensity += 0;
                            }
                        }
                        else if (m_CycleProgress < 0.6666f)
                        {
                            if (0.3333f <= RandomFloat(previousList[j].randomSeed + n) && RandomFloat(previousList[j].randomSeed + n) < 0.6666f)
                            {
                                m_Value *= 3;
                                m_Value = frac(m_Value);
                                m_Intensity += previousList[j].flashCurve.Evaluate(m_Value) * previousList[j].weight;
                            }
                            else
                            {
                                m_Intensity += 0;
                            }
                        }
                        else
                        {
                            if (0.6666f <= RandomFloat(previousList[j].randomSeed + n) && RandomFloat(previousList[j].randomSeed + n) <= 1.0f)
                            {
                                m_Value *= 3;
                                m_Value = frac(m_Value);
                                m_Intensity += previousList[j].flashCurve.Evaluate(m_Value) * previousList[j].weight;
                            }
                            else
                            {
                                m_Intensity += 0;
                            }
                        }
                        break;

                    // �����_���X�C�b�`���[�h
                    case LightFlashParameter.FlashMode.RandomSwitch:

                        // Random Seed���g���ċ^�����������
                        m_Value = frac(m_Value + RandomFloat(previousList[j].randomSeed + n));

                        // m_Valur�����ȉ��̃��C�g�����点��Bm_Value�ɂ͎��Ԃ������Ă���̂ŁA���Ԍo�߂Ō��郉�C�g���ς��
                        if (m_Value < previousList[j].randomSwitchRate)
                        {
                            // Random Switch Rate�Ō��点�銄���𒲐�
                            m_Value *= 1 / previousList[j].randomSwitchRate;

                            // �����_�̂ݎ��o��
                            m_Value = frac(m_Value);

                            // m_Value���g����Animation Curve���琔�l���擾����
                            m_Intensity += previousList[j].flashCurve.Evaluate(m_Value) * previousList[j].weight;
                        }
                        else
                        {
                            m_Intensity += 0;
                        }
                        break;

                    // Reference���Q�Ƃ��郂�[�h
                    case LightFlashParameter.FlashMode.UseReference:
                        
                        // �R���g���[�����Ȃ��ꍇ�͎擾�����݂�
                        if(light_controller == null && emission_controller == null)
                        {
                            light_controller = GetComponent<LightColorController>();
                            if (light_controller == null)
                                emission_controller = GetComponent<EmissionColorController>();
                        }
                        // Light Color Controller��Reference���ݒ肳��Ă���ꍇ�A����Reference����W�����擾
                        if (light_controller != null)
                        {
                            if (light_controller.reference != null)
                                if (!isReference) // �������[�v�h�~
                                    m_Intensity += light_controller.reference.GetReferenceFlashCoef(n, length);
                        }
                        // Light Emission Controller��Reference���ݒ肳��Ă���ꍇ�A����Reference����W�����擾
                        else if (emission_controller != null)
                        {
                            if (emission_controller.reference != null)
                                if (!isReference) // �������[�v�h�~
                                    m_Intensity += emission_controller.reference.GetReferenceFlashCoef(n, length);
                        }
                        break;
                }
            }
            return m_Intensity;
        }

        // Timeline clip���甭�����鏈��
        public void SetParameterFromTimeline(List<LightFlashParameter> parameter)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                SetParameter(parameter);
            }
            else
            {
                SetParameter(parameter);
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
            }
#endif
        }

        // �^����������鏈��
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

        // Marker�ʂ̔���
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            // Light Flash Marker�̂ݔ�������
            if (!(notification is LightFlashMarker)) return;

            var marker = notification as LightFlashMarker;

            // �蓮��Timeline�̃V�[�N�o�[�𓮂��������ɔ������Ȃ��悤�ɂ��鏈��
            // �������A���������ɂ����Marker�̏������΂��\�������܂�邱�Ƃɒ���
            if (!(origin.GetTime() < marker.time + 0.1f)) return;

            // �t���b�V������
            LightFlash_Flash(marker.flashIntensity, marker.flashTime);
        }

        // Marker����̐��l����ϐ��Ɋi�[
        void LightFlash_Flash(float intensity, float time)
        {
            isMarkerControl = true;
            markerControlIntensity = intensity;
            markerControlDuration = time;
            markerControlTime = time;
        }
    }
}
