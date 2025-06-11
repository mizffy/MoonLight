using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace StudioMaron
{
    public class LightAngleMixerBehaviour : PlayableBehaviour
    {
        public TimelineClip[] Clips { get; set; }
        public PlayableDirector Director { get; set; }
        public LightAngleController TrackBinding { get; set; }

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
        }



        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var lightAngleController = playerData as LightAngleController;
            if (lightAngleController == null)
            {
                return;
            }

            var time = Director.time;
            var currentWeight = 0f;
            List<LightAngleParameter> lightAngleList = new List<LightAngleParameter>();

            for (int i = 0; i < Clips.Length; i++)
            {
                var clip = Clips[i];
                var clipAsset = clip.asset as LightAngleClip;
                var behaviour = clipAsset.behaviour;
                var clipWeight = playable.GetInputWeight(i);
                var clipProgress = (float)((time - clip.start) / clip.duration);

                // �L����Timeline clip�̐������p�����[�^������ăR���g���[���ɓn��
                if (clipProgress >= 0.0f && clipProgress <= 1.0f)
                {
                    var param = new LightAngleParameter();

                    param.weight = clipWeight;
                    param.clipTime = (float)(time - clip.start);

                    param.operationMode = behaviour.operationMode;

                    param.isInstant = behaviour.isInstant;
                    param.allFixed_Pan = behaviour.allFixed_Pan;
                    param.allFixed_Tilt = behaviour.allFixed_Tilt;
                    param.allFixed_NoiseRange = behaviour.allFixed_NoiseRange;
                    param.allFixed_NoiseSpeed = behaviour.allFixed_NoiseSpeed;

                    param.lineFixed_Pan = behaviour.lineFixed_Pan;
                    param.lineFixed_Tilt = behaviour.lineFixed_Tilt;
                    param.lineFixed_TiltCurve = behaviour.lineFixed_TiltCurve;
                    param.lineFixed_Asymmetry = behaviour.lineFixed_Asymmetry;
                    param.lineFixed_NoiseRange = behaviour.lineFixed_NoiseRange;
                    param.lineFixed_NoiseSpeed = behaviour.lineFixed_NoiseSpeed;

                    param.randomFixed_MaxPan = behaviour.randomFixed_MaxPan;
                    param.randomFixed_MinPan = behaviour.randomFixed_MinPan;
                    param.randomFixed_MaxTilt = behaviour.randomFixed_MaxTilt;
                    param.randomFixed_MinTilt = behaviour.randomFixed_MinTilt;
                    param.randomFixed_RandomSeed = behaviour.randomFixed_RandomSeed;
                    param.randomFixed_NoiseRange = behaviour.randomFixed_NoiseRange;
                    param.randomFixed_NoiseSpeed = behaviour.randomFixed_NoiseSpeed;

                    param.rotation_Tilt = behaviour.rotation_Tilt;
                    param.rotation_CycleTime = behaviour.rotation_CycleTime;
                    param.rotation_Sync = behaviour.rotation_Sync;
                    param.rotation_ReverseRate = behaviour.rotation_ReverseRate;
                    param.rotation_RandomSeed = behaviour.rotation_RandomSeed;

                    param.tiltWave_MaxTilt = behaviour.tiltWave_MaxTilt;
                    param.tiltWave_MinTilt = behaviour.tiltWave_MinTilt;
                    param.tiltWave_CycleTime = behaviour.tiltWave_CycleTime;
                    param.tiltWave_Sync = behaviour.tiltWave_Sync;
                    param.tiltWave_Pan = behaviour.tiltWave_Pan;

                    param.searchLight_Range = behaviour.searchLight_Range;
                    param.searchLight_Speed = behaviour.searchLight_Speed;
                    param.searchLight_RandomSeed = behaviour.searchLight_RandomSeed;

                    param.lookAtTarget_LookAtWorldOffset = behaviour.lookAtTarget_LookAtWorldOffset;
                    param.lookAtTarget_NoiseRange = behaviour.lookAtTarget_NoiseRange;
                    param.lookAtTarget_NoiseSpeed = behaviour.lookAtTarget_NoiseSpeed;

                    param.noiseRandomSeed = behaviour.noiseRandomSeed;
                    
                    lightAngleList.Add(param);
                    currentWeight += clipWeight;
                }
            }

            // Timeline clip���Ȃ��Ƃ��͏��������Ȃ�
            if (currentWeight != 0)
                lightAngleController.SetParameterFromTimeline(lightAngleList);
        }

        // Timeline��Preview�I�����A����̏���������
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (TrackBinding == null) return;
            TrackBinding.ReapplyParameter();

        }

    }

}
