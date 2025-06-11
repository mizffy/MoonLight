using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Light Color Controller��Light Emission Controller�ŋ��ʂ̃N���X���g���̂ŁA
/// Timeline��ŕ����I���E�ҏW���ł���
/// </summary>
namespace StudioMaron
{
    public class LightColorClip : PlayableAsset, ITimelineClipAsset
    {
        public LightColorBehaviour behaviour = new LightColorBehaviour();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.Blending;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<LightColorBehaviour>.Create(graph);
        }
    }

}
