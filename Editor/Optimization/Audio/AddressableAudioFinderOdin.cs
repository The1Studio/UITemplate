namespace TheOne.Tool.Optimization.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEngine;

    public class AddressableAudioFinderOdin : OdinEditorWindow
    {
        [ShowInInspector] [ListDrawerSettings(NumberOfItemsPerPage = 4)] [TableList] [Title("Wrong Compression Audios", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<AudioInfo> wrongCompressionAudioInfoList = new();

        [ShowInInspector] [ListDrawerSettings(NumberOfItemsPerPage = 4)] [TableList] [Title("Compressed Short Audios", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<AudioInfo> shortCompressedAudioInfoList = new();

        [ShowInInspector] [ListDrawerSettings(NumberOfItemsPerPage = 4)] [TableList] [Title("Compressed Long Audios", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<AudioInfo> longCompressedAudioInfoList = new();

        [ShowInInspector] [HideReferenceObjectPicker] [Title("Audio Compress Settings", TitleAlignment = TitleAlignments.Centered)]
        private AudioCompressSetting audioCompressSetting = new();

        [MenuItem("TheOne/List And Optimize/Audio List")]
        private static void OpenWindow() { GetWindow<AddressableAudioFinderOdin>().Show(); }

        [ButtonGroup("Action")]
        [Button(ButtonSizes.Medium)]
        private void FindAllAudioAndImporter() { this.FindAudiosInAddressables(); }

        [ButtonGroup("Action")]
        [Button(ButtonSizes.Medium)]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        private void CompressAndFixAll()
        {
            var totalSteps  = this.wrongCompressionAudioInfoList.Count;
            var currentStep = 0;
            foreach (var itemInfo in this.wrongCompressionAudioInfoList)
            {
                EditorUtility.DisplayProgressBar("Refreshing Audios", "Processing Addressables", currentStep / (float)totalSteps);
                var audioSetting = itemInfo.Importer.defaultSampleSettings;
                var isLongSound  = this.IsLongSound(itemInfo.Audio);

                itemInfo.Importer.forceToMono      = true;
                itemInfo.Importer.loadInBackground = true;
                audioSetting.loadType              = isLongSound ? AudioClipLoadType.CompressedInMemory : AudioClipLoadType.DecompressOnLoad;
                audioSetting.preloadAudioData      = !isLongSound;
                audioSetting.compressionFormat     = AudioCompressionFormat.Vorbis;
                audioSetting.quality               = this.audioCompressSetting.quality;

                // setting normalize
                var serializedObject = new SerializedObject(itemInfo.Importer);
                var normalize        = serializedObject.FindProperty("m_Normalize");
                normalize.boolValue = false;
                serializedObject.ApplyModifiedProperties();

                itemInfo.Importer.defaultSampleSettings = audioSetting;
                itemInfo.Importer.SaveAndReimport();
            }

            EditorUtility.ClearProgressBar();
            this.FindAudiosInAddressables();
        }

        private void FindAudiosInAddressables()
        {
            this.wrongCompressionAudioInfoList.Clear();
            this.shortCompressedAudioInfoList.Clear();
            this.longCompressedAudioInfoList.Clear();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings)
            {
                var totalSteps  = settings.groups.Sum(group => group.entries.Count);
                var currentStep = 0;
                foreach (var group in settings.groups)
                {
                    foreach (var entry in group.entries)
                    {
                        EditorUtility.DisplayProgressBar("Refreshing Audios", "Processing Addressables", currentStep / (float)totalSteps);

                        var path = AssetDatabase.GUIDToAssetPath(entry.guid);

                        var dependencies = this.GetAllDependencies(path);

                        foreach (var item in dependencies.Select(depPath => AssetDatabase.LoadAssetAtPath<AudioClip>(depPath)).Where(mat => mat))
                        {
                            var assetPath = AssetDatabase.GetAssetPath(item);
                            var importer  = AssetImporter.GetAtPath(assetPath) as AudioImporter;
                            if (importer == null || this.GetAllAudioClipInAllList().Contains(item)) continue;
                            var serializedObject = new SerializedObject(importer);
                            var normalize        = serializedObject.FindProperty("m_Normalize").boolValue;
                            var audioSetting     = importer.defaultSampleSettings;

                            var meshInfo = new AudioInfo
                            {
                                AudioPreview      = item,
                                Audio             = item,
                                Importer          = importer, // Storing the reference
                                ForceToMono       = importer.forceToMono,
                                Normalize         = normalize,
                                LoadInBackground  = importer.loadInBackground,
                                LoadType          = audioSetting.loadType,
                                PreloadAudioData  = audioSetting.preloadAudioData,
                                CompressionFormat = audioSetting.compressionFormat,
                                Quality           = audioSetting.quality,
                            };

                            if (this.IsWrongCompression(importer, item, normalize))
                            {
                                this.wrongCompressionAudioInfoList.Add(meshInfo);
                            }
                            else
                            {
                                if (this.IsLongSound(item))
                                {
                                    this.longCompressedAudioInfoList.Add(meshInfo);
                                }
                                else
                                {
                                    this.shortCompressedAudioInfoList.Add(meshInfo);
                                }
                            }
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private bool IsWrongCompression(AudioImporter audioImporter, AudioClip audioClip, bool normalized)
        {
            var isWrongCompression = false;
            var audioSetting       = audioImporter.defaultSampleSettings;
            var isLongSound        = this.IsLongSound(audioClip);

            isWrongCompression = !audioImporter.forceToMono ||
                                 audioSetting.loadType != (isLongSound ? AudioClipLoadType.CompressedInMemory : AudioClipLoadType.DecompressOnLoad) ||
                                 audioSetting.preloadAudioData != !isLongSound ||
                                 audioSetting.compressionFormat != AudioCompressionFormat.Vorbis ||
                                 Math.Abs(audioSetting.quality - this.audioCompressSetting.quality) > 0.0001f ||
                                 normalized;

            return isWrongCompression;
        }

        private bool IsLongSound(AudioClip item) => item.length >= this.audioCompressSetting.longAudioLength;

        private List<string> GetAllDependencies(string assetPath) { return new List<string>(AssetDatabase.GetDependencies(assetPath, true)); }

        #region Check Duplicate Audio

        [BoxGroup("---", CenterLabel = true, Order = 10)]
        [Button(ButtonSizes.Medium)]
        [GUIColor(1, 1, 0.5f)]
        public void CheckDuplicateAudios()
        {
            if (!this.GetAllAudioClipInAllList().Any()) this.FindAudiosInAddressables();

            this.DuplicateAudios.Clear();
            var audioClips = this.GetAllAudioClipInAllList().ToList();
            if (audioClips.Count <= 1) return;
            for (var i = 0; i < audioClips.Count - 1; i++)
            {
                for (var j = i + 1; j < audioClips.Count; j++)
                {
                    if (!this.AreAudioClipsEqual(audioClips[i], audioClips[j])) continue;

                    if (this.DuplicateAudios.All(x => !x.Contains(audioClips[i])))
                    {
                        this.DuplicateAudios.Add(new HashSet<AudioClip> { audioClips[i], audioClips[j] });
                    }
                    else
                    {
                        this.DuplicateAudios.First(x => x.Contains(audioClips[i])).Add(audioClips[j]);
                    }

                    Debug.Log($"AudioClips {i} and {j} have the same audio data.");
                }
            }

            if (!this.DuplicateAudios.Any()) this.ShowNotification(new GUIContent("No Duplicate Audios"));
        }

        private IEnumerable<AudioClip> GetAllAudioClipInAllList()
        {
            return this.wrongCompressionAudioInfoList.Concat(this.shortCompressedAudioInfoList).Concat(this.longCompressedAudioInfoList).Select(x => x.Audio);
        }

        [BoxGroup("---")] [ShowIf("@this.DuplicateAudios.Count > 0")]
        public List<HashSet<AudioClip>> DuplicateAudios = new();

        bool AreAudioClipsEqual(AudioClip clipA, AudioClip clipB)
        {
            if (clipA.samples != clipB.samples || clipA.channels != clipB.channels || clipA.frequency != clipB.frequency)
            {
                // AudioClips have different settings
                return false;
            }

            float[] dataA = new float[clipA.samples * clipA.channels];
            float[] dataB = new float[clipB.samples * clipB.channels];

            clipA.GetData(dataA, 0);
            clipB.GetData(dataB, 0);

            // Compare audio data sample by sample
            for (int i = 0; i < dataA.Length; i++)
            {
                if (dataA[i] != dataB[i])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        [Serializable]
        private class AudioCompressSetting
        {
            [Range(0, 1f)] public float quality         = 0.2f;
            public                int   longAudioLength = 60;
        }
    }

    [Serializable]
    [HideReferenceObjectPicker]
    public class AudioInfo
    {
        [HideLabel]
        [PreviewField(90, ObjectFieldAlignment.Left)]
        [HorizontalGroup("row2", 90), VerticalGroup("row2/left")]
        [ShowInInspector]
        public AudioClip AudioPreview { get; set; }

        [VerticalGroup("row2/right")]
        [ShowInInspector]
        public AudioClip Audio { get; set; }

        [VerticalGroup("row2/right")]
        [ShowInInspector]
        public AudioImporter Importer { get; set; }

        [VerticalGroup("row2/right")]
        [ShowInInspector]
        public bool ForceToMono { get; set; }

        [VerticalGroup("row2/right")]
        [ShowInInspector]
        public bool Normalize { get; set; }

        [VerticalGroup("row2/right")]
        [ShowInInspector]
        public bool LoadInBackground { get; set; }

        [VerticalGroup("row2/right")]
        [ShowInInspector]
        public AudioClipLoadType LoadType { get; set; }

        [VerticalGroup("row2/right")]
        [ShowInInspector]
        public bool PreloadAudioData { get; set; }

        [VerticalGroup("row2/right")]
        [ShowInInspector]
        public AudioCompressionFormat CompressionFormat { get; set; }

        [VerticalGroup("row2/right")]
        [ShowInInspector]
        [PropertyRange(0, 1)]
        public float Quality { get; set; }
    }
}