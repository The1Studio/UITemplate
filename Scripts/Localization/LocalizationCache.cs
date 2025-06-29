#if THEONE_LOCALIZATION
namespace TheOneStudio.UITemplate.Localization
{
    using System.Collections.Generic;
    using BlueprintFlow.BlueprintReader;

    /// <summary>
    /// Manages registered blueprint readers for localization
    /// </summary>
    internal class LocalizationCache
    {
        private readonly Dictionary<IGenericBlueprintReader, List<LocalizedFieldInfo>> registeredReaders = new();

        /// <summary>
        /// Get localized fields for a blueprint reader by scanning it directly
        /// </summary>
        public List<LocalizedFieldInfo> GetLocalizedFields(IGenericBlueprintReader blueprintReader)
        {
            return CollectionScanner.ScanBlueprintReader(blueprintReader);
        }

        /// <summary>
        /// Register a blueprint reader with its localized fields
        /// </summary>
        public void RegisterReader(IGenericBlueprintReader blueprintReader, List<LocalizedFieldInfo> localizedFields)
        {
            this.registeredReaders[blueprintReader] = localizedFields;
        }

        /// <summary>
        /// Unregister a blueprint reader
        /// </summary>
        public void UnregisterReader(IGenericBlueprintReader blueprintReader)
        {
            this.registeredReaders.Remove(blueprintReader);
        }

        /// <summary>
        /// Get all registered readers and their localized fields
        /// </summary>
        public IEnumerable<KeyValuePair<IGenericBlueprintReader, List<LocalizedFieldInfo>>> GetRegisteredReaders()
        {
            return this.registeredReaders;
        }

        /// <summary>
        /// Get count of registered readers
        /// </summary>
        public int RegisteredCount => this.registeredReaders.Count;

        /// <summary>
        /// Clear all caches
        /// </summary>
        public void Clear()
        {
            this.registeredReaders.Clear();
        }
    }
}
#endif
