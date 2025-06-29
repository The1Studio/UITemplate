#if THEONE_LOCALIZATION
namespace TheOneStudio.UITemplate.Localization
{
    using System;
    using BlueprintFlow.BlueprintReader;
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Manages automatic localization of blueprint fields marked with LocalizedFieldAttribute.
    /// Listens for language change signals and updates all registered blueprint objects.
    /// </summary>
    public class BlueprintLocalizationManager : IInitializable, ILateDisposable
    {
        private readonly SignalBus signalBus;
        private readonly IEnumerable<IGenericBlueprintReader> blueprintReaders;
        private readonly LocalizationCache cache;
        
        public BlueprintLocalizationManager(SignalBus signalBus, IEnumerable<IGenericBlueprintReader> blueprintReaders)
        {
            this.signalBus = signalBus;
            this.blueprintReaders = blueprintReaders;
            this.cache = new LocalizationCache();
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<LanguageChangedSignal>(this.OnLanguageChanged);
        }

        public void LateDispose()
        {
            this.signalBus.TryUnsubscribe<LanguageChangedSignal>(this.OnLanguageChanged);
            this.cache.Clear();
        }

        /// <summary>
        /// Load và register tất cả blueprint readers có localized fields.
        /// Gọi method này sau khi tất cả blueprint đã load xong.
        /// </summary>
        public void LoadAllLocalizedFields()
        {
            foreach (var blueprintReader in this.blueprintReaders)
            {
                this.RegisterBlueprintReader(blueprintReader);
            }
            
            Debug.Log($"Registered {this.cache.RegisteredCount} blueprint readers for localization");
        }

        /// <summary>
        /// Register một blueprint reader cụ thể
        /// </summary>
        public void RegisterBlueprintReader(IGenericBlueprintReader blueprintReader)
        {
            if (blueprintReader == null) return;

            try
            {
                var localizedFields = this.cache.GetLocalizedFields(blueprintReader);
                if (localizedFields.Count > 0)
                {
                    this.cache.RegisterReader(blueprintReader, localizedFields);
                    LocalizationService.LocalizeFields(localizedFields);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to register blueprint reader {blueprintReader.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Unregister một blueprint reader
        /// </summary>
        public void UnregisterBlueprintReader(IGenericBlueprintReader blueprintReader)
        {
            this.cache.UnregisterReader(blueprintReader);
        }

        private void OnLanguageChanged(LanguageChangedSignal signal)
        {
            this.LocalizeAllRegisteredReaders();
        }

        private void LocalizeAllRegisteredReaders()
        {
            foreach (var kvp in this.cache.GetRegisteredReaders())
            {
                LocalizationService.LocalizeFields(kvp.Value);
            }
        }
    }
}
#endif
