namespace TheOneStudio.UITemplate.UITemplate.Localization.Examples
{
    using System;
    using Cysharp.Threading.Tasks;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.UITemplate.Localization.Signals;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// Example script showing how to use the Blueprint Localization system
    /// </summary>
    [Preserve]
    public class LocalizationUsageExample : IInitializable, IDisposable
    {
        private readonly LocalizationManager localizationManager;
        private readonly SignalBus signalBus;

        [Preserve]
        public LocalizationUsageExample(LocalizationManager localizationManager, SignalBus signalBus)
        {
            this.localizationManager = localizationManager;
            this.signalBus = signalBus;
        }

        public void Initialize()
        {
            // Subscribe to localization completion signals
            this.signalBus.Subscribe<BlueprintLocalizationCompletedSignal>(this.OnBlueprintLocalizationCompleted);
            
            // Example: Change language after 5 seconds
            this.ExampleLanguageChangeAsync().Forget();
        }

        public void Dispose()
        {
            this.signalBus?.Unsubscribe<BlueprintLocalizationCompletedSignal>(this.OnBlueprintLocalizationCompleted);
        }

        private async UniTaskVoid ExampleLanguageChangeAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5));

            Debug.Log("[LocalizationExample] Changing language to Vietnamese...");
            await this.localizationManager.ChangeLanguageAsync("vi");

            await UniTask.Delay(TimeSpan.FromSeconds(5));

            Debug.Log("[LocalizationExample] Changing language back to English...");
            await this.localizationManager.ChangeLanguageAsync("en");
        }

        private void OnBlueprintLocalizationCompleted(BlueprintLocalizationCompletedSignal signal)
        {
            Debug.Log($"[LocalizationExample] Blueprint localization completed! " +
                     $"Language: {signal.Language}, " +
                     $"Localized Fields: {signal.LocalizedFieldsCount}, " +
                     $"Blueprints: {signal.BlueprintCount}");
        }

        /// <summary>
        /// Example of how to access blueprint data after localization
        /// </summary>
        public void ExampleAccessLocalizedBlueprint()
        {
            // You can access localized blueprint data normally:
            // this.View.TxtContent.text = this.specialItemBlueprint.GetDataById(model.SpecialItemId).Content;
            
            // The Content field will be automatically localized when language changes
            // because it's marked with [LocalizableField] attribute
            
            Debug.Log($"Current language: {this.localizationManager.CurrentLanguage}");
            Debug.Log($"Available languages: {string.Join(", ", this.localizationManager.AvailableLanguages)}");
        }
    }
}
