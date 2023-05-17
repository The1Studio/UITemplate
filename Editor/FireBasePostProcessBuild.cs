using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_WEBGL
public static class FireBasePostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.WebGL) return;
#if FIREBASE_WEBGL
        //Firebase Process build
        SetUpFirebaseForIndexHtml($"{pathToBuiltProject}/index.html");
#endif
    }

    static void SetUpFirebaseForIndexHtml(string htmlPath)
    {
        var firebaseWebGlPath = $"{Application.dataPath}/FirebaseWebglConfig.txt";

        if (!File.Exists(firebaseWebGlPath))
        {
            Debug.LogError($"Waring you are using firebase webgl but you don't have the file FirebaseWebglConfig.txt in {Application.dataPath}");

            return;
        }

        var fileStream = new FileStream(firebaseWebGlPath, FileMode.Open, FileAccess.Read);

        using var streamReader = new StreamReader(fileStream);

        var firebaseConfig = streamReader.ReadToEnd();
        var html           = ReadTextFile(htmlPath);
        ConditionRegexFirebase(htmlPath, html, "const firebaseConfig .*[\\s\\S]*};", firebaseConfig);
    }

    private static string ReadTextFile(string pFilePath)
    {
        using var streamReader = new StreamReader(pFilePath, Encoding.UTF8);
        var       html         = streamReader.ReadToEnd();

        return html;
    }

    private static void ConditionRegexFirebase(string htmlPath, string htmlInput, string regexCondition, string firebaseConfig)
    {
        var                         rx      = new Regex(regexCondition);
        var                         matches = rx.Matches(htmlInput);
        IDictionary<string, string> map     = new Dictionary<string, string>();

        foreach (Match match in matches)
        {
            var groups    = match.Groups;
            var newConfig = firebaseConfig;
            map.TryAdd(groups[0].Value, newConfig);
        }

        var regex       = new Regex(string.Join("|", map.Keys.Select(Regex.Escape)));
        var finalConfig = regex.Replace(htmlInput, m => map[m.Value]);
        File.WriteAllText(htmlPath, finalConfig);
    }
}
#endif