namespace UITemplate.Editor.TheOneWindowTools
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
        [TabGroup("Analytics Config")]
        [InlineEditor]
        public AnalyticConfig AnalyticConfig;

        [TabGroup("Ads Config")]
        [InlineEditor]
        public ThirdPartiesConfig ThirdPartiesConfig;

        [InlineEditor]
        [TabGroup("Remote Config")]
        public RemoteConfigSetting remoteConfigSetting;

        [InlineEditor]
        [TabGroup("Game Features")]
        public GameFeaturesSetting gameFeaturesSetting;

        private void OnEnable()
        {
            this.AnalyticConfig      = ForceCreateAssetsIfNull<AnalyticConfig>("GameConfigs");
            this.ThirdPartiesConfig  = ForceCreateAssetsIfNull<ThirdPartiesConfig>("GameConfigs");
            this.remoteConfigSetting = ForceCreateAssetsIfNull<RemoteConfigSetting>("GameConfigs");
            this.gameFeaturesSetting = ForceCreateAssetsIfNull<GameFeaturesSetting>("GameConfigs");

            this.ThirdPartiesConfig.AdSettings.AdMob.OnDataChange = admobSetting =>
            {
                EditorUtility.SetDirty(admobSetting);
                AssetDatabase.SaveAssets();
            };
            return;

            T ForceCreateAssetsIfNull<T>(string configFolder, string assetName = null)
                where T : ScriptableObject
            {
                assetName ??= typeof(T).Name;
                if (Resources.Load<T>($"{configFolder}/{assetName}") is { } result)
                {
                    return result;
                }

                if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");
                if (!AssetDatabase.IsValidFolder($"Assets/Resources/{configFolder}")) AssetDatabase.CreateFolder("Assets/Resources", configFolder);

                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), $"Assets/Resources/{configFolder}/{assetName}.asset");

                return Resources.Load<T>($"{configFolder}/{assetName}");
            }
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

        [MenuItem("TheOne/Configuration And Tools")]
        private static void OpenWindow()
        {
            EditorWindow.GetWindow<TheOneWindow>().Show();
        }

        [InitializeOnLoadMethod]
        private static void OnReloadDomain()
        {
            OpenWindowIfNull<AnalyticConfig>("GameConfigs");
            OpenWindowIfNull<ThirdPartiesConfig>("GameConfigs");
            OpenWindowIfNull<RemoteConfigSetting>("GameConfigs");
            OpenWindowIfNull<GameFeaturesSetting>("GameConfigs");

            return;

            void OpenWindowIfNull<T>(string configFolder, string assetName = null)
                where T : ScriptableObject
            {
                assetName ??= typeof(T).Name;
                if (Resources.Load<T>($"{configFolder}/{assetName}") != null) return;
                Debug.LogError($"Cannot find {assetName}. Open TheOneWindow to create one.");
                TheOneWindow.OpenWindow();
            }
        }
    }
}