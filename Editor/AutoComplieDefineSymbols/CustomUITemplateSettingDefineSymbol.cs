namespace UITemplate.Editor.AutoComplieDefineSymbols
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using UnityEngine;

    [CustomEditor(typeof(UITemplateSettingDefineSymbol))]
    public class CustomUITemplateSettingDefineSymbol : Editor
    {
        private int      selected = 0;
        private AnimBool customizeValues;

        private void OnEnable()
        {
            var script    = UITemplateSettingDefineSymbol.Instance;
            var fieldInfo = script.Partner.GetType().GetFields();

            this.customizeValues = new AnimBool(script.IsEnable);
            this.selected        = fieldInfo.ToList().FindIndex(x => (bool)x.GetValue(script.Partner));

            this.customizeValues.valueChanged.AddListener(() =>
            {
                script.IsEnable = this.customizeValues.target;
                this.Repaint();
            });
        }

        public override void OnInspectorGUI()
        {
            var script = UITemplateSettingDefineSymbol.Instance;

            var style = new GUIStyle
            {
                normal =
                {
                    textColor = Color.green
                },
                fontSize  = 18,
                fontStyle = FontStyle.Bold
            };

            this.customizeValues.target = EditorGUILayout.ToggleLeft("ENABLE", this.customizeValues.target, style);

           

            if (this.customizeValues.target)
            {
                base.OnInspectorGUI();
                EditorGUILayout.Space(5);
                this.CustomDropDownPartner(script);
            }

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
                var isEnable = this.selected == index;
                p.SetValue(script.Partner, isEnable);
                var headerName = p.GetCustomAttribute<HeaderAttribute>().header;
                options.Add($"{headerName}");
            }

            var style = new GUIStyle
            {
                normal =
                {
                    textColor = Color.cyan
                },
                fontSize  = 15,
                fontStyle = FontStyle.Bold
            };

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("PARTNERS", style);

            this.selected = EditorGUILayout.Popup(this.selected, options.ToArray(), GUILayout.Height(30));
            EditorGUILayout.EndHorizontal();
        }
    }
}