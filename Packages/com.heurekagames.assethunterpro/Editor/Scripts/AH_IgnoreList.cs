using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HeurekaGames;
using HeurekaGames.Utils;

namespace HeurekaGames.AssetHunterPRO
{
    [Serializable]
    public class AH_IgnoreList
    {
        public delegate void ListUpdatedHandler();

        public event ListUpdatedHandler ListUpdatedEvent;

        private List<string> DefaultIgnored;

        /// <summary>
        /// List of the Ignored items
        /// </summary>
        [SerializeField]
        private SerializedIgnoreArray CombinedIgnored = new();

        /// <summary>
        /// Id of the playerpref location
        /// </summary>
        private string playerPrefKey;

        /// <summary>
        /// Interface that deals with drawing and excluding in a way that is specific to the type of exclusion we are doing
        /// </summary>
        private AH_IIgnoreListActions exclusionActions;

        //Size of buttons
        public const float  ButtonSpacer = 70;
        private      string toBeDeleted;
        private      bool   isDirty;

        public AH_IgnoreList(AH_IIgnoreListActions exclusionActions, List<string> Ignored, string playerPrefKey)
        {
            this.exclusionActions                   =  exclusionActions;
            this.exclusionActions.IgnoredAddedEvent += this.onAddedToignoredList;
            this.playerPrefKey                      =  playerPrefKey;
            this.DefaultIgnored                     =  new(Ignored);
        }

        internal List<string> GetIgnored()
        {
            //We already have a list of Ignores
            if (this.CombinedIgnored.Ignored.Count >= 1)
            {
                return this.CombinedIgnored.Ignored;
            }
            //We have no list, so read the defaults
            else if (EditorPrefs.HasKey(this.playerPrefKey))
                //Populates this class from prefs
            {
                this.CombinedIgnored = this.LoadFromPrefs();
            }
            else
            {
                //Save the default values into prefs
                this.SaveDefault();
                //Try to get values again after having set default to prefs
                return this.GetIgnored();
            }

            //Make sure default and combined are synced
            //If combined has element that doesn't exist in combined, add it!
            if (this.DefaultIgnored.Exists(val => !this.CombinedIgnored.Ignored.Contains(val))) this.CombinedIgnored.Ignored.AddRange(this.DefaultIgnored.FindAll(val => !this.CombinedIgnored.Ignored.Contains(val)));

            //Returns the values that have been read from prefs
            return this.CombinedIgnored.Ignored;
        }

        public SerializedIgnoreArray LoadFromPrefs()
        {
            return JsonUtility.FromJson<SerializedIgnoreArray>(EditorPrefs.GetString(this.playerPrefKey));
        }

        internal bool IsDirty()
        {
            return this.isDirty;
        }

        public void Save()
        {
            var newJsonString = JsonUtility.ToJson(this.CombinedIgnored);
            var oldJsonString = EditorPrefs.GetString(this.playerPrefKey);

            //If we haven't changed anything, dont save
            if (newJsonString.Equals(oldJsonString)) return;

            this.SetDirty(true);

            //Copy the default values into the other list
            EditorPrefs.SetString(this.playerPrefKey, newJsonString);

            //Send event that list was update
            if (this.ListUpdatedEvent != null) this.ListUpdatedEvent();
        }

        public void SaveDefault()
        {
            //Copy the default values into the other list
            this.CombinedIgnored = new(this.DefaultIgnored);
            this.Save();
        }

        internal void Reset()
        {
            this.SaveDefault();
        }

        internal void DrawIgnoreButton()
        {
            /*if (isDirty)
                return;*/

            this.exclusionActions.DrawIgnored(this);
        }

        internal void OnGUI()
        {
            if (Event.current.type != EventType.Layout && this.isDirty)
            {
                this.SetDirty(false);
                return;
            }

            //See if we are currently deleting an Ignore
            this.checkToDelete();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(this.exclusionActions.Header + " (" + this.GetIgnored().Count + ")", MessageType.None);

            if (this.GetIgnored().Count > 0)
            {
                EditorGUI.indentLevel++;
                foreach (var item in this.GetIgnored()) this.drawIgnored(item, this.exclusionActions.GetLabelFormattedItem(item));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void checkToDelete()
        {
            if (string.IsNullOrEmpty(this.toBeDeleted)) return;

            this.removeFromignoredList(this.toBeDeleted);
        }

        private void drawIgnored(string identifier, string legible)
        {
            if (string.IsNullOrEmpty(identifier)) return;

            EditorGUILayout.BeginHorizontal();
            if (!this.DefaultIgnored.Contains(identifier))
            {
                if (GUILayout.Button("Un-Ignore", EditorStyles.miniButton, GUILayout.Width(ButtonSpacer))) this.markForDeletion(identifier);
            }
            //Hidden button to help align, probably not the most elegant solution, but Ill fix later
            else if (this.DefaultIgnored.Count != this.GetIgnored().Count)
            {
                GUILayout.Button("", GUIStyle.none, GUILayout.Width(ButtonSpacer + 4));
            }

            var curWidth = EditorGUIUtility.labelWidth;
            EditorGUILayout.LabelField(this.getLabelContent(legible), GUILayout.MaxWidth(1024));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        internal void IgnoreElement(string identifier, bool ignore)
        {
            if (ignore && !this.ExistsInList(identifier)) this.AddToignoredList(identifier);

            if (!ignore && this.ExistsInList(identifier)) this.markForDeletion(identifier);
        }

        private void markForDeletion(string item)
        {
            this.toBeDeleted = item;
        }

        protected virtual string getLabelContent(string item)
        {
            return item;
        }

        public bool ExistsInList(string element)
        {
            return this.GetIgnored().Contains(element);
        }

        private void onAddedToignoredList(object sender, IgnoreListEventArgs e)
        {
            this.AddToignoredList(e.Item);
        }

        public void AddToignoredList(string element)
        {
            if (this.GetIgnored().Contains(element))
            {
                Debug.LogWarning("AH: Element already ignored: " + element);
                return;
            }

            this.GetIgnored().Add(element);
            //Save to prefs
            this.Save();
        }

        protected void removeFromignoredList(string element)
        {
            this.toBeDeleted = "";
            this.GetIgnored().Remove(element);
            //Save to prefs
            this.Save();
        }

        //Sets dirty so we know we need to manage the IMGUI (Mismatched LayoutGroup.Repaint)
        private void SetDirty(bool bDirty)
        {
            this.isDirty = bDirty;
        }

        internal bool ContainsElement(string localpath, string identifier = "")
        {
            return this.exclusionActions.ContainsElement(this.GetIgnored(), localpath, identifier);
        }

        [Serializable]
        public class SerializedIgnoreArray
        {
            /// <summary>
            /// List of the Ignored items
            /// </summary>
            [SerializeField]
            public List<string> Ignored = new();

            public SerializedIgnoreArray()
            {
            }

            public SerializedIgnoreArray(List<string> defaultIgnored)
            {
                this.Ignored = new(defaultIgnored);
            }
        }
    }

    public class AH_ExclusionTypeList : AH_IgnoreList
    {
        //Call base constructor but convert the types into serializable values
        public AH_ExclusionTypeList(AH_IIgnoreListActions exclusionAction, List<Type> Ignored, string playerPrefsKey) : base(exclusionAction, Ignored.ConvertAll<string>(val => Heureka_Serializer.SerializeType(val)), playerPrefsKey)
        {
        }

        //Return the type tostring instead of the fully qualified type identifier
        protected override string getLabelContent(string item)
        {
            var deserializedType = Heureka_Serializer.DeSerializeType(base.getLabelContent(item));

            if (deserializedType != null)
                return deserializedType.ToString();
            //The Ignored type does no longer exist in project
            else
                return "Unrecognized type : " + item;
        }

        internal void IgnoreType(Type t, bool ignore)
        {
            this.IgnoreElement(Heureka_Serializer.SerializeType(t), ignore);
        }
    }

    public class IgnoreListEventArgs : EventArgs
    {
        public string Item;

        public IgnoreListEventArgs(Type item)
        {
            this.Item = Heureka_Serializer.SerializeType(item);
        }

        public IgnoreListEventArgs(string item)
        {
            this.Item = item;
        }
    }
}