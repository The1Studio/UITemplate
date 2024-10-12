using System;
using System.Collections;
using System.Collections.Generic;
using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView;
using UnityEngine;
using UnityEditor;
using System.Linq;
using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl;
using System.IO;

namespace HeurekaGames.AssetHunterPRO
{
    [Serializable]
    public class AH_TreeViewSelectionInfo
    {
        public delegate void AssetDeletedHandler();

        public static event AssetDeletedHandler OnAssetDeleted;

        private bool hasSelection;
        public  bool HasSelection => this.hasSelection;

        public const float Height = 64;

        private AH_MultiColumnHeader     multiColumnHeader;
        private List<AH_TreeviewElement> selection;

        internal void Reset()
        {
            this.selection    = null;
            this.hasSelection = false;
        }

        internal void SetSelection(AH_TreeViewWithTreeModel treeview, IList<int> selectedIds)
        {
            this.multiColumnHeader = (AH_MultiColumnHeader)treeview.multiColumnHeader;
            this.selection         = new();

            foreach (var itemID in selectedIds) this.selection.Add(treeview.treeModel.Find(itemID));

            this.hasSelection = this.selection.Count > 0;

            //If we have more, select the assets in project view
            if (this.hasSelection)
            {
                if (this.selection.Count > 1)
                {
                    var selectedObjects                                               = new UnityEngine.Object[this.selection.Count];
                    for (var i = 0; i < this.selection.Count; i++) selectedObjects[i] = AssetDatabase.LoadMainAssetAtPath(this.selection[i].RelativePath);
                    Selection.objects = selectedObjects;
                }
                else
                {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(this.selection[0].RelativePath);
                }

                AH_Utils.PingObjectAtPath(this.selection[this.selection.Count - 1].RelativePath, false);
            }
        }

        internal void OnGUISelectionInfo(Rect selectionRect)
        {
            GUILayout.BeginArea(selectionRect);
            //TODO MAKE SURE WE DONT DO ALL OF THIS EACH FRAME, BUT CACHE THE SELECTION DATA

            using (new EditorGUILayout.HorizontalScope())
            {
                if (this.selection.Count == 1)
                    this.drawSingle();
                else
                    this.drawMulti();
            }
            GUILayout.EndArea();
        }

        private void drawSingle()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            this.drawAssetPreview(true);
            EditorGUILayout.EndVertical();

            //Draw info from single asset
            EditorGUILayout.BeginVertical();

            GUILayout.Label(this.selection[0].RelativePath);
            if (!this.selection[0].IsFolder) GUILayout.Label("(" + this.selection[0].AssetType + ")");

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            if (this.selection[0].IsFolder)
                this.DrawDeleteFolderButton(this.selection[0]);
            else
                this.drawDeleteAssetsButton();

            EditorGUILayout.EndHorizontal();
        }

        private void drawMulti()
        {
            //Make sure we have not selected folders
            var allFolders  = this.selection.All(val => val.IsFolder);
            var allFiles    = !this.selection.Any(val => val.IsFolder);
            var allSameType = this.selection.All(var => var.AssetType == this.selection[0].AssetType);

            var containsNested = false;
            foreach (var item in this.selection)
            {
                if (!item.IsFolder) continue;

                foreach (var other in this.selection)
                {
                    if (other == item) continue;

                    if (!other.RelativePath.StartsWith(item.RelativePath)) continue;

                    var dirInfo = new DirectoryInfo(item.RelativePath);

                    if (other.IsFolder)
                    {
                        var otherDir = new DirectoryInfo(other.RelativePath);

                        if (!dirInfo.GetDirectories(otherDir.Name, SearchOption.AllDirectories).Any(x => x.FullName == otherDir.FullName)) continue;

                        /*if (dirInfo.Parent.FullName == otherDir.Parent.FullName)
                            continue;*/
                    }
                    else
                    {
                        var fi = new FileInfo(other.RelativePath);

                        if (!dirInfo.GetFiles(fi.Name, SearchOption.AllDirectories).Any(x => x.FullName == fi.FullName)) continue;
                    }

                    containsNested = true;
                }

                if (containsNested) break;
            }

            this.drawAssetPreview(allSameType);

            EditorGUILayout.BeginHorizontal();
            //Draw info from multiple
            EditorGUILayout.BeginVertical();

            //Identical files
            if (allSameType && allFiles)
                GUILayout.Label(this.selection[0].AssetType.ToString() + " (" + this.selection.Count() + ")");
            //all folders
            else if (allSameType)
                GUILayout.Label("Folders (" + this.selection.Count() + ")");
            //Non identical selection
            else
                GUILayout.Label("Items (" + this.selection.Count() + ")");

            EditorGUILayout.EndVertical();

            if (!containsNested)
            {
                this.drawDeleteAssetsButton();
            }
            else
            {
                GUILayout.FlexibleSpace();
                var s = new GUIStyle(EditorStyles.textField);
                s.normal.textColor = Color.red;
                EditorGUILayout.LabelField("Nested selection is not allowed", s);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void drawDeleteAssetsButton()
        {
            if (this.multiColumnHeader.ShowMode != AH_MultiColumnHeader.AssetShowMode.Unused) return;

            long combinedSize = 0;
            foreach (var item in this.selection)
            {
                if (item.IsFolder)
                    combinedSize += item.GetFileSizeRecursively(AH_MultiColumnHeader.AssetShowMode.Unused);
                else
                    combinedSize += item.FileSize;
            }
            if (GUILayout.Button("Delete " + AH_Utils.GetSizeAsString(combinedSize), GUILayout.Width(160), GUILayout.Height(32))) this.deleteUnusedAssets();
        }

        private void DrawDeleteFolderButton(AH_TreeviewElement folder)
        {
            if (this.multiColumnHeader.ShowMode != AH_MultiColumnHeader.AssetShowMode.Unused) return;

            var description = "Delete unused assets from folder";
            var content     = new GUIContent("Delete " + AH_Utils.GetSizeAsString(folder.GetFileSizeRecursively(AH_MultiColumnHeader.AssetShowMode.Unused)), description);
            var style       = new GUIStyle(GUI.skin.button);
            this.DrawDeleteFolderButton(content, folder, style, description, "Do you want to delete all unused assets from:" + Environment.NewLine + folder.RelativePath, GUILayout.Width(160), GUILayout.Height(32));
        }

        public void DrawDeleteFolderButton(GUIContent content, AH_TreeviewElement folder, GUIStyle style, string dialogHeader, string dialogDescription, params GUILayoutOption[] layout)
        {
            if (GUILayout.Button(content, style, layout)) this.deleteUnusedFromFolder(dialogHeader, dialogDescription, folder);
        }

        private void drawAssetPreview(bool bDraw)
        {
            var content = new GUIContent();

            //Draw asset preview
            if (bDraw && !this.selection[0].IsFolder)
            {
                var preview = AssetPreview.GetAssetPreview(AssetDatabase.LoadMainAssetAtPath(this.selection[0].RelativePath));
                content = new(preview);
            }
            //Draw Folder icon
            else if (bDraw)
            {
                content = EditorGUIUtility.IconContent("Folder Icon");
            }

            GUILayout.Label(content, GUILayout.Width(Height), GUILayout.Height(Height));
        }

        private void deleteUnusedAssets()
        {
            var choice         = EditorUtility.DisplayDialogComplex("Delete unused assets", "Do you want to delete the selected assets", "Yes", "Cancel", "Backup (Very slow)");
            var affectedAssets = new List<string>();

            if (choice == 0) //Delete
            {
                foreach (var item in this.selection)
                {
                    if (item.IsFolder)
                        affectedAssets.AddRange(item.GetUnusedPathsRecursively());
                    else
                        affectedAssets.Add(item.RelativePath);
                }
                this.deleteMultipleAssets(affectedAssets);
            }
            else if (choice == 2) //Backup
            {
                foreach (var item in this.selection)
                {
                    if (item.IsFolder)
                        affectedAssets.AddRange(item.GetUnusedPathsRecursively());
                    else
                        affectedAssets.Add(item.RelativePath);
                }
                this.exportAssetsToPackage("Backup as unitypackage", affectedAssets);
            }
        }

        private void deleteUnusedFromFolder(string header, string description, AH_TreeviewElement folder)
        {
            var choice = EditorUtility.DisplayDialogComplex(header, description, "Yes", "Cancel", "Backup (Very slow)");

            var affectedAssets = new List<string>();
            if (choice != 1) //Not Cancel
                //Collect affected assets
                affectedAssets = folder.GetUnusedPathsRecursively();
            if (choice == 0) //Delete
                this.deleteMultipleAssets(affectedAssets);
            else if (choice == 2) //Backup
                this.exportAssetsToPackage("Backup as unitypackage", affectedAssets);
        }

        private void exportAssetsToPackage(string header, List<string> affectedAssets)
        {
            var filename = Environment.UserName + "_Backup_" + "_" + AH_SerializationHelper.GetDateString();
            var savePath = EditorUtility.SaveFilePanel(
                header,
                AH_SerializationHelper.GetBackupFolder(),
                filename,
                "unitypackage");

            if (!string.IsNullOrEmpty(savePath))
            {
                EditorUtility.DisplayProgressBar("Backup", "Creating backup of " + affectedAssets.Count() + " assets", 0f);
                AssetDatabase.ExportPackage(affectedAssets.ToArray<string>(), savePath, ExportPackageOptions.Recurse);
                EditorUtility.ClearProgressBar();
                EditorUtility.RevealInFinder(savePath);

                this.deleteMultipleAssets(affectedAssets);
            }
        }

        private void deleteMultipleAssets(List<string> affectedAssets)
        {
            #if UNITY_2020_1_OR_NEWER
            EditorUtility.DisplayProgressBar("Deleting unused assets", $"Deleting {affectedAssets.Count()} unused assets", .5f);
            var failedPaths = new List<string>();
            AssetDatabase.DeleteAssets(affectedAssets.ToArray(), failedPaths);
            EditorUtility.ClearProgressBar();
            #else
            foreach (var asset in affectedAssets)
            {
                EditorUtility.DisplayProgressBar("Deleting unused assets", $"Deleting {affectedAssets.IndexOf(asset)}:{affectedAssets.Count()}", affectedAssets.IndexOf(asset)/ affectedAssets.Count());
                //AssetDatabase.DeleteAsset(asset);
                FileUtil.DeleteFileOrDirectory(asset);
            }
            EditorUtility.ClearProgressBar();
            #endif

            AssetDatabase.Refresh();

            if (OnAssetDeleted != null) OnAssetDeleted();
        }
    }
}