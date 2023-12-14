namespace UITemplate.Editor.ShaderHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    public class FontFinderOdin : OdinEditorWindow
    {
        [ShowInInspector] [TableList] [Title("Compressed Font", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<FontInfo> compressedFonts = new();

        [ShowInInspector] [TableList] [Title("Non-Compressed Font", TitleAlignment = TitleAlignments.Centered)]
        private HashSet<FontInfo> noneCompressedFonts = new();

        [MenuItem("TheOne/FontFinder")]
        private static void OpenWindow() { GetWindow<FontFinderOdin>().Show(); }

        [ButtonGroup("Action")]
        [Button(ButtonSizes.Medium)]
        private void FindAllFonts() { this.FindFontsInAddressable(); }

        [ButtonGroup("Action")]
        [Button(ButtonSizes.Medium)]
        [GUIColor(0.3f, 0.8f, 0.3f)]
        private void CompressAllFont()
        {
            foreach (var noneCompressedFont in this.noneCompressedFonts)
            {
                noneCompressedFont.FontImporter.fontTextureCase  = FontTextureCase.CustomSet;
                noneCompressedFont.FontImporter.fontTextureCase  = FontTextureCase.CustomSet;
                noneCompressedFont.FontImporter.customCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz +-*/=\\|[]{}.,;\"':!@#$%^&()?";
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(noneCompressedFont.Font), ImportAssetOptions.ForceUpdate);
            }
        }

        private void FindFontsInAddressable()
        {
            this.compressedFonts.Clear();
            this.noneCompressedFonts.Clear();

            var fonts = AddressableSearcherTool.GetAllAssetInAddressable<Font>();
            foreach (var keyValuePair in fonts)
            {
                var font         = keyValuePair.Key;
                var path         = AssetDatabase.GetAssetPath(font);
                var fontImporter = AssetImporter.GetAtPath(path) as TrueTypeFontImporter;

                switch (fontImporter.fontTextureCase)
                {
                    case FontTextureCase.Dynamic:
                    case FontTextureCase.Unicode:
                    case FontTextureCase.ASCII:
                    case FontTextureCase.ASCIIUpperCase:
                    case FontTextureCase.ASCIILowerCase:
                        this.noneCompressedFonts.Add(new FontInfo
                        {
                            Font         = font,
                            FontImporter = fontImporter,
                            GameObjects  = keyValuePair.Value.ToList()
                        });
                        break;
                    case FontTextureCase.CustomSet:
                        this.compressedFonts.Add(new FontInfo
                        {
                            Font         = font,
                            FontImporter = fontImporter,
                            GameObjects  = keyValuePair.Value.ToList()
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public class FontInfo
    {
        public Font                 Font;
        public TrueTypeFontImporter FontImporter;
        public List<GameObject>     GameObjects;
        public string               CustomSet => this.FontImporter.customCharacters;
    }
}