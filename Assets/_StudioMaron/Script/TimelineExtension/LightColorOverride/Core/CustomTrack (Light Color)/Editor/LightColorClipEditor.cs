using UnityEditor;

/// <summary>
/// �����I���E�ҏW���\�ɂ��邽�߂�Editor�g��
/// </summary>
namespace StudioMaron
{
    [CustomEditor(typeof(LightColorClip))]
    [CanEditMultipleObjects]
    public class LightColorClipEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
        }
    }
}
