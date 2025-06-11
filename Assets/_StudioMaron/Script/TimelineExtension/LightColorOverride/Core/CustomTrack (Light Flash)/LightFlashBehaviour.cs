using UnityEngine;
using UnityEngine.Playables;

namespace StudioMaron
{
    [System.Serializable]
    public class LightFlashBehaviour : PlayableBehaviour
    {
        [Header("-----Flash Setting-------------------------------------------------------------")]
        [Space(10)]
        public LightFlashParameter.FlashMode flashMode = LightFlashParameter.FlashMode.None;
        [Min(0)] public int flashBpm = 120;
        [Min(1)] public int flashEveryBeatNumber = 2;
        public AnimationCurve flashCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [Range(0f, 1f)] public float timeAdjust = 0.5f;

        [Header("-----Random Setting-------------------------------------------------------------")]
        [Space(10)]
        [Range(0.01f, 0.99f)] public float randomSwitchRate = 0.5f;
        public int randomSeed = 1234;
    }
}