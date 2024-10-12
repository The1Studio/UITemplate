#if !UNITY_CLOUD_BUILD
namespace HeurekaGames.AssetHunterPRO
{
    using System.Linq;
    using UnityEngine;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    using UnityEditor.Build.Reporting;
    using UnityEditor.Build;

    internal class AH_BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport, IProcessSceneWithReport
    {
        public void OnProcessScene(UnityEngine.SceneManagement.Scene scene, BuildReport report)
        {
            //This was called on "Editor Play", so ignore
            if (report == null) return;

            //For some reason I have to do both recursive, and non-recursive version
            var dependencies = AssetDatabase.GetDependencies(scene.path, true);
            dependencies.ToList().AddRange(AssetDatabase.GetDependencies(scene.path, false));
            {
                foreach (var dependency in dependencies) this.processUsedAsset(scene.path, dependency);
            }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            this.initBuildReport(report.summary.platform, report.summary.outputPath);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            this.finalizeBuildReport(report);
        }

        private void finalizeBuildReport(BuildReport report)
        {
            this.addBuildReportInfo(report);

            //Dont force add special folders (resources etc) in 2018.1 because we have asccess to buildreport
            this.finalizeBuildReport(report.summary.platform);
        }

        private void addBuildReportInfo(BuildReport report)
        {
            if (buildInfo != null) buildInfo.ProcessBuildReport(report);
        }

        private static AH_SerializedBuildInfo buildInfo;

        private bool isProcessing;
        //private static bool isGenerating;

        private void initBuildReport(BuildTarget platform, string outputPath)
        {
            //Only start processing if its set in preferences
            this.isProcessing = AH_SettingsManager.Instance.AutoCreateLog /*|| isGenerating*/;

            if (this.isProcessing)
            {
                Debug.Log("AH: Initiated new buildreport - " + System.DateTime.Now);
                buildInfo = new();
            }
            else
            {
                Debug.Log("AH: Build logging not automatically started. Open Asset Hunter preferences if you want it to run");
            }
        }

        private void finalizeBuildReport(BuildTarget target)
        {
            if (this.isProcessing)
            {
                this.isProcessing = false;

                Debug.Log("AH: Finalizing new build report - " + System.DateTime.Now);

                buildInfo.FinalizeReport(target);
            }
        }

        private void processUsedAsset(string scenePath, string assetPath)
        {
            if (this.isProcessing) buildInfo.AddBuildDependency(scenePath, assetPath);
        }

        public int callbackOrder => 0;
    }
}
#endif