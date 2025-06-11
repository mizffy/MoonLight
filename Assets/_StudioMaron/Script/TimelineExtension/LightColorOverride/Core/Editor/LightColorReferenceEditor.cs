using UnityEditor;

namespace StudioMaron
{
    [CustomEditor(typeof(LightColorReference))]

    public class LightColorReferenceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            LightColorReference obj = target as LightColorReference;

            // Light Color ControllerをReferenceとする or Light Emission ControllerをReferenceとする
            obj.controllerType = (LightColorReference.LightColorControllerType)EditorGUILayout.EnumPopup("Controller Type", obj.controllerType);

            // Enumで選んだ方のコントローラだけInspectorに表示する
            if (obj.controllerType == LightColorReference.LightColorControllerType.LightColorController)
                obj.lightColorController = EditorGUILayout.ObjectField("Light Color Controller", obj.lightColorController, typeof(LightColorController), true) as LightColorController;
            else if (obj.controllerType == LightColorReference.LightColorControllerType.EmissionColorController)
                obj.emissionColorController = EditorGUILayout.ObjectField("Emission Color Controller", obj.emissionColorController, typeof(EmissionColorController), true) as EmissionColorController;

            // Undoにセット
            Undo.RecordObject(obj, "Light Color Reference");

            // 変更フラグのセット
            EditorUtility.SetDirty(obj);
        }
    }
}

