using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    [Serializable]
    public class AH_DuplicateDataManager : ScriptableSingleton<AH_DuplicateDataManager>, ISerializationCallbackReceiver
    {
        [Serializable]
        public class DuplicateAssetData
        {
            public  List<string> Guids;
            private List<string> paths;
            private Texture2D    preview;

            public Texture2D Preview
            {
                get
                {
                    if (this.preview != null)
                    {
                        return this.preview;
                    }
                    else
                    {
                        var loadedObj = AssetDatabase.LoadMainAssetAtPath(this.Paths[0]);
                        return this.preview = AssetPreview.GetAssetPreview(loadedObj);
                    }
                }
            }

            public List<string> Paths
            {
                get
                {
                    if (this.paths != null)
                        return this.paths;
                    else
                        return this.paths = this.Guids.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
                }
            }

            public DuplicateAssetData(List<string> guids)
            {
                this.Guids = guids;
            }
        }

        [SerializeField] public bool IsDirty = true;

        public bool RequiresScrollviewRebuild { get; internal set; }
        public bool HasCache                  { get; private set; }

        [SerializeField] private Dictionary<string, DuplicateAssetData> duplicateDict = new();

        #region serializationHelpers

        [SerializeField] private List<string>             _duplicateDictKeys   = new();
        [SerializeField] private List<DuplicateAssetData> _duplicateDictValues = new();

        public Dictionary<string, DuplicateAssetData> Entries => this.duplicateDict;

        #endregion

        internal bool HasDuplicates()
        {
            return this.duplicateDict.Count > 0;
        }

        public void OnBeforeSerialize()
        {
            this._duplicateDictKeys.Clear();
            this._duplicateDictValues.Clear();

            foreach (var kvp in this.duplicateDict)
            {
                this._duplicateDictKeys.Add(kvp.Key);
                this._duplicateDictValues.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.duplicateDict = new();
            for (var i = 0; i != Math.Min(this._duplicateDictKeys.Count, this._duplicateDictValues.Count); i++) this.duplicateDict.Add(this._duplicateDictKeys[i], new(this._duplicateDictValues[i].Guids));
        }

        internal void RefreshData()
        {
            //We need to analyze the scrollview to optimize how we draw it           
            this.RequiresScrollviewRebuild = true;

            this.duplicateDict = new();
            var hashDict = new Dictionary<string, List<string>>();

            var paths     = AssetDatabase.GetAllAssetPaths();
            var pathCount = paths.Length;

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                string             assetPathGuid;
                string             hash;
                UnityEngine.Object LoadedObj;

                var maxReadCount = 30; //We dont need to read every line using streamreader. We only need the m_name property, and that comes early in the file
                var lineCounter  = 0;

                for (var i = 0; i < pathCount; i++)
                {
                    var path = paths[i];
                    if (AssetDatabase.IsValidFolder(path) || !path.StartsWith("Assets")) //Slow, could be done recusively
                        continue;

                    if (EditorUtility.DisplayCancelableProgressBar("Finding duplicates", path, (float)i / (float)pathCount))
                    {
                        this.duplicateDict = new();
                        break;
                    }

                    assetPathGuid = AssetDatabase.AssetPathToGUID(path);
                    LoadedObj     = AssetDatabase.LoadMainAssetAtPath(path);
                    var line      = "";
                    var foundName = false;

                    if (LoadedObj != null)
                        try
                        {
                            using (var stream = File.OpenRead(path))
                            {
                                //We need to loop through certain native types (such as materials) to remove name from metadata - if we dont they wont have same hash
                                if (AssetDatabase.IsNativeAsset(LoadedObj) && !LoadedObj.GetType().IsSubclassOf(typeof(ScriptableObject)))
                                {
                                    var appendString = "";
                                    using (var sr = new StreamReader(stream))
                                    {
                                        //bool foundFileName = false;
                                        lineCounter = 0;
                                        while (!sr.EndOfStream)
                                        {
                                            lineCounter++;
                                            if (lineCounter >= maxReadCount)
                                            {
                                                appendString += sr.ReadToEnd();
                                            }
                                            else
                                            {
                                                line      = sr.ReadLine();
                                                foundName = line.Contains(LoadedObj.name);

                                                if (!foundName) //we want to ignore the m_name property, since that modifies the hashvalue
                                                    appendString += line;
                                                else
                                                    appendString += sr.ReadToEnd();
                                            }
                                        }
                                    }
                                    hash = BitConverter.ToString(System.Text.Encoding.Unicode.GetBytes(appendString));
                                }
                                else
                                {
                                    hash = BitConverter.ToString(md5.ComputeHash(stream));
                                }

                                if (!hashDict.ContainsKey(hash))
                                    hashDict.Add(hash, new() { assetPathGuid });
                                else
                                    hashDict[hash].Add(assetPathGuid);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                }

                foreach (var pair in hashDict)
                    if (pair.Value.Count > 1)
                        this.duplicateDict.Add(pair.Key, new(pair.Value));

                this.IsDirty  = false;
                this.HasCache = true;
                EditorUtility.ClearProgressBar();
            }
        }
    }
}