using UnityEngine;

/// <summary>
/// Timeline clip����R���g���[���֕ϐ���n�����A���̃N���X���g���ēn��
/// </summary>
namespace StudioMaron
{
    public class LightFlashParameter
    {
        public enum FlashMode
        {
            None,
            All,
            Line,
            Switch,
            Split_3,
            RandomSwitch,
            UseReference
        }

        public float weight = 0f;
        public float clipTime = 0f;

        public FlashMode flashMode = FlashMode.None;

        [Min(0)] public int flashBpm = 120;
        [Min(1)] public int flashEveryBeatNumber = 2;
        public AnimationCurve flashCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Range(0.01f, 0.99f)] public float randomSwitchRate = 0.5f;

        [Range(0f, 1f)] public float timeAdjust = 0.5f;
        public int randomSeed = 1234;

    }
}
