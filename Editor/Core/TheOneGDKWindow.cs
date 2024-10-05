namespace TheOne.Tool.Core
{
    using global::Core.AnalyticServices;
    using ServiceImplementation.Configs;
    using ServiceImplementation.FireBaseRemoteConfig;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using UnityEditor;
    using UnityEngine;

    public class TheOneWindow : OdinEditorWindow
    {
        [MenuItem("TheOne/Configuration And Tools")]
        private static void OpenWindow() { GetWindow<TheOneWindow>().Show(); }

        [TabGroup("Analytics Config")] [InlineEditor]
        public AnalyticConfig AnalyticConfig;

        [TabGroup("Ads Config")] [InlineEditor]
        public ThirdPartiesConfig ThirdPartiesConfig;

        [InlineEditor] [TabGroup("Remote Config")]
        public RemoteConfigSetting remoteConfigSetting;
        
        [InlineEditor] [TabGroup("Game Features")]
        public GameFeaturesSetting gameFeaturesSetting;

        private void OnEnable()
        {
            this.AnalyticConfig      = Resources.Load<AnalyticConfig>($"GameConfigs/{nameof(this.AnalyticConfig)}");
            this.ThirdPartiesConfig  = Resources.Load<ThirdPartiesConfig>($"GameConfigs/{nameof(this.ThirdPartiesConfig)}");
            this.remoteConfigSetting = Resources.Load<RemoteConfigSetting>(RemoteConfigSetting.ResourcePath);
            this.gameFeaturesSetting   = Resources.Load<GameFeaturesSetting>(GameFeaturesSetting.ResourcePath);

            if (this.remoteConfigSetting == null)
            {
                AssetDatabase.CreateAsset(CreateInstance<RemoteConfigSetting>(), $"Assets/Resources/{RemoteConfigSetting.ResourcePath}.asset");
                this.remoteConfigSetting = Resources.Load<RemoteConfigSetting>($"GameConfigs/{nameof(RemoteConfigSetting)}");
            }
            
            if (this.gameFeaturesSetting == null)
            {
                AssetDatabase.CreateAsset(CreateInstance<GameFeaturesSetting>(), $"Assets/Resources/{GameFeaturesSetting.ResourcePath}.asset");
                this.gameFeaturesSetting = Resources.Load<GameFeaturesSetting>($"GameConfigs/{nameof(this.gameFeaturesSetting)}");
            }

            this.ThirdPartiesConfig.AdSettings.AdMob.OnDataChange = (admobSetting) =>
            {
                EditorUtility.SetDirty(admobSetting);
                AssetDatabase.SaveAssets();
            };
        }

        protected override void OnImGUI()
        {
            base.OnImGUI();

            // If you make changes and want to save them back to the asset (optional)
            if (GUILayout.Button("Save Changes"))
            {
                EditorUtility.SetDirty(this.ThirdPartiesConfig);
                AssetDatabase.SaveAssets();
            }
        }
    }
}