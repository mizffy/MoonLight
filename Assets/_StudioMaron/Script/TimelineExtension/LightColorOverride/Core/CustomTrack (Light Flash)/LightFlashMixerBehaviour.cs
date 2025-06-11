using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace StudioMaron
{
    public class LightFlashMixerBehaviour : PlayableBehaviour
    {
        public TimelineClip[] Clips { get; set; }
        public PlayableDirector Director { get; set; }
        public LightFlashController TrackBinding { get; set; }

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
        }



        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var lightFlashController = playerData as LightFlashController;
            if (lightFlashController == null)
            {
                return;
            }

            var time = Director.time;
            var currentWeight = 0f;
            List<LightFlashParameter> lightFlashList = new List<LightFlashParameter>();

            // 有効なTimeline clipの数だけパラメータを作ってコントローラに渡す
            for (int i = 0; i < Clips.Length; i++)
            {
                var clip = Clips[i];
                var clipAsset = clip.asset as LightFlashClip;
                var behaviour = clipAsset.behaviour;
                var clipWeight = playable.GetInputWeight(i);
                var clipProgress = (float)((time - clip.start) / clip.duration);

                if (clipProgress >= 0.0f && clipProgress <= 1.0f)
                {
                    var param = new LightFlashParameter();

                    param.weight = clipWeight;
                    param.flashBpm = behaviour.flashBpm;
                    param.flashEveryBeatNumber = behaviour.flashEveryBeatNumber;
                    param.flashCurve = behaviour.flashCurve;
                    param.randomSwitchRate = behaviour.randomSwitchRate;
                    param.randomSeed = behaviour.randomSeed;
                    param.timeAdjust = behaviour.timeAdjust;
                    param.flashMode = behaviour.flashMode;
                    param.clipTime = (float)(time - clip.start);

                    lightFlashList.Add(param);
                    currentWeight += clipWeight;
                }
            }

            // Timeline clipがないときは処理をしない
            if (currentWeight != 0)
                lightFlashController.SetParameterFromTimeline(lightFlashList);
        }

        // TimelineのPreview終了時、特定の処理をする
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (TrackBinding == null) return;
            TrackBinding.ReapplyParameter();

        }

    }

}
