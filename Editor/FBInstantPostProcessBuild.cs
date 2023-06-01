#if UNITY_WEBGL
using System.IO;
using GameFoundation.BuildScripts.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;

public static class FBInstantPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.WebGL) return;
        var webLoaderPath = Path.Combine(pathToBuiltProject, $"Build/{GameVersion.ProjectName}.loader.js");
        var scriptContent = File.ReadAllText(webLoaderPath);
    
        //  Facebook block the blob when UnityWebGL try to create, so it's needed to specify the relative path 
        scriptContent = scriptContent.Replace("URL.createObjectURL(new Blob([e],{type:\"application/javascript\"}))", $"\"Build/{GameVersion.ProjectName}.framework.js\"");
    
        File.WriteAllText(webLoaderPath, scriptContent);
    }
}
#endif