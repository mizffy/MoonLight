using UnityEditor;

/// <summary>
/// �����I���E�ҏW���\�ɂ��邽�߂�Editor�g��
/// </summary>
namespace StudioMaron
{
    [CustomEditor(typeof(LightGoboClip))]
    [CanEditMultipleObjects]
    public class LightGoboClipEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
        }
    }
}
