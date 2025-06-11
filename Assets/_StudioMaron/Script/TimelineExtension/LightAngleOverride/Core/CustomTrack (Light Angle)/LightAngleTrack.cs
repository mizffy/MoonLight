using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StudioMaron
{
    [TrackColor(0f, 1f, 0.5f)]
    [TrackBindingType(typeof(LightAngleController))]
    [TrackClipType(typeof(LightAngleClip))]

    public class LightAngleTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<LightAngleMixerBehaviour>.Create(graph, inputCount);
            mixer.GetBehaviour().Clips = GetClips().ToArray();
            mixer.GetBehaviour().Director = go.GetComponent<PlayableDirector>();
            mixer.GetBehaviour().TrackBinding = go.GetComponent<PlayableDirector>().GetGenericBinding(this) as LightAngleController;

            foreach (TimelineClip clip in m_Clips)
            {
                var playableAsset = clip.asset as LightAngleClip;
                clip.displayName = playableAsset.behaviour.operationMode.ToString();
            }
            return mixer;
        }
    }
}
