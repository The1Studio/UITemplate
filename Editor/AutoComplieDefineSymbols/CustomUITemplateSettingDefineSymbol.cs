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
        private int                      Selected = 0;
        private AnimBool                 customizeValues;
        private Dictionary<string, bool> fieldDictionary = new();

        private void OnEnable()
        {
            var script    = UITemplateSettingDefineSymbol.Instance;
            var fieldInfo = script.Partner.GetType().GetFields();
            this.fieldDictionary.Clear();

            this.customizeValues = new AnimBool(script.IsEnable);
            this.Selected        = fieldInfo.ToList().FindIndex(x => (bool)x.GetValue(script.Partner));

            this.InitOriginalData(script.AnalyticAndTracking.GetType().GetFields(), script.AnalyticAndTracking);
            this.InitOriginalData(script.Monetization.GetType().GetFields(), script.Monetization);
            this.InitOriginalData(script.UITemplateGameAndServices.GetType().GetFields(), script.UITemplateGameAndServices);
            this.InitOriginalData(script.Partner.GetType().GetFields(), script.Partner);
            this.InitOriginalData(script.CustomDefineSymbols.GetType().GetFields(), script.CustomDefineSymbols);

            this.customizeValues.valueChanged.AddListener(() =>
            {
                script.IsEnable = this.customizeValues.target;
                this.Repaint();
            });
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = UITemplateSettingDefineSymbol.Instance;

            this.customizeValues.target = EditorGUILayout.ToggleLeft("Allow Auto Setting Define Symbols", this.customizeValues.target);

            if (EditorGUILayout.BeginFadeGroup(this.customizeValues.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Analytic And Tracking", GUILayout.Height(30));
                EditorGUILayout.Space(5);
                this.SetDataForField(script.AnalyticAndTracking.GetType().GetFields(), script.AnalyticAndTracking);
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Monetization", GUILayout.Height(30));
                this.SetDataForField(script.Monetization.GetType().GetFields(), script.Monetization);
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("UITemplateGameAndServices", GUILayout.Height(30));
                this.SetDataForField(script.UITemplateGameAndServices.GetType().GetFields(), script.UITemplateGameAndServices);
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("CustomDefineSymbol", GUILayout.Height(30));
                this.CustomDropDownPartner(script);
                EditorGUILayout.Space(5);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFadeGroup();

            if (GUILayout.Button("Manual Refresh Editor", GUILayout.Height(50)))
            {
                script.Apply();
            }
        }

        private void InitOriginalData(FieldInfo[] propertyInfos, object objectContain)
        {
            foreach (var p in propertyInfos)
            {
                var isEnable   = (bool)p.GetValue(objectContain);
                var headerName = p.GetCustomAttribute<HeaderAttribute>().header;
                this.fieldDictionary.Add(headerName, isEnable);
            }
        }

        private void SetDataForField(FieldInfo[] propertyInfos, object objectContain)
        {
            foreach (var p in propertyInfos)
            {
                //var isEnable   = (bool)p.GetValue(objectContain);
                var headerName = p.GetCustomAttribute<HeaderAttribute>().header;
                this.fieldDictionary[headerName] = EditorGUILayout.Toggle(headerName, this.fieldDictionary[headerName]);
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