namespace UITemplate.Editor.TheOneWindowTools.ListAndOptimize
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    public class TextureFinderOdin : OdinEditorWindow
    {
        public enum SortingBy
        {
            FileSize,
            TotalPixel
        }

        public enum SortingOrder
        {
            Ascending,
            Descending
        }

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("Texture", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> textures = new();
        
        [BoxGroup("Setting", order: -1)]
        [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
        private void FindAllTextures() { this.FindTexturesInAssets(); }

        [EnumToggleButtons] [ShowInInspector] [OnValueChanged(nameof(Sort))] [BoxGroup("Setting")]
        private SortingBy sortingBy = SortingBy.TotalPixel;

        [EnumToggleButtons] [ShowInInspector] [OnValueChanged(nameof(Sort))] [BoxGroup("Setting")]
        private SortingOrder sortingOrder = SortingOrder.Descending;
                
        [BoxGroup("Setting")]
        [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
        private void Sort()
        {
            this.textures.Sort((a, b) =>
            {
                switch (this.sortingBy)
                {
                    case SortingBy.FileSize:
                        return this.sortingOrder == SortingOrder.Ascending
                            ? a.FileSize.CompareTo(b.FileSize)
                            : b.FileSize.CompareTo(a.FileSize);
                    case SortingBy.TotalPixel:
                        return this.sortingOrder == SortingOrder.Ascending
                            ? a.Texture.width * a.Texture.height - b.Texture.width * b.Texture.height
                            : b.Texture.width * b.Texture.height - a.Texture.width * a.Texture.height;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        [MenuItem("TheOne/List And Optimize/Texture List")]
        private static void OpenWindow() { GetWindow<TextureFinderOdin>().Show(); }

        private void FindTexturesInAssets()
        {
            this.textures.Clear();

            var findAssets = AssetDatabase.FindAssets("t:Texture");
            foreach (var texture in findAssets)
            {
                var path     = AssetDatabase.GUIDToAssetPath(texture);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;
                this.textures.Add(new TextureInfo
                {
                    Texture               = AssetDatabase.LoadAssetAtPath<Texture>(path),
                    TextureImporter       = importer,
                    FileSize              = new FileInfo(path).Length,
                    ReadWriteEnabled      = importer.isReadable,
                    GenerateMipMapEnabled = importer.mipmapEnabled,
                });
            }
        }
    }

    [Serializable]
    [HideReferenceObjectPicker]
    public class TextureInfo
    {
        [HideLabel]
        [PreviewField(90, ObjectFieldAlignment.Left)]
        [HorizontalGroup("group", 90), VerticalGroup("group/left")]
        [ShowInInspector]
        public Texture Texture { get; set; }

        [VerticalGroup("group/right")]
        [ShowInInspector]
        public string Name => this.Texture.name;

        [VerticalGroup("group/right")]
        [ShowInInspector]
        public long FileSize { get; set; }

        public TextureImporter TextureImporter { get; set; }

        [VerticalGroup("group/right")]
        [ShowInInspector]
        public bool ReadWriteEnabled { get; set; }

        [VerticalGroup("group/right")]
        [ShowInInspector]
        public bool GenerateMipMapEnabled { get; set; }

        [VerticalGroup("group/right")]
        [ShowInInspector]
        public int MaxTextureSize => this.TextureImporter.maxTextureSize;

        [VerticalGroup("group/right")]
        [ShowInInspector]
        public TextureImporterCompression CompressionType => this.TextureImporter.textureCompression;

        [VerticalGroup("group/right")]
        [ShowInInspector]
        public bool UseCrunchCompression => this.TextureImporter.crunchedCompression;
    }
}