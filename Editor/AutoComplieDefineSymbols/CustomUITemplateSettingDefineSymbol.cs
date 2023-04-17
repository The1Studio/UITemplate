namespace UITemplate.Editor.AutoComplieDefineSymbols
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(UITemplateSettingDefineSymbol))]
    public class CustomUITemplateSettingDefineSymbol : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = UITemplateSettingDefineSymbol.Instance;

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Manual Refresh Editor", GUILayout.Height(50)))
            {
                script.Apply();
            }
        }
    }
}