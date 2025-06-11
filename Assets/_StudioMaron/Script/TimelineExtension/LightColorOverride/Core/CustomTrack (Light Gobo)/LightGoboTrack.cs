using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StudioMaron
{
    [TrackColor(1f, 1f, 1f)]
    [TrackBindingType(typeof(LightGoboController))]
    [TrackClipType(typeof(LightGoboClip))]

    public class LightGoboTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<LightGoboMixerBehaviour>.Create(graph, inputCount);
            mixer.GetBehaviour().Clips = GetClips().ToArray();
            mixer.GetBehaviour().Director = go.GetComponent<PlayableDirector>();
            mixer.GetBehaviour().TrackBinding = go.GetComponent<PlayableDirector>().GetGenericBinding(this) as LightGoboController;

            foreach (TimelineClip clip in m_Clips)
            {
                var playableAsset = clip.asset as LightGoboClip;
                clip.displayName = "Rotation: " + playableAsset.behaviour.rotationMode.ToString() + " / Gobo: " + playableAsset.behaviour.textureMode.ToString();
            }
            return mixer;
        }
    }
}
