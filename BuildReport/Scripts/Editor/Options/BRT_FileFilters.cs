using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BuildReportTool
{
    [Serializable]
    public class FileFilters
    {
        public FileFilters(string label, string[] filters)
        {
            this._label = label;

            for (int n = 0, len = filters.Length; n < len; ++n)
            {
                this._filtersDict.Add(filters[n], false);
                var shouldBeAllLowerCase = true;

                if ((filters[n].StartsWith("/") || filters[n].StartsWith("Assets/", StringComparison.OrdinalIgnoreCase)) && filters[n].EndsWith("/"))
                {
                    this._usesFolderFilter = true;
                }
                else if (filters[n].StartsWith(BUILT_IN_ASSET_KEYWORD, StringComparison.OrdinalIgnoreCase))
                {
                    this._usesFolderFilter = true;

                    // note: filters for built-in types are case-sensitive
                    shouldBeAllLowerCase = false;

                    //Debug.Log("uses built-in: " + label + ", " + filters[n]);
                }
                else if (filters[n].StartsWith("\"") && filters[n].EndsWith("\""))
                {
                    this._usesExactFileMatching = true;
                }

                if (shouldBeAllLowerCase) filters[n] = filters[n].ToLower();
            }

            this._filtersList = filters;
        }

        public FileFilters()
        {
            this._label = "";
        }

        private const string BUILT_IN_ASSET_KEYWORD = "Built-in";

        [SerializeField] private string _label;

        private readonly Dictionary<string, bool> _filtersDict = new();

        [SerializeField] private string[] _filtersList;

        [SerializeField] private bool _usesFolderFilter;

        [SerializeField] private bool _usesExactFileMatching;

        public string Label { get => this._label; set => this._label = value; }

        public string[] FiltersList
        {
            get => this._filtersList;
            set
            {
                this._filtersList = value;

                for (int n = 0, len = this._filtersList.Length; n < len; ++n)
                {
                    this._filtersDict.Add(this._filtersList[n], false);
                    var shouldBeAllLowerCase = true;

                    if ((this._filtersList[n].StartsWith("/") || this._filtersList[n].StartsWith("Assets/", StringComparison.OrdinalIgnoreCase)) && this._filtersList[n].EndsWith("/"))
                    {
                        this._usesFolderFilter = true;
                    }
                    else if (this._filtersList[n].StartsWith(BUILT_IN_ASSET_KEYWORD, StringComparison.OrdinalIgnoreCase))
                    {
                        this._usesFolderFilter = true;
                        shouldBeAllLowerCase   = false;
                        //Debug.Log("uses built-in: " + _label + ", " + _filtersList[n]);
                    }
                    else if (this._filtersList[n].StartsWith("\"") && this._filtersList[n].EndsWith("\""))
                    {
                        this._usesExactFileMatching = true;
                    }

                    if (shouldBeAllLowerCase) this._filtersList[n] = this._filtersList[n].ToLower();
                }
            }
        }

        public string GetFileExt(string file)
        {
            var lastDotIdx = file.LastIndexOf(".", StringComparison.OrdinalIgnoreCase);
            if (lastDotIdx == -1) return "";
            return file.Substring(lastDotIdx, file.Length - lastDotIdx);
        }

        public bool IsFileInFilter(string file)
        {
            // -------------------------------------------------
            // try using folder filter method:

            if (this._usesFolderFilter)
                //Debug.Log(_label + " uses folder filter");
                for (int n = 0, len = this._filtersList.Length; n < len; ++n)
                {
                    // built-in asset compare is case-sensitive
                    if (this._filtersList[n].StartsWith(BUILT_IN_ASSET_KEYWORD, StringComparison.OrdinalIgnoreCase) && file.StartsWith(BUILT_IN_ASSET_KEYWORD, StringComparison.OrdinalIgnoreCase) && file.IndexOf(this._filtersList[n], StringComparison.OrdinalIgnoreCase) != -1) return true;

                    //Debug.Log(file + " ---- " + _filtersList[n]);
                    if ((this._filtersList[n].StartsWith("/") || this._filtersList[n].StartsWith("Assets/", StringComparison.OrdinalIgnoreCase)) && this._filtersList[n].EndsWith("/") && file.IndexOf(this._filtersList[n], StringComparison.OrdinalIgnoreCase) != -1) return true;
                }

            // -------------------------------------------------
            // if not found using folder filter method, try exact file matching next:

            if (this._usesExactFileMatching)
            {
                //Debug.Log("_usesExactFileMatching");

                var fileNameOnly = file.GetFileNameOnly();

                for (int n = 0, len = this._filtersList.Length; n < len; ++n)
                    //Debug.Log("in quotes: " + _filtersList[n] + " " + (_filtersList[n].StartsWith("\"") && _filtersList[n].EndsWith("\"")));
                {
                    if (this._filtersList[n].StartsWith("\"") && this._filtersList[n].EndsWith("\""))
                    {
                        var fileWithQuotes = string.Format("\"{0}\"", fileNameOnly);

                        //Debug.Log("match? " + _filtersList[n] + " == " + fileWithQuotes);
                        if (this._filtersList[n].Equals(fileWithQuotes)) return true;
                    }
                }
            }

            // -------------------------------------------------
            // if not found using exact file matching, try checking in dictionary next:

            var fileExtension = this.GetFileExt(file);

            if (this._filtersDict.ContainsKey(fileExtension)) return true;

            for (int n = 0, len = this._filtersList.Length; n < len; ++n)
                //Debug.Log("in quotes: " + _filtersList[n] + " " + (_filtersList[n].StartsWith("\"") && _filtersList[n].EndsWith("\"")));
                if (fileExtension.Equals(this._filtersList[n], StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }
    }

    [Serializable]
    [XmlRoot("FileFilterGroup")]
    public class FileFilterGroup
    {
        [SerializeField] private FileFilters[] _fileFilters;

        public FileFilters[] FileFilters
        {
            get => this._fileFilters;
            set
            {
                this._fileFilters = value;
                this.InitNames();
            }
        }

        [SerializeField] private string[] _names;

        public FileFilterGroup()
        {
            this._fileFilters = null;
            this._names       = null;
        }

        public FileFilterGroup(FileFilters[] filters)
        {
            this._fileFilters = filters;
            this.InitNames();
        }

        private void InitNames()
        {
            this._names = new string[this._fileFilters.Length + 2];

            this._names[0] = "All";

            for (int n = 0, len = this._fileFilters.Length; n < len; ++n) this._names[n + 1] = this._fileFilters[n].Label;

            this._names[this._names.Length - 1] = "Unknown";
        }

        private int _selectedFilterIdx;

        /// <summary>
        /// -1 means "All" list.
        /// </summary>
        public int SelectedFilterIdx => this._selectedFilterIdx - 1;

        public int GetSelectedFilterIdx()
        {
            return this._selectedFilterIdx;
        }

        public string GetSelectedFilterLabel()
        {
            return this._selectedFilterIdx >= 1 && this._selectedFilterIdx <= this._fileFilters.Length ? this._fileFilters[this._selectedFilterIdx - 1].Label : null;
        }

        public void ForceSetSelectedFilterIdx(int idx)
        {
            if (idx < this._fileFilters.Length + 2 && idx >= 0) this._selectedFilterIdx = idx;
        }

        private const string UNPRESSED_STYLE_NAME       = "ButtonNoContents";
        private const string ALREADY_PRESSED_STYLE_NAME = "ButtonAlreadyPressed";

        private const string HAS_CONTENTS_UNPRESSED_STYLE_NAME       = "ButtonHasContents";
        private const string HAS_CONTENTS_ALREADY_PRESSED_STYLE_NAME = "ButtonAlreadyPressed";

        private GUIStyle GetStyleToUse(int assetNum, int selectedIdx, int idxOfThisGroup)
        {
            string styleToUse;

            if (assetNum > 0)
            {
                styleToUse = HAS_CONTENTS_UNPRESSED_STYLE_NAME;
                if (selectedIdx == idxOfThisGroup) styleToUse = HAS_CONTENTS_ALREADY_PRESSED_STYLE_NAME;
            }
            else
            {
                styleToUse = UNPRESSED_STYLE_NAME;
                if (selectedIdx == idxOfThisGroup) styleToUse = ALREADY_PRESSED_STYLE_NAME;
            }

            var style = GUI.skin.FindStyle(styleToUse);
            if (style == null) return GUI.skin.button;
            return style;
        }

        public bool Draw(AssetList assetList, float width)
        {
            var displayType = Options.GetOptionFileFilterDisplay();
            switch (displayType)
            {
                case Options.FileFilterDisplay.DropDown: return this.DrawFiltersAsDropDown(assetList, width);
                case Options.FileFilterDisplay.Buttons:  return this.DrawFiltersAsButtons(assetList, width);
            }

            return false;
        }

        private bool DrawFiltersAsDropDown(AssetList assetList, float width)
        {
            var topBarLabelStyle                           = GUI.skin.FindStyle(Window.Settings.TOP_BAR_LABEL_STYLE_NAME);
            if (topBarLabelStyle == null) topBarLabelStyle = GUI.skin.label;

            var topBarPopupStyle                           = GUI.skin.FindStyle(Window.Settings.FILE_FILTER_POPUP_STYLE_NAME);
            if (topBarPopupStyle == null) topBarPopupStyle = GUI.skin.label;

            var changed = false;
            GUILayout.BeginHorizontal();
            GUILayout.Space(3);
            GUILayout.Label("Filter: ", topBarLabelStyle);
            if (assetList != null && assetList.Labels != null && assetList.Labels.Length > 0)
            {
                var newSelectedFilterIdx = EditorGUILayout.Popup(this._selectedFilterIdx,
                    assetList.Labels,
                    topBarPopupStyle);

                if (newSelectedFilterIdx != this._selectedFilterIdx)
                {
                    this._selectedFilterIdx = newSelectedFilterIdx;
                    assetList.SortIfNeeded(this);
                    changed = true;
                }
            }

            GUILayout.EndHorizontal();

            return changed;
        }

        private bool DrawFiltersAsButtons(AssetList assetList, float width)
        {
            var changed = false;
            GUILayout.BeginHorizontal();

            float overallWidth = 0;

            var styleToUse = this.GetStyleToUse(assetList.All.Length, this._selectedFilterIdx, 0);
            var label      = string.Format("All ({0})", assetList.All.Length.ToString());

            var widthToAdd = styleToUse.CalcSize(new(label)).x;

            overallWidth += widthToAdd;

            if (GUILayout.Button(label, styleToUse))
            {
                this._selectedFilterIdx = 0;
                assetList.SortIfNeeded(this);
                changed = true;
            }

            if (overallWidth >= width)
            {
                overallWidth = 0;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }

            if (assetList.PerCategory != null && assetList.PerCategory.Length >= this._fileFilters.Length)
            {
                for (int n = 0, len = this._fileFilters.Length; n < len; ++n)
                {
                    styleToUse = this.GetStyleToUse(assetList.PerCategory[n].Length, this._selectedFilterIdx, n + 1);
                    label      = string.Format("{0} ({1})", this._fileFilters[n].Label, assetList.PerCategory[n].Length.ToString());

                    widthToAdd = styleToUse.CalcSize(new(label)).x;

                    if (overallWidth + widthToAdd >= width)
                    {
                        overallWidth = 0;
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }

                    overallWidth += widthToAdd;

                    if (GUILayout.Button(label, styleToUse))
                    {
                        this._selectedFilterIdx = n + 1;
                        assetList.SortIfNeeded(this);
                        changed = true;
                    }
                }

                styleToUse = this.GetStyleToUse(assetList.PerCategory[assetList.PerCategory.Length - 1].Length,
                    this._selectedFilterIdx,
                    assetList.PerCategory.Length);

                label = string.Format("Unknown ({0})",
                    assetList.PerCategory[assetList.PerCategory.Length - 1].Length.ToString());
                widthToAdd = styleToUse.CalcSize(new(label)).x;
                if (overallWidth + widthToAdd >= width)
                {
                    //overallWidth = 0;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }

                if (GUILayout.Button(label, styleToUse))
                {
                    this._selectedFilterIdx = assetList.PerCategory.Length;
                    assetList.SortIfNeeded(this);
                    changed = true;
                }
            }

            GUILayout.EndHorizontal();
            return changed;
        }

        public FileFilters this[int idx] => this._fileFilters[idx];

        public int Count => this._fileFilters.Length;

        public override string ToString()
        {
            var ret = "(" + this._names.Length.ToString() + ") ";

            for (int n = 0, len = this._names.Length; n < len; ++n) ret += this._names[n] + ", ";

            return ret;
        }
    }
} // namespace BuildReportTool