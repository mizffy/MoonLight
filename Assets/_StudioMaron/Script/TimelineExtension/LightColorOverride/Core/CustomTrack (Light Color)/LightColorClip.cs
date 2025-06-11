using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Light Color ControllerとLight Emission Controllerで共通のクラスを使うので、
/// Timeline上で複数選択・編集ができる
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
