using UnityEditor;

/// <summary>
/// 複数選択・編集を可能にするためのEditor拡張
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
