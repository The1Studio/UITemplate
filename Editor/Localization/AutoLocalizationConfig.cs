#if THEONE_LOCALIZATION
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TheOne.Tool.Localization
{
    [CreateAssetMenu(fileName = "AutoLocalizationConfig", menuName = "TheOne/Localization/Auto Localization Config")]
    public class AutoLocalizationConfig : ScriptableObject
    {
        [Title("OpenAI Configuration")]
        [SerializeField, HideInInspector] private string openAIApiKey = "";
        
        [PropertySpace(10)]
        [LabelText("OpenAI API Key")]
        [ShowInInspector]
        [PropertyOrder(1)]
        public string OpenAIApiKey
        {
            get => this.openAIApiKey;
            set => this.openAIApiKey = value;
        }
        
        [LabelText("GPT Model")]
        [PropertyOrder(2)]
        public string gptModel = "gpt-3.5-turbo";
        
        [Title("Localization Settings")]
        [LabelText("Source Language")]
        [PropertyOrder(3)]
        public SystemLanguage sourceLanguage = SystemLanguage.English;
        
        [LabelText("Target Languages")]
        [PropertyOrder(4)]
        public List<LocalizationLanguage> targetLanguages = new List<LocalizationLanguage>
        {
            new LocalizationLanguage { language = SystemLanguage.English, localeCode = "en", isEnabled = true },
            new LocalizationLanguage { language = SystemLanguage.Vietnamese, localeCode = "vi", isEnabled = true },
            new LocalizationLanguage { language = SystemLanguage.Japanese, localeCode = "ja", isEnabled = false },
            new LocalizationLanguage { language = SystemLanguage.Korean, localeCode = "ko", isEnabled = false },
            new LocalizationLanguage { language = SystemLanguage.Chinese, localeCode = "zh", isEnabled = false },
            new LocalizationLanguage { language = SystemLanguage.Spanish, localeCode = "es", isEnabled = false },
            new LocalizationLanguage { language = SystemLanguage.French, localeCode = "fr", isEnabled = false },
            new LocalizationLanguage { language = SystemLanguage.German, localeCode = "de", isEnabled = false },
        };
        
        [Title("Translation Settings")]
        [LabelText("Auto Translate on Build")]
        [PropertyOrder(5)]
        public bool autoTranslateOnBuild = true;
        
        [LabelText("Batch Size for Translation")]
        [Range(1, 20)]
        [PropertyOrder(6)]
        public int translationBatchSize = 5;
        
        [LabelText("Translation Delay (seconds)")]
        [Range(0.5f, 3f)]
        [PropertyOrder(7)]
        public float translationDelay = 1f;
        
        [Title("Context Settings")]
        [LabelText("Game Context")]
        [TextArea(3, 5)]
        [PropertyOrder(8)]
        public string gameContext = "This is a pet simulation game where players take care of virtual pets, feed them, play with them, and watch them grow.";
        
        [LabelText("Translation Instructions")]
        [TextArea(3, 5)]
        [PropertyOrder(9)]
        public string translationInstructions = "Translate the following text to {language}. Keep the tone casual and friendly, suitable for a mobile game audience. Maintain any formatting tags like <color> or <b>.";
        
        [Title("Actions")]
        [Button("Test OpenAI Connection", ButtonSizes.Large)]
        [PropertyOrder(10)]
        public void TestOpenAIConnection()
        {
            AutoTranslationManager.TestOpenAIConnection(this);
        }
        
        [Button("Translate All Missing Entries", ButtonSizes.Large)]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        [PropertyOrder(11)]
        public void TranslateAllMissingEntries()
        {
            AutoTranslationManager.TranslateAllMissingEntries(this);
        }
    }
    
    [Serializable]
    public class LocalizationLanguage
    {
        [LabelText("Language")]
        public SystemLanguage language;
        
        [LabelText("Locale Code")]
        public string localeCode;
        
        [LabelText("Enabled")]
        public bool isEnabled = true;
        
        [LabelText("Display Name")]
        public string displayName = "";
        
        public string GetDisplayName()
        {
            return string.IsNullOrEmpty(this.displayName) ? this.language.ToString() : this.displayName;
        }
    }
}
#endif
