#if THEONE_LOCALIZATION
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace TheOne.Tool.Localization
{
    public static class AutoTranslationManager
    {
        private static readonly HttpClient httpClient = new HttpClient();
        
        public static void TestOpenAIConnection(AutoLocalizationConfig config)
        {
            _ = TestOpenAIConnectionAsync(config);
        }
        
        public static void TranslateAllMissingEntries(AutoLocalizationConfig config)
        {
            _ = TranslateAllMissingEntriesAsync(config);
        }
        
        private static async Task TestOpenAIConnectionAsync(AutoLocalizationConfig config)
        {
            try
            {
                if (string.IsNullOrEmpty(config.OpenAIApiKey))
                {
                    EditorUtility.DisplayDialog("API Key Missing", "Please set your OpenAI API Key in the config.", "OK");
                    return;
                }
                
                var result = await TranslateTextAsync("Hello, World!", "Vietnamese", config);
                EditorUtility.DisplayDialog("OpenAI Test", $"✅ Success! Translation: {result}", "OK");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("OpenAI Test Failed", $"❌ Error: {e.Message}", "OK");
            }
        }
        
        private static async Task TranslateAllMissingEntriesAsync(AutoLocalizationConfig config)
        {
            try
            {
                if (string.IsNullOrEmpty(config.OpenAIApiKey))
                {
                    EditorUtility.DisplayDialog("API Key Missing", "Please set your OpenAI API Key in the config.", "OK");
                    return;
                }
                
                var collections = LocalizationEditorSettings.GetStringTableCollections();
                var translationCount = 0;
                var totalMissing = 0;
                
                // Count missing translations first
                foreach (var collection in collections)
                {
                    foreach (var sharedEntry in collection.SharedData.Entries)
                    {
                        // Get source text (English)
                        var sourceTable = collection.GetTable("en") as StringTable;
                        var sourceEntry = sourceTable?.GetEntry(sharedEntry.Id);
                        if (sourceEntry == null || string.IsNullOrEmpty(sourceEntry.Value))
                            continue;
                            
                        foreach (var targetLang in config.targetLanguages.Where(l => l.isEnabled && l.localeCode != "en"))
                        {
                            var targetTable = collection.GetTable(targetLang.localeCode) as StringTable;
                            var targetEntry = targetTable?.GetEntry(sharedEntry.Id);
                            
                            if (targetEntry == null || string.IsNullOrEmpty(targetEntry.Value))
                            {
                                totalMissing++;
                            }
                        }
                    }
                }
                
                if (totalMissing == 0)
                {
                    EditorUtility.DisplayDialog("Translation Complete", "🎉 All entries are already translated!", "OK");
                    return;
                }
                
                if (!EditorUtility.DisplayDialog("Confirm Translation", 
                    $"Found {totalMissing} missing translations.\n\nThis will use OpenAI API and may cost money.\n\nContinue?", 
                    "Yes", "Cancel"))
                {
                    return;
                }
                
                EditorUtility.DisplayProgressBar("Translating", "Preparing...", 0f);
                
                // Start translation
                foreach (var collection in collections)
                {
                    foreach (var sharedEntry in collection.SharedData.Entries)
                    {
                        // Get source text (English)
                        var sourceTable = collection.GetTable("en") as StringTable;
                        var sourceEntry = sourceTable?.GetEntry(sharedEntry.Id);
                        if (sourceEntry == null || string.IsNullOrEmpty(sourceEntry.Value))
                            continue;
                            
                        var sourceText = sourceEntry.Value;
                        
                        foreach (var targetLang in config.targetLanguages.Where(l => l.isEnabled && l.localeCode != "en"))
                        {
                            var targetTable = collection.GetTable(targetLang.localeCode) as StringTable;
                            if (targetTable == null)
                            {
                                // Create table if it doesn't exist
                                var locale = LocalizationEditorSettings.GetLocales()
                                    .FirstOrDefault(l => l.Identifier.Code == targetLang.localeCode);
                                if (locale != null)
                                {
                                    targetTable = collection.AddNewTable(locale.Identifier) as StringTable;
                                }
                            }
                            
                            if (targetTable == null) continue;
                            
                            var targetEntry = targetTable.GetEntry(sharedEntry.Id);
                            
                            if (targetEntry == null || string.IsNullOrEmpty(targetEntry.Value))
                            {
                                var progress = (float)translationCount / totalMissing;
                                EditorUtility.DisplayProgressBar("Translating", 
                                    $"Translating '{sharedEntry.Key}' to {targetLang.language} ({translationCount + 1}/{totalMissing})", 
                                    progress);
                                
                                try
                                {
                                    var translatedText = await TranslateTextAsync(sourceText, targetLang.language.ToString(), config);
                                    
                                    if (targetEntry == null)
                                    {
                                        targetTable.AddEntry(sharedEntry.Id, translatedText);
                                    }
                                    else
                                    {
                                        targetEntry.Value = translatedText;
                                    }
                                    
                                    EditorUtility.SetDirty(targetTable);
                                    translationCount++;
                                    
                                    Debug.Log($"✅ Translated '{sharedEntry.Key}' to {targetLang.language}: {translatedText}");
                                    
                                    // Delay between API calls
                                    await Task.Delay((int)(config.translationDelay * 1000));
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError($"Failed to translate '{sharedEntry.Key}' to {targetLang.language}: {e.Message}");
                                    translationCount++; // Still count it to avoid infinite loop
                                }
                            }
                        }
                    }
                }
                
                EditorUtility.ClearProgressBar();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                EditorUtility.DisplayDialog("Translation Complete", 
                    $"🎉 Completed! Translated {translationCount} entries.\n\nCheck Console for details.", "OK");
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"Translation failed: {e.Message}");
                EditorUtility.DisplayDialog("Translation Failed", $"❌ Error: {e.Message}", "OK");
            }
        }
        
        private static async Task<string> TranslateTextAsync(string text, string targetLanguage, AutoLocalizationConfig config)
        {
            var requestBody = new
            {
                model = config.gptModel,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = $"You are a professional game translator. {config.gameContext}\n\n{config.translationInstructions.Replace("{language}", targetLanguage)}"
                    },
                    new
                    {
                        role = "user",
                        content = $"Translate this text to {targetLanguage}: \"{text}\""
                    }
                },
                max_tokens = 1000,
                temperature = 0.3
            };
            
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.OpenAIApiKey}");
            
            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI API error: {response.StatusCode} - {responseContent}");
            }
            
            dynamic result = JsonConvert.DeserializeObject(responseContent);
            return result.choices[0].message.content.ToString().Trim().Trim('"');
        }
        
        /// <summary>
        /// Get all localization entries for overview
        /// </summary>
        public static List<LocalizationEntryInfo> GetAllLocalizationEntries()
        {
            var entries = new List<LocalizationEntryInfo>();
            var collections = LocalizationEditorSettings.GetStringTableCollections();
            
            foreach (var collection in collections)
            {
                foreach (var sharedEntry in collection.SharedData.Entries)
                {
                    var entry = new LocalizationEntryInfo
                    {
                        key = sharedEntry.Key,
                        collection = collection.name,
                        translations = new Dictionary<string, string>()
                    };
                    
                    foreach (var table in collection.Tables)
                    {
                        if (table.asset is StringTable stringTable)
                        {
                            var tableEntry = stringTable.GetEntry(sharedEntry.Id);
                            if (tableEntry != null && !string.IsNullOrEmpty(tableEntry.Value))
                            {
                                entry.translations[stringTable.LocaleIdentifier.Code] = tableEntry.Value;
                            }
                        }
                    }
                    
                    entries.Add(entry);
                }
            }
            
            return entries;
        }
    }
    
    [Serializable]
    public class LocalizationEntryInfo
    {
        public string key;
        public string collection;
        public Dictionary<string, string> translations;
    }
}
#endif