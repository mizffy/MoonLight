using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���[�r���O���C�g�̐���
/// </summary>
namespace StudioMaron
{
    public class LightAngle : MonoBehaviour
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        [Header("-----Tilt-------------------------------------------------------------------------------")]
        public Transform tilt;
        // Tilt�ŉ�]���������[�J����
        public Axis rotAxis_Tilt = Axis.X;
        // Tilt�̃��[�J����]�̏����l
        public Vector3 initRot_Tilt;
        [Space(10)]
        public bool debugTilt;
        [Range(-90, 90)] public float debugTiltAngle;

        [Header("-----Pan--------------------------------------------------------------------------------")]
        public Transform pan;
        // Pan�ŉ�]���������[�J����
        public Axis rotAxis_Pan = Axis.Y;
        // Pan�̃��[�J����]�̏����l
        public Vector3 initRot_Pan;
        [Space(10)]
        public bool debugPan;
        [Range(-180, 180)] public float debugPanAngle;

        [Header("-----Look At Target--------------------------------------------------------------------------------")]
        // Look at target�p��Tilt�̎����킹�B[X,Y,Z,-X,-Y,-Z]��6�p�^�[������̂Ő������W����1�I��
        [Range(0, 5)] public int lookAtAxisAdjust_Tilt;
        // Look at target�p��Pan�̎����킹�B[X,Y,Z]�~[Y,Z,-Y,-Z]��12�p�^�[������̂Ő������W����1�I��
        [Range(0, 11)] public int lookAtAxisAdjust_Pan;
        [Space(10)]
        // ��L�̎����킹������ۂɂ����B�^�[�Q�b�g�̕����𐳂��������W����I�ԁB
        public bool debugLookAt;
        public Transform debugLookAtTarget;

        // �v�Z�p�̕ϐ�
        Quaternion init;
        Quaternion end;
        float noisePan;
        float noiseTilt;
        Vector3 noisePosition;
        float weight;
        bool invalid;
        List<LightAngleInfo> infoList = new List<LightAngleInfo>();
        Quaternion previousWorldRot_Pan = Quaternion.identity;
        Quaternion previousWorldRot_Tilt = Quaternion.identity;
        Quaternion previousLocalRot_Pan = Quaternion.identity;
        Quaternion previousLocalRot_Tilt = Quaternion.identity;

        // Inspector�̐��l�̕ύX���������ꍇ�A�������f������
        private void OnValidate()
        {
            infoList.Clear();

            var info = new LightAngleInfo();

            // �f�o�b�O���[�h�̏ꍇ�AInspector�̊p�x�𔽉f������
            info.pan = debugPanAngle;
            info.tilt = debugTiltAngle;
            info.weight = 1;
            if (debugLookAt && debugLookAtTarget)
            {
                info.isLookAtTarget = true;
                info.targetPosition = debugLookAtTarget.position;
            }
            infoList.Add(info);

            // �f�o�b�O���[�h��Pan��Tilt��K�p����
            SetPanTiltSlerp(infoList, true);
        }
        // �f�o�b�O���[�h�̏ꍇ�̂�Update�Ōv�Z����B�ʏ��Light Angle Controller����̐���҂�
        private void Update()
        {
            if (debugTilt || debugPan || debugLookAt)
                OnValidate();
        }
        // Light Angle Controller���甭������BisDebugMode���Ƒ������f���邪�A�ʏ��Slerp(previous, current, 0.1f)�Ń��[�p�X�t�B���^�������Ă���
        public void SetPanTiltSlerp(List<LightAngleInfo> infoList, bool isDebugMode = false)
        {
            // infoList�̒��g���S�Ė����Ȃ珈���I��
            invalid = true;
            for (int i = 0; i < infoList.Count; i++)
            {
                invalid = invalid && infoList[i].isInvalid;
            }
            if (invalid) return;

            weight = 0;

            if (isDebugMode)
            {
                // �f�o�b�O���[�h�̏ꍇ��Tilt��Pan��Е����������̂ŉ�]�̏��������s��Ȃ��ꍇ������
                if (debugPan)
                    pan.localRotation = Quaternion.identity;
                if(debugTilt)
                    tilt.localRotation = Quaternion.identity;
                if (debugLookAt)
                {
                    pan.localRotation = Quaternion.identity;
                    tilt.localRotation = Quaternion.identity;
                }
            }
            else
            {
                // ��]�̏�����������B���̏�������Look At Target�̎��ɐ������v�Z����̂ɕK�v
                pan.localRotation = Quaternion.identity;
                tilt.localRotation = Quaternion.identity;
            }

            for (int i = 0; i < infoList.Count; i++)
            {
                if (infoList[i].isInvalid) continue;
                if (infoList[i].weight == 0) continue;
                // Look At Target�̎��̏����̓O���[�o����]�Ōv�Z����K�v������B
                if (infoList[i].isLookAtTarget)
                {
                    if (pan == null) continue;
                    if (tilt == null) continue;

                    weight += infoList[i].weight;
                    noisePan = infoList[i].noiseRange * (-2 + 4 * perlinNoise(new Vector2(RandomFloat(infoList[i].noiseRandomSeed), Time.time * infoList[i].noiseSpeed * 3 - infoList[i].noiseRandomSeed)));
                    noiseTilt = infoList[i].noiseRange * (-2 + 4 * perlinNoise(new Vector2(RandomFloat(2 * infoList[i].noiseRandomSeed), Time.time * infoList[i].noiseSpeed * 3 + infoList[i].noiseRandomSeed)));
                    noisePosition = new Vector3(noisePan, 0, noiseTilt);

                    // Pan
                    init = Quaternion.Euler(initRot_Pan);

                    var pan_up = AxisAdjustPan(Dir.UP, lookAtAxisAdjust_Pan, pan);
                    var pan_forward = AxisAdjustPan(Dir.FORWARD, lookAtAxisAdjust_Pan, pan);

                    var panToTarget = infoList[i].targetPosition - pan.position - noisePosition;
                    var projection_p = panToTarget - Vector3.Dot(panToTarget, pan_up) * pan_up;
                    var angle_p = Vector3.Angle(pan_forward, projection_p);
                    if (angle_p < 179)
                        infoList[i].quaternion = Quaternion.FromToRotation(pan_forward, projection_p) * pan.rotation * init;
                    else
                        infoList[i].quaternion = Quaternion.AngleAxis(angle_p, pan_up) * pan.rotation * init; // since the object flips around 180 degrees.

                    end = Quaternion.Slerp(pan.rotation, infoList[i].quaternion, infoList[i].weight / weight);
                    pan.rotation = infoList[i].quaternion;
                    if (weight == 1)
                    {
                        previousLocalRot_Pan = pan.localRotation;
                        previousWorldRot_Pan = pan.rotation;
                    }

                    // Tilt
                    init = Quaternion.Euler(initRot_Tilt);

                    var tilt_up = AxisAdjustTilt(Dir.UP, lookAtAxisAdjust_Tilt, tilt); // Tilt��UP�����������Ă���΂悢

                    var tiltToTarget = infoList[i].targetPosition - tilt.position - noisePosition;
                    var projection_t = tiltToTarget; // Pan�Ŋ��Ɍ����̒��߂����Ă���̂Łu�V���~�b�g�̐��K���s������Right�x�N�g�������̍폜�v�̑���͕s�v
                    infoList[i].quaternion = Quaternion.FromToRotation(tilt_up, projection_t) * tilt.rotation * init;

                    end = Quaternion.Slerp(tilt.rotation, infoList[i].quaternion, infoList[i].weight / weight);
                    tilt.rotation = infoList[i].quaternion;
                    if (weight == 1)
                    {
                        previousLocalRot_Tilt = tilt.localRotation;
                        previousWorldRot_Tilt = tilt.rotation;
                    }
                }
                // Look At Target�ȊO�̏����̓��[�J����]�ōs��
                else
                {
                    weight += infoList[i].weight;
                    if (pan != null)
                    {
                        init = Quaternion.Euler(initRot_Pan);
                        noisePan = infoList[i].noiseRange * (-180 + 360 * perlinNoise(new Vector2(RandomFloat(infoList[i].noiseRandomSeed), Time.time * infoList[i].noiseSpeed * 3 - infoList[i].noiseRandomSeed)));
                        infoList[i].quaternion = Q_Axis(rotAxis_Pan, infoList[i].pan + noisePan) * init;
                        end = Quaternion.Slerp(pan.localRotation, infoList[i].quaternion, infoList[i].weight / weight);
                        if (isDebugMode)
                        {
                            if(debugPan)
                                pan.localRotation = end;
                        }
                        else
                        {
                            pan.localRotation = infoList[i].quaternion;
                        }
                        if (weight == 1)
                        {
                            previousLocalRot_Pan = pan.localRotation;
                            previousWorldRot_Pan = pan.rotation;
                        }
                    }

                    if (tilt != null)
                    {
                        init = Quaternion.Euler(initRot_Tilt);
                        noiseTilt = infoList[i].noiseRange * (-45 + 90 * perlinNoise(new Vector2(RandomFloat(2 * infoList[i].noiseRandomSeed), Time.time * infoList[i].noiseSpeed * 3 + infoList[i].noiseRandomSeed)));
                        infoList[i].quaternion = Q_Axis(rotAxis_Tilt, infoList[i].tilt + noiseTilt) * init;
                        end = Quaternion.Slerp(tilt.localRotation, infoList[i].quaternion, infoList[i].weight / weight);
                        if (isDebugMode)
                        {
                            if(debugTilt)
                                tilt.localRotation = end;
                        }
                        else
                        {
                            tilt.localRotation = infoList[i].quaternion;
                        }
                        if (weight == 1)
                        {
                            previousLocalRot_Tilt = tilt.localRotation;
                            previousWorldRot_Tilt = tilt.rotation;
                        }
                    }
                }
            }
        }
        // Inspector�őI�񂾎��ƁA�p�x�ɍ��킹��Quaternion��Ԃ�
        Quaternion Q_Axis(Axis axis, float degree)
        {
            if (degree == 0)
                return Quaternion.identity;
            switch (axis)
            {
                default:
                    return Quaternion.Euler(Vector3.right * degree);
                case Axis.Y:
                    return Quaternion.Euler(Vector3.up * degree);
                case Axis.Z:
                    return Quaternion.Euler(Vector3.forward * degree);
            }
        }
        public enum Dir
        {
            UP,
            FORWARD
        }
        // Look At Target�p�̎����킹��Tilt�W��
        Vector3 AxisAdjustTilt(Dir dir, int n, Transform t)
        {
            // Tilt: UP��6���
            switch (n)
            {
                default:
                    switch (dir) { default: return t.up;}
                case 1:
                    switch (dir) { default: return -t.up;}
                case 2:
                    switch (dir) { default: return t.right;}
                case 3:
                    switch (dir) { default: return -t.right;}
                case 4:
                    switch (dir) { default: return t.forward;}
                case 5:
                    switch (dir) { default: return -t.forward;}
            }
        }
        // Look At Target�p�̎����킹��Pan�W��
        Vector3 AxisAdjustPan(Dir dir, int n, Transform t)
        {
            // Pan: UP��3��ށAFORWARD��4���
            switch (n)
            {
                default:
                    switch (dir) { default: return t.up; case Dir.FORWARD: return t.forward;}
                case 1:
                    switch (dir) { default: return t.up; case Dir.FORWARD: return t.right;}
                case 2:
                    switch (dir) { default: return t.up; case Dir.FORWARD: return -t.forward;}
                case 3:
                    switch (dir) { default: return t.up; case Dir.FORWARD: return -t.right;}

                case 4:
                    switch (dir) { default: return t.right; case Dir.FORWARD: return t.up;}
                case 5:
                    switch (dir) { default: return t.right; case Dir.FORWARD: return t.forward;}
                case 6:
                    switch (dir) { default: return t.right; case Dir.FORWARD: return -t.up;}
                case 7:
                    switch (dir) { default: return t.right; case Dir.FORWARD: return -t.forward;}

                case 8:
                    switch (dir) { default: return t.forward; case Dir.FORWARD: return t.right;}
                case 9:
                    switch (dir) { default: return t.forward; case Dir.FORWARD: return t.up;}
                case 10:
                    switch (dir) { default: return t.forward; case Dir.FORWARD: return -t.right;}
                case 11:
                    switch (dir) { default: return t.forward; case Dir.FORWARD: return -t.up;}
            }
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
        Vector2 random2(Vector2 vec)
        {
            vec = new Vector2(Vector2.Dot(vec, new Vector2(127.1f, 311.7f)), Vector2.Dot(vec, new Vector2(269.5f, 183.3f)));
            var p = new Vector2(Mathf.Sin(vec.x), Mathf.Sin(vec.y)) * 43758.5453123f;
            var frac = p - new Vector2(Mathf.Floor(p.x), Mathf.Floor(p.y));
            return new Vector2(-1.0f, -1.0f) + 2.0f * frac;
        }
        float perlinNoise(Vector2 st)
        {
            Vector2 p = new Vector2(Mathf.Floor(st.x), Mathf.Floor(st.y));
            Vector2 f = st - p;
            Vector2 u = f * f * (new Vector2(3.0f, 3.0f) - 2.0f * f);

            float v00 = random2(p + new Vector2(0, 0)).x;
            float v10 = random2(p + new Vector2(1, 0)).x;
            float v01 = random2(p + new Vector2(0, 1)).x;
            float v11 = random2(p + new Vector2(1, 1)).x;

            return Mathf.Lerp(Mathf.Lerp(Vector2.Dot(new Vector2(v00, v00), f - new Vector2(0, 0)), Vector2.Dot(new Vector2(v10, v10), f - new Vector2(1, 0)), u.x),
                Mathf.Lerp(Vector2.Dot(new Vector2(v01, v01), f - new Vector2(0, 1)), Vector2.Dot(new Vector2(v11, v11), f - new Vector2(1, 1)), u.x), u.y) + 0.5f;
        }

    }
}
