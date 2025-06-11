using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StudioMaron
{
    public class LightFlashClip : PlayableAsset, ITimelineClipAsset
    {
        public LightFlashBehaviour behaviour = new LightFlashBehaviour();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.Blending;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<LightFlashBehaviour>.Create(graph);
        }
    }

}
