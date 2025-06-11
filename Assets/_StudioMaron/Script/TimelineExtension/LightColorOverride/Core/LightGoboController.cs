using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Light�R���|�[�l���g��Cookie��ݒ肷��
/// Light Color Controller�ƈꏏ�ɃA�^�b�`�����
/// Timeline�Ő��䂷��p�Ȃ̂ŁA���䂳��Ȃ����͉������Ȃ��Ƃ��������ɂ���
/// </summary>
namespace StudioMaron
{
    public class LightGoboController : MonoBehaviour
    {
        // ���C�g���Q�Ƃ���R���g���[��
        LightColorController controller;
        List<Light> lightList = new List<Light>();

        // Light Gobo Parameter�̃��X�g
        // Timeline���炱�̃��X�g���X�V���AUpdate���Ɍv�Z����
        [HideInInspector] public List<LightGoboParameter> previousList = new List<LightGoboParameter>();

        // Timeline����p�����[�^�̍X�V���������t���O
        bool parameterChangedByTimeline;

        // �v�Z�p�̕ϐ�
        float fps = 60;
        float deltaTime;
        int count;
        float coef;

        // �Q�[���I�u�W�F�N�g�ɕt�^�������ALight Color Controller���擾
        void Reset()
        {
            controller = GetComponent<LightColorController>();
        }


        // Start����Light Gobo Parameter������ď�����
        private void Start()
        {
            CreateParameter();
        }
        // ���Light Gobo Parameter�������previousList�ɓ����
        void CreateParameter()
        {
            var param = new LightGoboParameter();
            SetParameter(new List<LightGoboParameter>() { param });
        }
        // Light Gobo Parameter�̃��X�g���쐬����
        void SetParameter(List<LightGoboParameter> list)
        {
            previousList.Clear();
            previousList.AddRange(list);
        }

        // �t���[�����[�g60(1.6ms����)�ɏ������s��
        private void Update()
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

            // Cookie�̍X�V
            UpdateCookie();
        }
        // Cookie�̍X�V
        void UpdateCookie()
        {
            // null�`�F�b�N
            if (controller == null)
                controller = GetComponent<LightColorController>();

            if (controller == null) return;

            // ���C�g�̃��X�g���X�V
            lightList = controller.lightList;

            // ���C�g���Ƃɏ���
            for (int i = 0; i < lightList.Count; i++)
            {
                // ���C�g��null�Ȃ玟��
                if (lightList[i] == null) continue;

                // Light Gobo Parameter���Ƃɏ���������
                // �����ATimeline clip��Mix�ł��Ȃ��̂ő����Ă���Parameter�͍ő�1�B�����1��܂ł����������Ȃ�
                for(int j = 0; j < previousList.Count; j++)
                {
                    // Gobo�e�N�X�`������̏ꍇ��Cookie = null
                    if (previousList[j].cookieTexture.Count == 0)
                    {
                        lightList[i].cookie = null;
                    }
                    else
                    {
                        switch (previousList[j].textureMode)
                        {
                            // Order���[�h�ł̓e�N�X�`�������ԂɊ��蓖�Ă�
                            case LightGoboParameter.TextureMode.Order:
                                lightList[i].cookie = previousList[j].cookieTexture[i % previousList[j].cookieTexture.Count];
                                break;
                            // Random���[�h�ł̓����_���Ɋ��蓖�Ă�
                            case LightGoboParameter.TextureMode.Random:
                                lightList[i].cookie = previousList[j].cookieTexture[(int)(0.9999f * RandomFloat(previousList[j].randomSeed + i) * previousList[j].cookieTexture.Count)];
                                break;
                        }
                    }

                    switch (previousList[j].rotationMode)
                    {
                        case LightGoboParameter.RotationMode.None:
                            break;
                        // Constant���[�h�ł͑S�̂����X�s�[�h�ŉ�]
                        case LightGoboParameter.RotationMode.Constant:
                            coef = lightList[i].transform.localRotation.eulerAngles.z + previousList[j].rotationSpeed * 5;
                            lightList[i].transform.localRotation = Quaternion.Euler(0, 0, coef);
                            break;
                        // Odd Reverse���[�h�ł͊�Ԗڂ͋t��]
                        case LightGoboParameter.RotationMode.OddReverse:
                            if (i % 2 == 0)
                            {
                                coef = lightList[i].transform.localRotation.eulerAngles.z + previousList[j].rotationSpeed * 5;
                                lightList[i].transform.localRotation = Quaternion.Euler(0, 0, coef);
                            }
                            else
                            {
                                coef = lightList[i].transform.localRotation.eulerAngles.z - previousList[j].rotationSpeed * 5;
                                lightList[i].transform.localRotation = Quaternion.Euler(0, 0, coef);
                            }
                            break;
                        // Half Reverse���[�h�ł͌�딼���͋t��]�A�z��̐�����̏ꍇ�A���傤�ǐ^�񒆂̃��C�g�͉�]���Ȃ�
                        case LightGoboParameter.RotationMode.HalfReverse:
                            if (i < (lightList.Count - 1) / 2f)
                            {
                                coef = lightList[i].transform.localRotation.eulerAngles.z + previousList[j].rotationSpeed * 5;
                                lightList[i].transform.localRotation = Quaternion.Euler(0, 0, coef);
                            }
                            else if(i == (lightList.Count - 1) / 2f)
                                lightList[i].transform.localRotation = Quaternion.identity;
                            else
                            {
                                coef = lightList[i].transform.localRotation.eulerAngles.z - previousList[j].rotationSpeed * 5;
                                lightList[i].transform.localRotation = Quaternion.Euler(0, 0, coef);
                            }
                            break;
                    }
                }
            }
        }
        // Timeline clip���甭�����鏈��
        public void SetParameterFromTimeline(List<LightGoboParameter> parameter)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                SetParameter(parameter);
            }
            else
            {
                SetParameter(parameter);
                UpdateCookie();
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

        // �^�����������
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
