using UnityEngine;
using UnityEngine.Playables;

namespace StudioMaron
{
    [System.Serializable]
    public class LightAngleBehaviour : PlayableBehaviour
    {
        public float weight = 0;
        public float clipTime = 0;

        public LightAngleParameter.OperationMode operationMode = LightAngleParameter.OperationMode.None;

        public bool isInstant = false;
        [Range(-90, 90)] public float allFixed_Tilt = 0;
        [Range(-180, 180)] public float allFixed_Pan = 0;
        [Range(0, 1)] public float allFixed_NoiseRange = 0f;
        [Range(0, 1)] public float allFixed_NoiseSpeed = 0f;

        [Range(-90, 90)] public float lineFixed_Tilt = 0;
        [Range(-180, 180)] public float lineFixed_Pan = 0;
        public AnimationCurve lineFixed_TiltCurve = AnimationCurve.Linear(timeStart: 0f, valueStart: 0f, timeEnd: 1f, valueEnd: 1f);
        public bool lineFixed_Asymmetry = false;
        [Range(0, 1)] public float lineFixed_NoiseRange = 0f;
        [Range(0, 1)] public float lineFixed_NoiseSpeed = 0f;

        [Range(-180, 180)] public float randomFixed_MaxPan = 180;
        [Range(-180, 180)] public float randomFixed_MinPan = -180;
        [Range(-90, 90)] public float randomFixed_MaxTilt = 90;
        [Range(-90, 90)] public float randomFixed_MinTilt = -90;
        public int randomFixed_RandomSeed = 1234;
        [Range(0, 1)] public float randomFixed_NoiseRange = 0f;
        [Range(0, 1)] public float randomFixed_NoiseSpeed = 0f;

        [Range(0, 90)] public float rotation_Tilt = 15;
        [Min(0.001f)] public float rotation_CycleTime = 3;
        [Range(0, 1)] public float rotation_Sync = 0;
        [Range(0, 1)] public float rotation_ReverseRate = 0;
        public int rotation_RandomSeed = 1234;

        [Range(-90, 90)] public float tiltWave_MaxTilt = 30;
        [Range(-90, 90)] public float tiltWave_MinTilt = -30;
        [Min(0.001f)] public float tiltWave_CycleTime = 5;
        [Range(0, 1)] public float tiltWave_Sync = 0;
        [Range(-180, 180)] public float tiltWave_Pan = 0;

        [Range(0.01f, 1)] public float searchLight_Range = 0.5f;
        [Range(0.01f, 1)] public float searchLight_Speed = 0.5f;
        public int searchLight_RandomSeed = 1234;

        public Vector3 lookAtTarget_LookAtWorldOffset = Vector3.zero;
        [Range(0, 1)] public float lookAtTarget_NoiseRange = 0f;
        [Range(0, 1)] public float lookAtTarget_NoiseSpeed = 0f;

        public int noiseRandomSeed = 1234;
    }
}