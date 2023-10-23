namespace UITemplate.Editor.ShaderHelper
{
    using Core.AnalyticServices;
    using ServiceImplementation.Configs;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using TheOneStudio.UITemplate.UITemplate.Scripts.Services;
    using UnityEditor;
    using UnityEngine;

    public class TheOneWindow : OdinEditorWindow
    {
        [MenuItem("TheOne/Configuration And Tools")]
        private static void OpenWindow() { GetWindow<TheOneWindow>().Show(); }
        
        [TabGroup("AnalyticsConfig")]
        [InlineEditor]
        public AnalyticConfig AnalyticConfig;
        [TabGroup("AdsConfig")]
        [InlineEditor]
        public ThirdPartiesConfig ThirdPartiesConfig;
        [InlineEditor]

        private void OnEnable()
        {
            this.AnalyticConfig     = Resources.Load<AnalyticConfig>("GameConfigs/AnalyticConfig");
            this.ThirdPartiesConfig = Resources.Load<ThirdPartiesConfig>("GameConfigs/ThirdPartiesConfig");
        }
        
        protected override void OnGUI()
        {
            base.OnGUI();
        
            // If you make changes and want to save them back to the asset (optional)
            if (GUILayout.Button("Save Changes"))
            {
                EditorUtility.SetDirty(this.ThirdPartiesConfig);
                AssetDatabase.SaveAssets();
            }
        }
    }
}