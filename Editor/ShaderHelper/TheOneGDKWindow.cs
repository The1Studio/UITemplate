namespace UITemplate.Editor.ShaderHelper
{
    using Core.AnalyticServices;
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
        
        [InlineEditor] [TabGroup("Game Events")]
        public GameEventsSetting GameEventsSetting;

        private void OnEnable()
        {
            this.AnalyticConfig      = Resources.Load<AnalyticConfig>($"GameConfigs/{nameof(AnalyticConfig)}");
            this.ThirdPartiesConfig  = Resources.Load<ThirdPartiesConfig>($"GameConfigs/{nameof(ThirdPartiesConfig)}");
            this.remoteConfigSetting = Resources.Load<RemoteConfigSetting>(RemoteConfigSetting.ResourcePath);
            this.GameEventsSetting = Resources.Load<GameEventsSetting>(GameEventsSetting.ResourcePath);

            if (this.remoteConfigSetting == null)
            {
                AssetDatabase.CreateAsset(CreateInstance<RemoteConfigSetting>(), $"Assets/Resources/{RemoteConfigSetting.ResourcePath}.asset");
                this.remoteConfigSetting = Resources.Load<RemoteConfigSetting>($"GameConfigs/{nameof(RemoteConfigSetting)}");
            }
            
            if (this.GameEventsSetting == null)
            {
                AssetDatabase.CreateAsset(CreateInstance<GameEventsSetting>(), $"Assets/Resources/{GameEventsSetting.ResourcePath}.asset");
                this.GameEventsSetting = Resources.Load<GameEventsSetting>($"GameConfigs/{nameof(GameEventsSetting)}");
            }

            this.ThirdPartiesConfig.AdSettings.AdMob.OnDataChange = (admobSetting) =>
            {
                EditorUtility.SetDirty(admobSetting);
                AssetDatabase.SaveAssets();
            };
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