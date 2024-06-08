namespace UITemplate.Editor.ProjectMigration.MigrationModules
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;
#if APPLOVIN
    using AppLovinMax.Scripts.IntegrationManager.Editor;
#endif

    public static class ApplovinMigration
    {
        public static async void DoMigrate()
        {
#if APPLOVIN
            var       url = $"https://unity.applovin.com/max/1.0/integration_manager_info?plugin_version={MaxSdk.Version}";
            using var www = UnityWebRequest.Get(url);
            var       operation = www.SendWebRequest();

            await operation;

            if (www.result != UnityWebRequest.Result.Success) return;

            PluginData pluginData;
            try
            {
                pluginData = JsonUtility.FromJson<PluginData>(www.downloadHandler.text);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                pluginData = null;
            }

            if (pluginData != null)
            {
                // Get current version of the plugin then upgrade
                var appLovinMax = pluginData.AppLovinMax;
                AppLovinIntegrationManager.UpdateCurrentVersions(appLovinMax, AppLovinIntegrationManager.PluginParentDirectory);
                var maxVersionComparisonResult = MaxSdkUtils.CompareVersions(MaxSdk.Version, appLovinMax.LatestVersions.Unity);
                if (maxVersionComparisonResult == MaxSdkUtils.VersionComparisonResult.Lesser)
                {
                    // Show a dialog with a message to upgrade AppLovin
                    EditorUtility.DisplayDialog(
                        "Upgrade AppLovin",
                        $"Your current version of AppLovin is {MaxSdk.Version}. The latest version is {appLovinMax.LatestVersions.Unity}. Please upgrade AppLovin to the latest version.",
                        "OK"
                    );

                    AppLovinEditorCoroutine.StartCoroutine(AppLovinIntegrationManager.Instance.DownloadPlugin(appLovinMax, false));
                    
                    AppLovinIntegrationManagerWindow.ShowManager();
                    EditorUtility.DisplayDialog(
                        "Upgrade AppLovin Adapters",
                        "One or more of your AppLovin adapters need to be upgraded. Please upgrade to the latest version.",
                        "OK"
                    );
                }
            }
#endif
        }
    }
}