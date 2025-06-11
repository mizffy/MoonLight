using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace StudioMaron
{
    [TrackColor(0f, 0f, 0f)]
    [TrackBindingType(typeof(LightFlashController))]
    [TrackClipType(typeof(LightFlashClip))]

    public class LightFlashTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<LightFlashMixerBehaviour>.Create(graph, inputCount);
            mixer.GetBehaviour().Clips = GetClips().ToArray();
            mixer.GetBehaviour().Director = go.GetComponent<PlayableDirector>();
            mixer.GetBehaviour().TrackBinding = go.GetComponent<PlayableDirector>().GetGenericBinding(this) as LightFlashController;

            foreach (TimelineClip clip in m_Clips)
            {
                var playableAsset = clip.asset as LightFlashClip;
                switch (playableAsset.behaviour.flashMode)
                {
                    case LightFlashParameter.FlashMode.None:
                        clip.displayName = "None";
                        break;
                    case LightFlashParameter.FlashMode.All:
                        clip.displayName = "Flash: ALL" + "   BPM: " + (playableAsset.behaviour.flashBpm.ToString());
                        break;
                    case LightFlashParameter.FlashMode.Line:
                        clip.displayName = "Flash: Line" + "   BPM: " + (playableAsset.behaviour.flashBpm.ToString());
                        break;
                    case LightFlashParameter.FlashMode.Switch:
                        clip.displayName = "Flash: Switch" + "   BPM: " + (playableAsset.behaviour.flashBpm.ToString());
                        break;
                    case LightFlashParameter.FlashMode.Split_3:
                        clip.displayName = "Flash: Split 3" + "   BPM: " + (playableAsset.behaviour.flashBpm.ToString());
                        break;
                    case LightFlashParameter.FlashMode.RandomSwitch:
                        clip.displayName = "Flash: Random" + "   BPM: " + (playableAsset.behaviour.flashBpm.ToString());
                        break;
                    case LightFlashParameter.FlashMode.UseReference:
                        clip.displayName = "Use Reference Flash";
                        break;
                }
            }
            return mixer;
        }
    }
}
