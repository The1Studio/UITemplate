using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HeurekaGames.Utils
{
    public class Heureka_PackageDataVersioned : Heureka_PackageData
    {
        public   List<PackageVersion> VersionData = new();
        internal bool                 FoldOutVersionHistory;

        private void Item_OnChanged()
        {
            EditorUtility.SetDirty(this);
        }

        public void AddNewVersion(int major, int minor, int patch)
        {
            var newPackageVersion = new PackageVersion(major, minor, patch);
            this.VersionData.Add(newPackageVersion);
        }

        public void CollapseAll()
        {
            foreach (var item in this.VersionData) item.FoldOut = false;
        }

        public void Delete(PackageVersion target)
        {
            this.VersionData.Remove(target);
        }
    }

    [Serializable]
    public class PackageVersion
    {
        [SerializeField] public PackageVersionNum VersionNum     = new();
        [SerializeField] public List<string>      VersionChanges = new();

        internal bool              FoldOut       = false;
        private  PackageVersionNum newVersionNum = new();

        private const float           btnWidth = 150;
        private       ReorderableList reorderableList;

        private bool initialized = false;

        public PackageVersion(int major, int minor, int patch)
        {
            this.VersionNum = this.newVersionNum = new(major, minor, patch);
            this.FoldOut    = true;
        }

        private void initialize()
        {
            this.initialized     = true;
            this.reorderableList = new(this.VersionChanges, typeof(string), true, true, true, true);

            this.reorderableList.drawHeaderCallback  += this.DrawHeader;
            this.reorderableList.drawElementCallback += this.DrawElement;

            this.reorderableList.onAddCallback    += this.AddItem;
            this.reorderableList.onRemoveCallback += this.RemoveItem;
        }

        /// <summary>
        /// Draws the header of the list
        /// </summary>
        /// <param name="rect"></param>
        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Version changes");
        }

        /// <summary>
        /// Draws one element of the list (ListItemExample)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="active"></param>
        /// <param name="focused"></param>
        private void DrawElement(Rect rect, int index, bool active, bool focused)
        {
            this.VersionChanges[index] = EditorGUI.TextField(new(rect.x + 18, rect.y, rect.width - 18, rect.height), this.VersionChanges[index]);
        }

        private void AddItem(ReorderableList list)
        {
            this.VersionChanges.Add("");
        }

        private void RemoveItem(ReorderableList list)
        {
            this.VersionChanges.RemoveAt(list.index);
        }

        public void OnGUI(ref bool shouldDelete)
        {
            if (!this.initialized || this.reorderableList == null) this.initialize();

            GUILayout.Space(10);
            this.FoldOut = EditorGUILayout.Foldout(this.FoldOut, this.VersionNum.GetVersionString());

            if (GUILayout.Button("Delete Version", GUILayout.Width(btnWidth))) shouldDelete = true;

            if (this.FoldOut)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Update Version", GUILayout.Width(btnWidth))) this.updateVersion();

                //Allow for changing version num
                this.newVersionNum.Major = EditorGUILayout.IntField(this.newVersionNum.Major);
                this.newVersionNum.Minor = EditorGUILayout.IntField(this.newVersionNum.Minor);
                this.newVersionNum.Patch = EditorGUILayout.IntField(this.newVersionNum.Patch);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                //versionDescription = GUILayout.TextArea(versionDescription);
                if (this.reorderableList.count > 0 && GUILayout.Button("Copy to clipboard"))
                {
                    var clipboardString                                             = "";
                    foreach (var item in this.reorderableList.list) clipboardString += item.ToString() + Environment.NewLine;
                    EditorGUIUtility.systemCopyBuffer = clipboardString;
                }

                EditorGUILayout.EndHorizontal();
                {
                    this.reorderableList.DoLayoutList();
                }
            }
        }

        private void updateVersion()
        {
            this.VersionNum = new(this.newVersionNum.Major, this.newVersionNum.Minor, this.newVersionNum.Patch);
            //TODO SORT Parent list
        }
    }

    [Serializable]
    public struct PackageVersionNum : IComparable<PackageVersionNum>
    {
        [SerializeField] public int Major;
        [SerializeField] public int Minor;
        [SerializeField] public int Patch;

        public PackageVersionNum(int major, int minor, int path)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = path;
        }

        public int CompareTo(PackageVersionNum other)
        {
            if (this.Major != other.Major)
                return this.Major.CompareTo(other.Major);
            else if (this.Minor != other.Minor)
                return this.Minor.CompareTo(other.Minor);
            else
                return this.Patch.CompareTo(other.Patch);
        }

        public class VersionComparer : IComparer<PackageVersionNum>
        {
            int IComparer<PackageVersionNum>.Compare(PackageVersionNum objA, PackageVersionNum objB)
            {
                return objA.CompareTo(objB);
            }
        }

        public string GetVersionString()
        {
            return string.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Patch);
        }

        public bool IsEmpty()
        {
            return this.Major == 0 && this.Minor == 0 && this.Patch == 0;
        }
    }
}