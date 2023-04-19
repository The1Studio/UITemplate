namespace UITemplate.Editor.AutoComplieDefineSymbols
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(UITemplateSettingDefineSymbol))]
    public class CustomUITemplateSettingDefineSymbol : Editor
    {
        private int Selected = 0;

        private void OnEnable()
        {
            var script    = UITemplateSettingDefineSymbol.Instance;
            var fieldInfo = script.Partner.GetType().GetFields();

            this.Selected = fieldInfo.ToList().FindIndex(x => (bool)x.GetValue(script.Partner));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = UITemplateSettingDefineSymbol.Instance;

            EditorGUILayout.Space(20);

            this.CustomDropDownPartner(script);
            EditorGUILayout.Space(20);

            if (GUILayout.Button("Manual Refresh Editor", GUILayout.Height(50)))
            {
                script.Apply();
            }
        }

        private void CustomDropDownPartner(UITemplateSettingDefineSymbol script)
        {
            var fieldInfo = script.Partner.GetType().GetFields();

            var options = new List<string>();

            for (var index = 0; index < fieldInfo.Length; index++)
            {
                var p        = fieldInfo[index];
                var isEnable = this.Selected == index;
                p.SetValue(script.Partner, isEnable);
                var headerName = p.GetCustomAttribute<HeaderAttribute>().header;
                options.Add($"{headerName}");
            }

            this.Selected = EditorGUILayout.Popup("Partners", this.Selected, options.ToArray());
        }
    }
}