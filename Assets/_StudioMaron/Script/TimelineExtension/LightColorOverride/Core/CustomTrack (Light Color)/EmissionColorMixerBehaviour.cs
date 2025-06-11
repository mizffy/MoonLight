using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace StudioMaron
{
    public class EmissionColorMixerBehaviour : PlayableBehaviour
    {
        public TimelineClip[] Clips { get; set; }
        public PlayableDirector Director { get; set; }
        public EmissionColorController TrackBinding { get; set; }

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
        }



        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var lightColorController = playerData as EmissionColorController;
            if (lightColorController == null)
            {
                return;
            }

            var time = Director.time;
            var currentWeight = 0f;
            List<LightColorParameter> lightColorList = new List<LightColorParameter>();

            for (int i = 0; i < Clips.Length; i++)
            {
                var clip = Clips[i];
                var clipAsset = clip.asset as LightColorClip;
                var behaviour = clipAsset.behaviour;
                var clipWeight = playable.GetInputWeight(i);
                var clipProgress = (float)((time - clip.start) / clip.duration);

                // 有効なTimeline clipの数だけパラメータを作ってコントローラに渡す
                if (clipProgress >= 0.0f && clipProgress <= 1.0f)
                {
                    var param = new LightColorParameter();

                    param.weight = clipWeight;
                    param.intensityMultiplier = behaviour.intensityMultiplier;
                    param.gradientMode = behaviour.gradientMode;
                    param.gradientColor = behaviour.gradientColor;
                    param.gradientValue = behaviour.gradientValue;
                    param.colorScrollBPM = behaviour.colorScrollBPM;
                    param.colorScrollBeat = behaviour.colorScrollBeat;
                    param.overrideRamdomSeed = behaviour.overrideRamdomSeed;
                    param.randomSeed = behaviour.randomSeed;
                    param.operationMode = behaviour.operationMode;
                    param.mixWeight = behaviour.mixWeight;
                    param.clipTime = (float)(time - clip.start);

                    lightColorList.Add(param);
                    currentWeight += clipWeight;
                }
            }

            // Timeline clipがないときは処理をしない
            if (currentWeight != 0)
                lightColorController.SetParameterFromTimeline(lightColorList);
        }

        // TimelineのPreview終了時、特定の処理をする
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (TrackBinding == null) return;
            TrackBinding.ReapplyParameter();

        }

    }

}
