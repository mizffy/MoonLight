using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StudioMaron
{
    public class LightGoboClip : PlayableAsset, ITimelineClipAsset
    {
        public LightGoboBehaviour behaviour = new LightGoboBehaviour();

        public ClipCaps clipCaps
        {
            get
            {
                // Light Gobo ClipはMixできない設定
                return ClipCaps.None;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<LightGoboBehaviour>.Create(graph);
        }
    }

}
