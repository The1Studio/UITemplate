namespace UITemplate.Editor.TheOneWindowTools.ListAndOptimize
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.Utilities.Extension;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Profiling;
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

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("3D Model Texture", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> modelTextures = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("All Texture (Not In Models)", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> allTextures = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("Mipmap", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> generatedMipMap = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("No In Atlas", TitleAlignment = TitleAlignments.Centered)]
        private List<TextureInfo> notInAtlasTexture = new();

        [ShowInInspector] [TableList(ShowPaging = true)] [Title("Not Compressed and Not In Atlas", TitleAlignment = TitleAlignments.Centered)]
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
                    SortingBy.TotalPixel => this.sortingOrder == SortingOrder.Ascending
                        ? a.Texture.width * a.Texture.height - b.Texture.width * b.Texture.height
                        : b.Texture.width * b.Texture.height - a.Texture.width * a.Texture.height,
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
        }

        [MenuItem("TheOne/List And Optimize/Texture List")]
        private static void OpenWindow() { GetWindow<TextureFinderOdin>().Show(); }

        private void FindTexturesInAssets()
        {
            this.modelTextures.Clear();
            this.allTextures.Clear();
            this.generatedMipMap.Clear();
            this.notInAtlasTexture.Clear();
            this.notCompressedAndNotInAtlasTexture.Clear();

            var textures             = AssetSearcher.GetAllAssetInAddressable<Texture>().Keys.ToList();
            var atlases              = AssetSearcher.GetAllAssetsOfType<SpriteAtlas>();
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

            var textureInfos = textures.Select(texture =>
            {
                var path     = AssetDatabase.GetAssetPath(texture);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) return null;
                return new TextureInfo
                {
                    Texture               = texture,
                    TextureImporter       = importer,
                    FileSize              = Profiler.GetRuntimeMemorySizeLong(texture) / 1024,
                    TextureSize           = this.GetTextureSizeAccordingToMaxSize(texture, importer),
                    ReadWriteEnabled      = importer.isReadable,
                    GenerateMipMapEnabled = importer.mipmapEnabled,
                };
            }).Where(textureInfo => textureInfo != null).ToList();
            this.Sort(textureInfos);

            foreach (var textureInfo in textureInfos)
            {
                if (modelTextureSet.Contains(textureInfo.Texture))
                {
                    this.modelTextures.Add(textureInfo);
                    continue;
                }

                this.allTextures.Add(textureInfo);
                if (textureInfo.GenerateMipMapEnabled)
                {
                    this.generatedMipMap.Add(textureInfo);
                }

                if (!this.IsTextureInAnyAtlas(textureInfo.Texture, atlases))
                {
                    this.notInAtlasTexture.Add(textureInfo);
                }

                if (textureInfo.CompressionType == TextureImporterCompression.Uncompressed && !this.IsTextureInAnyAtlas(textureInfo.Texture, atlases))
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

        private bool IsTextureInAnyAtlas(Texture texture, List<SpriteAtlas> allAtlases)
        {
            foreach (var atlas in allAtlases)
            {
                Sprite[] sprites = new Sprite[atlas.spriteCount];
                atlas.GetSprites(sprites);

                foreach (var sprite in sprites)
                {
                    var spriteNameWithoutClone = sprite.name.Replace("(Clone)", "").Trim();
                    if (spriteNameWithoutClone.Equals(texture.name, StringComparison.OrdinalIgnoreCase))
                    {
                        if (Mathf.Approximately(sprite.rect.width, texture.width) && Mathf.Approximately(sprite.rect.height, texture.height))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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
    }
}