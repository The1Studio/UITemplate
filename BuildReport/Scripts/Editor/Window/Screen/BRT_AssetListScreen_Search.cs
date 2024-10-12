using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using FuzzyString;

namespace BuildReportTool.Window.Screen
{
    public partial class AssetList
    {
        private SizePart[] _searchResults;

        private const double SEARCH_DELAY = 0.75f;
        private       double _lastSearchTime;
        private       string _lastSearchText = string.Empty;

        private string _searchTextInput = string.Empty;

        private int _searchViewOffset;

        private bool _showSearchOptions;
        private Rect _searchTextfieldRect;

        // Search algorithms that will weigh in for the comparison
        private readonly FuzzyStringComparisonOptions[] _searchOptions =
        {
            FuzzyStringComparisonOptions.UseOverlapCoefficient,
            FuzzyStringComparisonOptions.UseLongestCommonSubsequence,
            FuzzyStringComparisonOptions.UseLongestCommonSubstring,
        };

        private void ClearSearch()
        {
            this._searchTextInput = "";
            this._lastSearchText  = null;
            this._searchResults   = null;
        }

        private void UpdateSearch(double timeNow, BuildInfo buildReportToDisplay)
        {
            if (string.IsNullOrEmpty(this._searchTextInput) && !string.IsNullOrEmpty(this._lastSearchText))
            {
                // cancel search
                this.ClearSearch();
                if (buildReportToDisplay != null) buildReportToDisplay.FlagOkToRefresh();

                this._searchViewOffset = 0;
            }
            else if (timeNow - this._lastSearchTime >= SEARCH_DELAY && !this._searchTextInput.Equals(this._lastSearchText, StringComparison.Ordinal))
            {
                this.UpdateSearchNow(buildReportToDisplay);
                this._lastSearchTime = timeNow;
            }
        }

        public void UpdateSearchNow(BuildInfo buildReportToDisplay)
        {
            if (string.IsNullOrEmpty(this._searchTextInput)) return;

            // update search
            this._lastSearchText = this._searchTextInput;
            this._lastSearchTime = EditorApplication.timeSinceStartup;

            if (buildReportToDisplay != null)
            {
                this.Search(this._lastSearchText, BuildReportTool.Options.SearchType, BuildReportTool.Options.SearchFilenameOnly, BuildReportTool.Options.SearchCaseSensitive, buildReportToDisplay);
                buildReportToDisplay.FlagOkToRefresh();
            }

            this._searchViewOffset = 0;
            this._currentSortType  = BuildReportTool.AssetList.SortType.None;
        }

        private void Search(string searchText, SearchType searchType, bool searchFilenameOnly, bool caseSensitive, BuildInfo buildReportToDisplay)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                this._searchResults = null;
                return;
            }

            var list = this.GetAssetListToDisplay(buildReportToDisplay);

            var filter = buildReportToDisplay.FileFilters;

            if (BuildReportTool.Options.ShouldUseConfiguredFileFilters()) filter = this._configuredFileFilterGroup;

            var searchResults = new List<SizePart>();

            var assetListToSearchFrom = list.GetListToDisplay(filter);

            var options = caseSensitive ? System.Text.RegularExpressions.RegexOptions.None : System.Text.RegularExpressions.RegexOptions.IgnoreCase;

            for (var n = 0; n < assetListToSearchFrom.Length; ++n)
            {
                string input;
                if (searchFilenameOnly)
                    input = assetListToSearchFrom[n].Name.GetFileNameOnly();
                else
                    input = assetListToSearchFrom[n].Name;

                bool assetIsMatch;

                if (string.IsNullOrEmpty(input))
                    assetIsMatch = false;
                else
                    switch (searchType)
                    {
                        case SearchType.Regex:
                            try
                            {
                                assetIsMatch = System.Text.RegularExpressions.Regex.IsMatch(input, searchText, options);
                            }
                            catch (ArgumentException)
                            {
                                assetIsMatch = false;
                            }
                            break;
                        case SearchType.Fuzzy:
                            assetIsMatch = this.IsANearStringMatch(input, searchText);
                            break;
                        default:
                            // default is SearchType.Basic
                            assetIsMatch = System.Text.RegularExpressions.Regex.IsMatch(input, Util.WildCardToRegex(searchText), options);
                            break;
                    }

                if (assetIsMatch) searchResults.Add(assetListToSearchFrom[n]);
            }

            if (searchResults.Count > 0) searchResults.Sort((a, b) => this.GetFuzzyEqualityScore(searchText, a.Name).CompareTo(this.GetFuzzyEqualityScore(searchText, b.Name)));

            this._searchResults = searchResults.ToArray();
        }

        private void SortBySearchRank(SizePart[] assetList, string searchText)
        {
            if (assetList.Length <= 0) return;

            Array.Sort(assetList,
                (entry1, entry2) => this.GetFuzzyEqualityScore(searchText, entry1.Name)
                    .CompareTo(this.GetFuzzyEqualityScore(searchText, entry2.Name)));
        }

        private bool IsANearStringMatch(string source, string target)
        {
            if (string.IsNullOrEmpty(target)) return false;

            // Choose the relative strength of the comparison - is it almost exactly equal? or is it just close?
            const FuzzyStringComparisonTolerance TOLERANCE = FuzzyStringComparisonTolerance.Strong;

            // Get a boolean determination of approximate equality
            return source.ApproximatelyEquals(target, TOLERANCE, this._searchOptions);
        }

        private double GetFuzzyEqualityScore(string source, string target)
        {
            if (string.IsNullOrEmpty(target)) return 0;

            return source.GetFuzzyEqualityScore(target, this._searchOptions);
        }
    }
}