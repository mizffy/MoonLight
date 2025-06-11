using UnityEditor;

/// <summary>
/// Light Angle Clipのエディタ拡張
/// ClipのOperation Modeで選択されたモードで表示する変数を変える
/// エディタ拡張をする都合上、複数選択・編集ができないので注意
/// </summary>
namespace StudioMaron
{
    [CustomEditor(typeof(LightAngleClip))]

    public class LightAngleClipEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            LightAngleClip clip = target as LightAngleClip;
            LightAngleBehaviour obj = clip.behaviour;

            obj.operationMode = (LightAngleParameter.OperationMode)EditorGUILayout.EnumPopup("Operation Mode", obj.operationMode);

            switch (obj.operationMode)
            {
                case LightAngleParameter.OperationMode.None:
                    break;
                case LightAngleParameter.OperationMode.All_Fixed:
                    obj.allFixed_Tilt = EditorGUILayout.Slider("Tilt", obj.allFixed_Tilt, -90, 90);
                    obj.allFixed_Pan = EditorGUILayout.Slider("Pan", obj.allFixed_Pan, -180, 180);
                    EditorGUILayout.Space(10);
                    obj.allFixed_NoiseRange = EditorGUILayout.Slider("Noise Range", obj.allFixed_NoiseRange, 0, 1);
                    obj.allFixed_NoiseSpeed = EditorGUILayout.Slider("Noise Speed", obj.allFixed_NoiseSpeed, 0, 1);
                    obj.noiseRandomSeed = EditorGUILayout.IntSlider("Noise Random Seed", obj.noiseRandomSeed, 1, 9999);
                    break;
                case LightAngleParameter.OperationMode.Line_Fixed:
                    obj.lineFixed_Tilt = EditorGUILayout.Slider("Tilt", obj.lineFixed_Tilt, -90, 90);
                    obj.lineFixed_Asymmetry = EditorGUILayout.Toggle("Tilt Asymmetry", obj.lineFixed_Asymmetry);
                    obj.lineFixed_TiltCurve = EditorGUILayout.CurveField("Tilt Curve", obj.lineFixed_TiltCurve);
                    EditorGUILayout.Space(10);
                    obj.lineFixed_Pan = EditorGUILayout.Slider("Pan", obj.lineFixed_Pan, -180, 180);
                    EditorGUILayout.Space(10);
                    obj.lineFixed_NoiseRange = EditorGUILayout.Slider("Noise Range", obj.lineFixed_NoiseRange, 0, 1);
                    obj.lineFixed_NoiseSpeed = EditorGUILayout.Slider("Noise Speed", obj.lineFixed_NoiseSpeed, 0, 1);
                    obj.noiseRandomSeed = EditorGUILayout.IntSlider("Noise Random Seed", obj.noiseRandomSeed, 1, 9999);
                    break;
                case LightAngleParameter.OperationMode.Random_Fixed:
                    EditorGUILayout.Space(10);
                    EditorGUILayout.MinMaxSlider("Tilt Range (Min: " + obj.randomFixed_MinTilt.ToString("F0") + "  Max: " + obj.randomFixed_MaxTilt.ToString("F0") + ")", ref obj.randomFixed_MinTilt, ref obj.randomFixed_MaxTilt, -90, 90);
                    EditorGUILayout.Space(10);
                    EditorGUILayout.MinMaxSlider("Pan Range (Min: " + obj.randomFixed_MinPan.ToString("F0") + "  Max: " + obj.randomFixed_MaxPan.ToString("F0") + ")", ref obj.randomFixed_MinPan, ref obj.randomFixed_MaxPan, -180, 180);
                    obj.randomFixed_RandomSeed = EditorGUILayout.IntSlider("Random Seed", obj.randomFixed_RandomSeed, 1, 9999);
                    EditorGUILayout.Space(10);
                    obj.randomFixed_NoiseRange = EditorGUILayout.Slider("Noise Range", obj.randomFixed_NoiseRange, 0, 1);
                    obj.randomFixed_NoiseSpeed = EditorGUILayout.Slider("Noise Speed", obj.randomFixed_NoiseSpeed, 0, 1);
                    obj.noiseRandomSeed = EditorGUILayout.IntSlider("Noise Random Seed", obj.noiseRandomSeed, 1, 9999);
                    break;
                case LightAngleParameter.OperationMode.Rotation:
                    obj.rotation_Tilt = EditorGUILayout.Slider("Tilt", obj.rotation_Tilt, 0, 90);
                    obj.rotation_CycleTime = EditorGUILayout.FloatField("Cycle Time", obj.rotation_CycleTime);
                    obj.rotation_Sync = EditorGUILayout.Slider("Sync", obj.rotation_Sync, 0, 1);
                    obj.rotation_ReverseRate = EditorGUILayout.Slider("Reverse Rate", obj.rotation_ReverseRate, 0, 1);
                    obj.rotation_RandomSeed = EditorGUILayout.IntSlider("Random Seed", obj.rotation_RandomSeed, 1, 9999);
                    break;
                case LightAngleParameter.OperationMode.Tilt_Wave:
                    EditorGUILayout.MinMaxSlider("Tilt Range (Min: " + obj.tiltWave_MinTilt.ToString("F0") + "  Max: " + obj.tiltWave_MaxTilt.ToString("F0") + ")", ref obj.tiltWave_MinTilt, ref obj.tiltWave_MaxTilt, -90, 90);
                    obj.tiltWave_CycleTime = EditorGUILayout.FloatField("Cycle Time", obj.tiltWave_CycleTime);
                    obj.tiltWave_Sync = EditorGUILayout.Slider("Sync", obj.tiltWave_Sync, 0, 1);
                    EditorGUILayout.Space(10);
                    obj.tiltWave_Pan = EditorGUILayout.Slider("Pan", obj.tiltWave_Pan, -180, 180);
                    break;
                case LightAngleParameter.OperationMode.Search_Light:
                    obj.searchLight_Range = EditorGUILayout.Slider("Range", obj.searchLight_Range, 0.01f, 1);
                    obj.searchLight_Speed = EditorGUILayout.Slider("Speed", obj.searchLight_Speed, 0.01f, 1);
                    obj.searchLight_RandomSeed = EditorGUILayout.IntSlider("Random Seed", obj.searchLight_RandomSeed, 1, 9999);
                    break;
                case LightAngleParameter.OperationMode.LookAt_Target:
                    obj.lookAtTarget_LookAtWorldOffset = EditorGUILayout.Vector3Field("Look at offset (world position)", obj.lookAtTarget_LookAtWorldOffset);
                    EditorGUILayout.Space(10);
                    obj.lookAtTarget_NoiseRange = EditorGUILayout.Slider("Noise Range", obj.lookAtTarget_NoiseRange, 0, 1);
                    obj.lookAtTarget_NoiseSpeed = EditorGUILayout.Slider("Noise Speed", obj.lookAtTarget_NoiseSpeed, 0, 1);
                    obj.noiseRandomSeed = EditorGUILayout.IntSlider("Noise Random Seed", obj.noiseRandomSeed, 1, 9999);
                    break;
                case LightAngleParameter.OperationMode.UseReference:
                    break;
            }

            // Undoの登録
            Undo.RecordObject(clip, "Light Angle Clip");

            // 変更フラグ
            EditorUtility.SetDirty(clip);
        }
    }
}

