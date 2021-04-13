using UnityEditor;
using UnityEditorInternal;

namespace FMODUnity
{
    [CustomEditor(typeof(StudioListener))]
    [CanEditMultipleObjects]
    public class StudioListenerEditor : Editor
    {
        public SerializedProperty attenuationObject;

        private void OnEnable()
        {
            attenuationObject = serializedObject.FindProperty("attenuationObject");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginDisabledGroup(true);
            var index = serializedObject.FindProperty("ListenerNumber");
            EditorGUILayout.IntSlider(index, 0, FMOD.CONSTANTS.MAX_LISTENERS - 1, "Listener Index");
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();
            var mask = serializedObject.FindProperty("occlusionMask");
            int temp = EditorGUILayout.MaskField("Occlusion Mask", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(mask.intValue), InternalEditorUtility.layers);
            mask.intValue = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(temp);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.PropertyField(attenuationObject);
            serializedObject.ApplyModifiedProperties();
        }
    }
}