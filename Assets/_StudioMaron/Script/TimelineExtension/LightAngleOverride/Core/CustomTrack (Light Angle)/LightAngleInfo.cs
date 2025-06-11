using UnityEngine;

/// <summary>
/// Light Angle Controller����eLight Angle�֕ϐ���n�����A���̃N���X���g���ēn��
/// </summary>
namespace StudioMaron
{
    public class LightAngleInfo
    {
        public float weight = 0;
        public float pan = 0;
        public float tilt = 0;
        public bool isLookAtTarget = false;
        public Vector3 targetPosition = Vector3.zero;
        public bool isInvalid = false;
        public Quaternion quaternion = Quaternion.identity;
        public float noiseRange = 0f;
        public float noiseSpeed = 0f;
        public int noiseRandomSeed = 1234;
    }
}
