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

            // Light Color Controller��Reference�Ƃ��� or Light Emission Controller��Reference�Ƃ���
            obj.controllerType = (LightColorReference.LightColorControllerType)EditorGUILayout.EnumPopup("Controller Type", obj.controllerType);

            // Enum�őI�񂾕��̃R���g���[������Inspector�ɕ\������
            if (obj.controllerType == LightColorReference.LightColorControllerType.LightColorController)
                obj.lightColorController = EditorGUILayout.ObjectField("Light Color Controller", obj.lightColorController, typeof(LightColorController), true) as LightColorController;
            else if (obj.controllerType == LightColorReference.LightColorControllerType.EmissionColorController)
                obj.emissionColorController = EditorGUILayout.ObjectField("Emission Color Controller", obj.emissionColorController, typeof(EmissionColorController), true) as EmissionColorController;

            // Undo�ɃZ�b�g
            Undo.RecordObject(obj, "Light Color Reference");

            // �ύX�t���O�̃Z�b�g
            EditorUtility.SetDirty(obj);
        }
    }
}

