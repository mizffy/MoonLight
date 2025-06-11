using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace StudioMaron
{
    public class LightGoboMixerBehaviour : PlayableBehaviour
    {
        public TimelineClip[] Clips { get; set; }
        public PlayableDirector Director { get; set; }
        public LightGoboController TrackBinding { get; set; }

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var controller = playerData as LightGoboController;
            if (controller == null)
            {
                return;
            }

            var time = Director.time;
            var currentWeight = 0f;
            List<LightGoboParameter> goboList = new List<LightGoboParameter>();

            // 有効なTimeline clipの数だけパラメータを作ってコントローラに渡す
            for (int i = 0; i < Clips.Length; i++)
            {
                var param = new LightGoboParameter();
                var clip = Clips[i];
                var clipAsset = clip.asset as LightGoboClip;
                var behaviour = clipAsset.behaviour;
                var clipWeight = playable.GetInputWeight(i);
                var clipProgress = (float)((time - clip.start) / clip.duration);
                if (clipProgress >= 0.0f && clipProgress < 1.0f)
                {
                    param.textureMode = behaviour.textureMode;
                    param.cookieTexture.AddRange(behaviour.cookieTexture);

                    param.rotationMode = behaviour.rotationMode;
                    param.rotationSpeed = behaviour.rotationSpeed;
                    param.randomSeed = behaviour.randomSeed;
                    currentWeight += clipWeight;
                    goboList.Add(param);
                }
            }

            // Timeline clipがないときは処理をしない
            if (currentWeight != 0)
                controller.SetParameterFromTimeline(goboList);
        }

        // TimelineのPreview終了時、特定の処理をする
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (TrackBinding == null) return;
            TrackBinding.ReapplyParameter();
        }
    }
}
