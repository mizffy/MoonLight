using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StudioMaron
{
    [TrackColor(0.9f, 1f, 0f)]
    [TrackBindingType(typeof(LightColorController))]
    [TrackClipType(typeof(LightColorClip))]

    public class LightColorTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<LightColorMixerBehaviour>.Create(graph, inputCount);
            mixer.GetBehaviour().Clips = GetClips().ToArray();
            mixer.GetBehaviour().Director = go.GetComponent<PlayableDirector>();
            mixer.GetBehaviour().TrackBinding = go.GetComponent<PlayableDirector>().GetGenericBinding(this) as LightColorController;

            foreach (TimelineClip clip in m_Clips)
            {
                var playableAsset = clip.asset as LightColorClip;
                switch (playableAsset.behaviour.operationMode)
                {
                    case LightColorParameter.OperationMode.Default:
                        clip.displayName = "Intensity x" + playableAsset.behaviour.intensityMultiplier.ToString() + "   BPM: " + (playableAsset.behaviour.colorScrollBPM == 0 ? "OFF" : playableAsset.behaviour.colorScrollBPM.ToString());
                        break;
                    case LightColorParameter.OperationMode.UseReference:
                        clip.displayName = "Use Reference Color";
                        break;
                    case LightColorParameter.OperationMode.MixReference:
                        clip.displayName = "Mix Reference Color";
                        break;
                }
            }
            return mixer;
        }
    }
}
