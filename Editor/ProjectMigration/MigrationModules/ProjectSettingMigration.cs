namespace TheOne.Tool.Migration.ProjectMigration.MigrationModules
{
    using UnityEditor;

    public static class ProjectSettingMigration
    {
        public static void APICompatibilityLevel()
        {
            if (PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Android) != ApiCompatibilityLevel.NET_Unity_4_8)
            {
                PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_Unity_4_8);
                AssetDatabase.SaveAssets();
            }

            if (PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.iOS) != ApiCompatibilityLevel.NET_Unity_4_8)
            {
                PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_Unity_4_8);
                AssetDatabase.SaveAssets();
            }
            
            if (PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.WebGL) != ApiCompatibilityLevel.NET_Unity_4_8)
            {
                PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.WebGL, ApiCompatibilityLevel.NET_Unity_4_8);
                AssetDatabase.SaveAssets();
            }
        }
    }
}