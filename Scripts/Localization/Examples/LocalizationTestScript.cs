namespace TheOneStudio.UITemplate.UITemplate.Localization.Examples
{
    using System.Collections;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOne.Simulation.Animal.Pet.Scripts.Blueprints;
    using TheOneStudio.UITemplate.UITemplate.Localization.Signals;
    using UnityEngine;

    /// <summary>
    /// Simple test script for blueprint localization system
    /// Attach to a GameObject and run to test language switching
    /// </summary>
    public class LocalizationTestScript : MonoBehaviour
    {
        [SerializeField] private string testSpecialItemId = "item_001";

        private LocalizationManager localizationManager;
        private SpecialItemBlueprint specialItemBlueprint;
        private SignalBus signalBus;

        private void Start()
        {
            // Get services from DI container
            this.localizationManager = ProjectContext.Current.Container.Resolve<LocalizationManager>();
            this.specialItemBlueprint = ProjectContext.Current.Container.Resolve<SpecialItemBlueprint>();
            this.signalBus = ProjectContext.Current.Container.Resolve<SignalBus>();

            // Subscribe to events
            this.signalBus.Subscribe<LanguageChangedSignal>(this.OnLanguageChanged);
            this.signalBus.Subscribe<BlueprintLocalizationCompletedSignal>(this.OnBlueprintLocalizationCompleted);

            // Start testing
            StartCoroutine(this.TestLanguages());
        }

        private void OnDestroy()
        {
            if (this.signalBus != null)
            {
                this.signalBus.Unsubscribe<LanguageChangedSignal>(this.OnLanguageChanged);
                this.signalBus.Unsubscribe<BlueprintLocalizationCompletedSignal>(this.OnBlueprintLocalizationCompleted);
            }
        }

        private IEnumerator TestLanguages()
        {
            yield return new WaitForSeconds(1f);

            Debug.Log("=== Blueprint Localization Test ===");
            
            // Test English
            yield return this.TestLanguage("en");
            yield return new WaitForSeconds(0.5f);
            
            // Test Vietnamese
            yield return this.TestLanguage("vi");
            yield return new WaitForSeconds(0.5f);
            
            // Test Spanish
            yield return this.TestLanguage("es");
            
            Debug.Log("=== Test Complete ===");
        }

        private IEnumerator TestLanguage(string languageCode)
        {
            Debug.Log($"Switching to language: {languageCode}");
            this.localizationManager.SetLanguage(languageCode);
            
            yield return new WaitForSeconds(0.1f);
            
            // Check blueprint content
            try
            {
                var item = this.specialItemBlueprint.GetDataById(this.testSpecialItemId);
                if (item != null)
                {
                    Debug.Log($"Item [{this.testSpecialItemId}] Content: '{item.Content}'");
                }
                else
                {
                    Debug.LogWarning($"Item '{this.testSpecialItemId}' not found");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error reading blueprint: {ex.Message}");
            }
        }

        private void OnLanguageChanged(LanguageChangedSignal signal)
        {
            Debug.Log($"Language changed to: {signal.NewLanguage}");
        }

        private void OnBlueprintLocalizationCompleted(BlueprintLocalizationCompletedSignal signal)
        {
            Debug.Log($"Localized {signal.LocalizedFieldsCount} fields in {signal.BlueprintCount} blueprints");
        }

        [ContextMenu("Test English")]
        public void TestEnglish() => StartCoroutine(this.TestLanguage("en"));

        [ContextMenu("Test Vietnamese")]
        public void TestVietnamese() => StartCoroutine(this.TestLanguage("vi"));

        [ContextMenu("Test Spanish")]
        public void TestSpanish() => StartCoroutine(this.TestLanguage("es"));
    }
}