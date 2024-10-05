namespace TheOne.Tool.Optimization.Texture
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using TheOne.Tool.Core;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.U2D;

    public class TextureFinderOdin : OdinEditorWindow
    {
        private enum SortingBy
        {
            FileSize,
            TotalPixel
        }

        private enum SortingOrder
        {
            Ascending,
            Descending
        }

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("All Textures", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> allTextures = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("Mipmap", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> generatedMipMap = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("Compressed and Not Crunch Textures", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> compressedAndNotCrunchTexture = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("3D Model Texture", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> modelTextures = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("All Texture (Not In Models)", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> allTexturesNotInModels = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("Not In Atlas", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> notInAtlasTexture = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("Duplicated Atlas Textures", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> duplicatedAtlasTexture = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("Dont Use Atlas Textures", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> dontUseAtlasTexture = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("Not Compressed and Not In Atlas Textures", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> notCompressedAndNotInAtlasTexture = new();

        [BoxGroup("Setting", order: -1)]
        [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
        private void FindAllTextures() { this.FindTexturesInAssets(); }

        [EnumToggleButtons] [ShowInInspector] [OnValueChanged(nameof(FindTexturesInAssets))] [BoxGroup("Setting")]
        private SortingBy sortingBy = SortingBy.TotalPixel;

        [EnumToggleButtons] [ShowInInspector] [OnValueChanged(nameof(FindTexturesInAssets))] [BoxGroup("Setting")]
        private SortingOrder sortingOrder = SortingOrder.Descending;

        private void Sort(List<TextureInfo> textureInfos)
        {
            textureInfos.Sort((a, b) =>
            {
                return this.sortingBy switch
                {
                    SortingBy.FileSize => this.sortingOrder == SortingOrder.Ascending ? a.FileSize.CompareTo(b.FileSize) : b.FileSize.CompareTo(a.FileSize),
                    SortingBy.TotalPixel => (int)(this.sortingOrder == SortingOrder.Ascending
                        ? a.TextureSize.x * a.TextureSize.y - b.TextureSize.x * b.TextureSize.y
                        : b.TextureSize.x * b.TextureSize.y - a.TextureSize.x * a.TextureSize.y),
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
        }

        [MenuItem("TheOne/List And Optimize/Texture List")]
        private static void OpenWindow() { GetWindow<TextureFinderOdin>().Show(); }

        [BoxGroup("Generate"), Button(ButtonSizes.Large), GUIColor(0.7f, 0.7f, 1)]
        private void CreateTextureInfoDataAssetButton() { this.GenerateTextureInfoDataAsset(); }

        #region Action

        [BoxGroup("Actions"), Button(ButtonSizes.Large), GUIColor(0.5f, 0.8f, 0.5f)]
        private void SelectAllModelTextures()
        {
            // Select the textures in Unity Editor
            Selection.objects = this.modelTextures.Select(info => info.Texture).ToArray();
        }

        [BoxGroup("Actions"), Button(ButtonSizes.Large), GUIColor(0.5f, 0.8f, 0.5f)]
        private void SelectAllGeneratedMipMapTextures()
        {
            // Select the textures in Unity Editor
            Selection.objects = this.generatedMipMap.Select(info => info.Texture).ToArray();
        }

        [BoxGroup("Actions"), Button(ButtonSizes.Large), GUIColor(0.5f, 0.8f, 0.5f)]
        private void SelectAllCompressedAndNotCrunchedTextures()
        {
            // Select the textures in Unity Editor
            Selection.objects = this.compressedAndNotCrunchTexture.Select(info => info.Texture).ToArray();
        }

        [BoxGroup("Actions"), Button(ButtonSizes.Large), GUIColor(0.5f, 0.8f, 0.5f)]
        private void SelectAllDontUseAtlasTextures()
        {
            // Select the textures in Unity Editor
            Selection.objects = this.dontUseAtlasTexture.Select(info => info.Texture).ToArray();
        }

        [BoxGroup("Actions"), Button(ButtonSizes.Large), GUIColor(0.5f, 0.8f, 0.5f)]
        private void SelectAllTextures()
        {
            // Select the textures in Unity Editor
            Selection.objects = this.allTextures.Select(info => info.Texture).ToArray();
        }

        #endregion

        private void GenerateTextureInfoDataAsset()
        {
            var             path            = "Assets/OptimizationData/TextureInfoData.asset";
            TextureInfoData textureInfoData = AssetDatabase.LoadAssetAtPath<TextureInfoData>(path);

            if (textureInfoData == null)
            {
                textureInfoData = CreateInstance<TextureInfoData>();
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }

                AssetDatabase.CreateAsset(textureInfoData, path);
            }

            textureInfoData.textureInfos = this.GetTextureInfos(AssetSearcher.GetAllAssetInAddressable<Texture>().Keys.ToList());
            EditorUtility.SetDirty(textureInfoData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = textureInfoData;
            Debug.Log("TextureInfoData ScriptableObject " + (textureInfoData ? "updated" : "created") + " at " + path);
        }

        private List<TextureInfo> GetTextureInfos(List<Texture> textures)
        {
            var textureInfos = textures.Select(texture =>
            {
                var path     = AssetDatabase.GetAssetPath(texture);
                var fileInfo = new FileInfo(path);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) return null;
                return new TextureInfo
                {
                    Texture               = texture,
                    TextureImporter       = importer,
                    FileSize              = fileInfo.Exists ? fileInfo.Length : 0, // File size in kilobytes
                    TextureSize           = this.GetTextureSizeAccordingToMaxSize(texture, importer),
                    ReadWriteEnabled      = importer.isReadable,
                    GenerateMipMapEnabled = importer.mipmapEnabled,
                    Path                  = path
                };
            }).Where(textureInfo => textureInfo != null).ToList();
            this.Sort(textureInfos);

            return textureInfos;
        }

        private void FindTexturesInAssets()
        {
            this.allTextures.Clear();
            this.generatedMipMap.Clear();
            this.modelTextures.Clear();
            this.allTexturesNotInModels.Clear();
            this.notInAtlasTexture.Clear();
            this.duplicatedAtlasTexture.Clear();
            this.dontUseAtlasTexture.Clear();
            this.compressedAndNotCrunchTexture.Clear();
            this.notCompressedAndNotInAtlasTexture.Clear();

            var allAddressableTextures = AssetSearcher.GetAllAssetInAddressable<Texture>().Keys.ToList();
            var textureInfos           = this.GetTextureInfos(allAddressableTextures);
            var allAddressableTextureSet = allAddressableTextures.ToHashSet();

            //Model 3D
            var meshRenderers        = AssetSearcher.GetAllAssetInAddressable<MeshRenderer>().Keys.ToList();
            var skinnedMeshRenderers = AssetSearcher.GetAllAssetInAddressable<SkinnedMeshRenderer>().Keys.ToList();
            var modelTextureSet      = new HashSet<Texture>();
            foreach (var meshRenderer in meshRenderers)
            {
                modelTextureSet.AddRange(AssetSearcher.GetAllDependencies<Texture>(meshRenderer));
            }

            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                modelTextureSet.AddRange(AssetSearcher.GetAllDependencies<Texture>(skinnedMeshRenderer));
            }

            //Atlas
            var atlases                   = AssetSearcher.GetAllAssetsOfType<SpriteAtlas>();
            var atlasTextureSet           = new HashSet<Texture>();
            var duplicatedAtlasTextureSet = new HashSet<Texture>();
            foreach (var spriteAtlas in atlases)
            {
                var textureInAtlases = AssetSearcher.GetAllDependencies<Texture>(spriteAtlas);
                foreach (var textureInAtlas in textureInAtlases)
                {
                    if (!atlasTextureSet.Add(textureInAtlas))
                    {
                        duplicatedAtlasTextureSet.Add(textureInAtlas);
                    }
                }
            }

            var dontUseAtlasTextureSet = new HashSet<Texture>(atlasTextureSet);
            dontUseAtlasTextureSet.RemoveRange(allAddressableTextureSet);
            this.dontUseAtlasTexture = this.GetTextureInfos(dontUseAtlasTextureSet.ToList());
            this.dontUseAtlasTexture = this.dontUseAtlasTexture.Where(info => !info.Path.Contains("UITemplate")).ToList(); //Temporarily disable until moving all feature to other packages

            foreach (var textureInfo in textureInfos)
            {
                this.allTextures.Add(textureInfo);
                if (textureInfo.GenerateMipMapEnabled)
                {
                    this.generatedMipMap.Add(textureInfo);
                }

                if (textureInfo.TextureImporter.textureCompression != TextureImporterCompression.Uncompressed && !textureInfo.TextureImporter.crunchedCompression)
                {
                    this.compressedAndNotCrunchTexture.Add(textureInfo);
                }

                if (modelTextureSet.Contains(textureInfo.Texture))
                {
                    this.modelTextures.Add(textureInfo);
                    continue;
                }

                this.allTexturesNotInModels.Add(textureInfo);

                if (!atlasTextureSet.Contains(textureInfo.Texture) && textureInfo.TextureImporter.textureType == TextureImporterType.Sprite)
                {
                    this.notInAtlasTexture.Add(textureInfo);
                }

                if (duplicatedAtlasTextureSet.Contains(textureInfo.Texture))
                {
                    this.duplicatedAtlasTexture.Add(textureInfo);
                }

                if (textureInfo.CompressionType == TextureImporterCompression.Uncompressed && !atlasTextureSet.Contains(textureInfo.Texture))
                {
                    this.notCompressedAndNotInAtlasTexture.Add(textureInfo);
                }
            }
        }

        private Vector2 GetTextureSizeAccordingToMaxSize(Texture texture, TextureImporter importer)
        {
            if (importer != null)
            {
                var maxSize     = importer.maxTextureSize;
                var aspectRatio = (float)texture.width / texture.height;

                if (texture.width > maxSize || texture.height > maxSize)
                {
                    if (texture.width > texture.height)
                    {
                        return new Vector2(maxSize, maxSize / aspectRatio);
                    }
                    else
                    {
                        return new Vector2(maxSize * aspectRatio, maxSize);
                    }
                }
            }

            return new Vector2(texture.width, texture.height);
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
        [LabelText("File Size (Byte)")]
        public long FileSize { get; set; }

        [VerticalGroup("group/right")]
        [ShowInInspector]
        [LabelText("TinyPNG Compressed File Size (Byte)")]
        public long TinyPNGCompressedFileSize { get; set; }

        [VerticalGroup("group/right")]
        [ShowInInspector]
        public Vector2 TextureSize { get; set; }

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

        public string Path { get; set; }
    }
}