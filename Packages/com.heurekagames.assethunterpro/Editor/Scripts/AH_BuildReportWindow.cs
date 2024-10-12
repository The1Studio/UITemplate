//Only avaliable in 2018

#if UNITY_2018_1_OR_NEWER

using HeurekaGames.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_BuildReportWindow : EditorWindow
    {
        private static AH_BuildReportWindow     m_window;
        private        Vector2                  scrollPos;
        protected      AH_BuildInfoManager      buildInfoManager;
        private        AH_BuildReportWindowData data;

        //Adding same string multiple times in order to show more green and yellow than orange and red
        public static readonly List<string> ColorDotIconList = new()
        {
            "sv_icon_dot6_pix16_gizmo",
            "sv_icon_dot5_pix16_gizmo",
            "sv_icon_dot5_pix16_gizmo",
            "sv_icon_dot4_pix16_gizmo",
            "sv_icon_dot4_pix16_gizmo",
            "sv_icon_dot4_pix16_gizmo",
            "sv_icon_dot3_pix16_gizmo",
            "sv_icon_dot3_pix16_gizmo",
            "sv_icon_dot3_pix16_gizmo",
            "sv_icon_dot3_pix16_gizmo",
        };

        [MenuItem("Tools/Asset Hunter PRO/Build report")]
        [MenuItem("Window/Heureka/Asset Hunter PRO/Build report")]
        public static void Init()
        {
            //Make sure it exists so we can attach this window next to it
            AH_Window.GetBuildInfoManager();

            var alreadyExist = m_window != null;
            if (!alreadyExist)
            {
                m_window                    = GetWindow<AH_BuildReportWindow>("AH Report", true, typeof(AH_Window));
                m_window.titleContent.image = AH_EditorData.Icons.Report;

                m_window.buildInfoManager                             =  AH_Window.GetBuildInfoManager();
                m_window.buildInfoManager.OnBuildInfoSelectionChanged += m_window.OnBuildInfoSelectionChanged;
                m_window.populateBuildReportWindowData();
            }
        }

        private void OnBuildInfoSelectionChanged()
        {
            this.populateBuildReportWindowData();
        }

        private void populateBuildReportWindowData()
        {
            if (this.buildInfoManager.HasSelection)
            {
                this.data = new(this.buildInfoManager.GetSerializedBuildInfo());
                this.data.SetRelativeValuesForFiles();
            }
        }

        private void OnGUI()
        {
            if (!m_window) Init();
            Heureka_WindowStyler.DrawGlobalHeader(Heureka_WindowStyler.clr_Dark, "BUILD REPORT", AH_Window.VERSION);

            if (this.buildInfoManager == null || this.buildInfoManager.HasSelection == false)
            {
                Heureka_WindowStyler.DrawCenteredMessage(m_window, AH_EditorData.Icons.IconLargeWhite, 310f, 110f, "No buildinfo currently loaded in main window");
                return;
            }
            else if (this.buildInfoManager.IsMergedReport())
            {
                Heureka_WindowStyler.DrawCenteredMessage(m_window, AH_EditorData.Icons.IconLargeWhite, 366f, 110f, "Buildreport window does not work with merged buildreports");
                return;
            }

            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);
            this.data.OnGUI();

            EditorGUILayout.EndScrollView();
        }

        [Serializable]
        private class AH_BuildReportWindowData
        {
            [SerializeField] private ulong                              buildSize;
            [SerializeField] private string                             buildTarget;
            [SerializeField] private string                             buildDate;
            [SerializeField] private List<AH_BuildReportWindowRoleInfo> roleInfoList;

            public AH_BuildReportWindowData(AH_SerializedBuildInfo buildInfo)
            {
                this.buildSize   = buildInfo.TotalSize;
                this.buildTarget = buildInfo.buildTargetInfo;
                this.buildDate   = buildInfo.dateTime;

                this.roleInfoList = new();
                foreach (var item in buildInfo.BuildReportInfoList)
                    //Check if role exists already
                {
                    if (this.roleInfoList.Exists(val => val.roleName.Equals(item.Role)))
                        this.roleInfoList.First(val => val.roleName.Equals(item.Role)).AddToRoleInfo(item);
                    //If not, add new roleentry
                    else
                        this.roleInfoList.Add(new(item));
                }

                //Sort roles
                IEnumerable<AH_BuildReportWindowRoleInfo> tmp = this.roleInfoList.OrderByDescending(val => val.combinedRoleSize);
                this.roleInfoList = tmp.ToList();

                //Sort elements in roles
                foreach (var item in this.roleInfoList) item.Order();
            }

            internal void OnGUI()
            {
                if (this.buildSize <= 0)
                {
                    Heureka_WindowStyler.DrawCenteredMessage(m_window, AH_EditorData.Icons.IconLargeWhite, 462f, 120f, "The selected buildinfo lacks information. It was probably created with older version. Create new with this version");
                    return;
                }

                var guiWidth = 260;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(" Combined Build Size:", Heureka_EditorData.Instance.HeadlineStyle, GUILayout.Width(guiWidth));
                EditorGUILayout.LabelField(AH_Utils.GetSizeAsString(this.buildSize), Heureka_EditorData.Instance.HeadlineStyle);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(" Build Target:", Heureka_EditorData.Instance.HeadlineStyle, GUILayout.Width(guiWidth));
                EditorGUILayout.LabelField(this.buildTarget, Heureka_EditorData.Instance.HeadlineStyle);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(" Build Time:", Heureka_EditorData.Instance.HeadlineStyle, GUILayout.Width(guiWidth));
                var parsedDate = DateTime.ParseExact(this.buildDate, AH_SerializationHelper.DateTimeFormat, System.Globalization.CultureInfo.CurrentCulture).ToString();
                EditorGUILayout.LabelField(parsedDate, Heureka_EditorData.Instance.HeadlineStyle);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                foreach (var item in this.roleInfoList)
                {
                    item.OnGUI();
                    EditorGUILayout.Space();
                }
            }

            internal void SetRelativeValuesForFiles()
            {
                //Find the relative value of all items so we can show which files are taking up the most space
                //A way to keep track of the sorted values
                var tmpList = new List<AH_BuildReportWindowFileInfo>();
                foreach (var infoList in this.roleInfoList)
                foreach (var fileInfo in infoList.fileInfoList)
                    tmpList.Add(fileInfo);

                var sortedFileInfo = tmpList.OrderByDescending(val => val.size).ToList();
                for (var i = 0; i < sortedFileInfo.Count; i++)
                {
                    var groupSize = ColorDotIconList.Count;
                    //Figure out which icon to show (create 4 groups from sortedlist)
                    var groupIndex = Mathf.FloorToInt((float)i / (float)sortedFileInfo.Count * (float)groupSize);
                    sortedFileInfo[i].SetFileSizeGroup(groupIndex);
                }
            }
        }

        [Serializable]
        internal class AH_BuildReportWindowRoleInfo
        {
            [SerializeField] internal ulong                              combinedRoleSize = 0;
            [SerializeField] internal string                             roleName;
            [SerializeField] internal List<AH_BuildReportWindowFileInfo> fileInfoList;

            public AH_BuildReportWindowRoleInfo(AH_BuildReportFileInfo item)
            {
                this.roleName     = item.Role;
                this.fileInfoList = new();
                this.addFile(item);
            }

            internal void AddToRoleInfo(AH_BuildReportFileInfo item)
            {
                this.combinedRoleSize += item.Size;
                this.addFile(item);
            }

            private void addFile(AH_BuildReportFileInfo item)
            {
                this.combinedRoleSize += item.Size;
                this.fileInfoList.Add(new(item));
            }

            internal void OnGUI()
            {
                EditorGUILayout.HelpBox(this.roleName + " combined: " + AH_Utils.GetSizeAsString(this.combinedRoleSize), MessageType.Info);
                foreach (var item in this.fileInfoList) item.OnGUI();
            }

            internal void Order()
            {
                IEnumerable<AH_BuildReportWindowFileInfo> tmp = this.fileInfoList.OrderByDescending(val => val.size);
                this.fileInfoList = tmp.ToList();
            }
        }

        [Serializable]
        internal class AH_BuildReportWindowFileInfo
        {
            [SerializeField] internal string     fileName;
            [SerializeField] internal string     path;
            [SerializeField] internal ulong      size;
            [SerializeField] internal string     sizeString;
            [SerializeField] private  GUIContent content       = new();
            [SerializeField] private  int        fileSizeGroup = 0;

            public AH_BuildReportWindowFileInfo(AH_BuildReportFileInfo item)
            {
                this.path       = item.Path;
                this.fileName   = System.IO.Path.GetFileName(this.path);
                this.size       = item.Size;
                this.sizeString = AH_Utils.GetSizeAsString(this.size);

                this.content.text    = this.fileName;
                this.content.tooltip = this.path;
            }

            internal void OnGUI()
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(this.content, GUILayout.MinWidth(300));
                EditorGUILayout.LabelField(this.sizeString, GUILayout.MaxWidth(80));
                GUILayout.Label(EditorGUIUtility.IconContent(ColorDotIconList[this.fileSizeGroup]), GUILayout.MaxHeight(16));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            internal void SetFileSizeGroup(int groupIndex)
            {
                this.fileSizeGroup = groupIndex;
            }
        }

        private void OnDestroy()
        {
            m_window.buildInfoManager.OnBuildInfoSelectionChanged -= m_window.OnBuildInfoSelectionChanged;
        }
    }
}
#endif