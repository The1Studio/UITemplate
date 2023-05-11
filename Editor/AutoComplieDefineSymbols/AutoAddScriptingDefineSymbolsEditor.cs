using System.Linq;
using UITemplate.Editor.AutoComplieDefineSymbols;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[InitializeOnLoad]
public class AutoAddScriptingDefineSymbolsEditor : AssetPostprocessor
{
    static AutoAddScriptingDefineSymbolsEditor() { EditorApplication.update += Initialize; }

    private static void Initialize()
    {
        EditorApplication.update                        -= Initialize;
        //TODO enable when use another way to cache the scripting defination
        CompilationPipeline.assemblyCompilationFinished += CompilationPipelineOnassemblyCompilationFinished;
    }

    private static void CompilationPipelineOnassemblyCompilationFinished(string outputPath, CompilerMessage[] compilerMessages)
    {
        var compilerErrorFound = false;

        foreach (var msg in compilerMessages)
        {
            if (msg.type == CompilerMessageType.Error && (msg.message.Contains("CS0246") || msg.message.Contains("CS0006")))
            {
                compilerErrorFound = true;
            }
        }

        var currentDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        var totalDefine   = currentDefine.Split(";").ToList();

        if (!compilerErrorFound)
        {
            UITemplateSettingDefineSymbol.Instance.AddCustomDefineSymbol(totalDefine);
        }
    }
}