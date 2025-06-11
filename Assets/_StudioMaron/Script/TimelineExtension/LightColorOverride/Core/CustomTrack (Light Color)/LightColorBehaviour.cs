using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Light Color Controller��Light Emission Controller�ŋ��ʂ̃N���X���g��
/// </summary>
namespace StudioMaron
{
    [System.Serializable]
    public class LightColorBehaviour : PlayableBehaviour
    {
        [Header("-----Color Setting-------------------------------------------------------------")]
        [Space(10)]
        [Min(0)] public float intensityMultiplier = 1f;
        public LightColorParameter.GradientMode gradientMode = LightColorParameter.GradientMode.Line;
        [GradientUsage(true)] public Gradient gradientColor = new Gradient();
        [Range(0f, 1f)] public float gradientValue = 0.5f;

        [Header("-----Scroll Setting-------------------------------------------------------------")]
        [Space(10)]
        [Min(0)] public int colorScrollBPM = 0;
        [Min(1)] public int colorScrollBeat = 2;
        public bool reverseScroll = false;

        [Header("-----Random Seed-------------------------------------------------------------")]
        [Space(10)]
        public bool overrideRamdomSeed = false;
        public int randomSeed = 1234;

        [Header("-----Option Setting-------------------------------------------------------------")]
        [Space(10)]
        public LightColorParameter.OperationMode operationMode = LightColorParameter.OperationMode.Default;
        [Space(10)]
        [Range(0f, 1f)] public float mixWeight = 0.5f;
        
        [Header("-----Flashing Parameters-----------------------------------------------------------------")]
        public bool enableBeatFlash = true;
        public float flashCycleDuration = 4f; // 4秒（= 1小節）で1周期（フェードイン→アウト）
        
    }
}