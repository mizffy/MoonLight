using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StudioMaron
{
    public class LightAngleClip : PlayableAsset, ITimelineClipAsset
    {
        public LightAngleBehaviour behaviour = new LightAngleBehaviour();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.Blending;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<LightAngleBehaviour>.Create(graph);
        }
    }

}
