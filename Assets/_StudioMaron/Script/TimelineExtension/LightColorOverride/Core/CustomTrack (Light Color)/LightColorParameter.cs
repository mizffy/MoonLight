using UnityEngine;

/// <summary>
/// Timeline clip����R���g���[���֕ϐ���n�����A���̃N���X���g���ēn��
/// Light Color Controller��Light Emission Controller�ŋ��ʂ̃N���X���g��
/// </summary>
namespace StudioMaron
{
    public class LightColorParameter
    {
        public enum GradientMode
        {
            Line,
            Sync,
            Random
        }
        public enum OperationMode
        {
            Default,
            UseReference,
            MixReference
        }

        public float weight = 0f;
        [Min(0)] public float intensityMultiplier = 1f;
        public GradientMode gradientMode = GradientMode.Line;
        [GradientUsage(true)] public Gradient gradientColor = new Gradient();
        [Range(0f, 1f)] public float gradientValue = 0.5f;

        [Min(0)] public int colorScrollBPM = 0;
        [Min(1)] public int colorScrollBeat = 2;
        public bool reverseScroll = false;

        public float clipTime = 0f;

        public bool overrideRamdomSeed = false;
        public int randomSeed = 1234;

        public OperationMode operationMode = OperationMode.Default;
        [Range(0f, 1f)] public float mixWeight = 0.5f;
        
        public bool enableBeatFlash = true;
        public float flashCycleDuration = 4f;
    }
}
