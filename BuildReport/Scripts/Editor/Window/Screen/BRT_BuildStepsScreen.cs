using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace BuildReportTool.Window.Screen
{
    public class BuildStepsScreen : BaseScreen
    {
        public override string Name => "Build Steps";

        public override void RefreshData(
            BuildInfo         buildReport,
            AssetDependencies assetDependencies,
            TextureData       textureData,
            MeshData          meshData,
            UnityBuildReport  unityBuildReport
        )
        {
            if (unityBuildReport != null) this.SelectStep(-1, unityBuildReport.BuildProcessSteps);
        }

        private Vector2 _scrollPos;
        private Vector2 _logMessagesScrollPos;
        private Texture _indentLine;

        private int _selectedStepIdx    = -1;
        private int _selectedLogStepIdx = -1;
        private int _selectedLogIdx     = -1;

        private Texture2D _logIcon;
        private Texture2D _warnIcon;
        private Texture2D _errorIcon;

        private readonly GUIContent _logFilterLabel   = new("0");
        private readonly GUIContent _warnFilterLabel  = new("0");
        private readonly GUIContent _errorFilterLabel = new("0");

        private Rect _stepsViewRect;

        private bool _showLogMessagesCollapsed;
        private bool _showLogMessages   = true;
        private bool _showWarnMessages  = true;
        private bool _showErrorMessages = true;

        private int _infoMessageCount;
        private int _warnMessageCount;
        private int _errorMessageCount;

        private int _collapsedInfoMessageCount;
        private int _collapsedWarnMessageCount;
        private int _collapsedErrorMessageCount;

        private int _totalVisibleMessageCount;

        private struct LogMsgIdx
        {
            public int StepIdx;
            public int LogIdx;
        }

        private static LogMsgIdx MakeLogMsgIdx(int stepIdx, int logIdx)
        {
            LogMsgIdx newEntry;
            newEntry.StepIdx = stepIdx;
            newEntry.LogIdx  = logIdx;
            return newEntry;
        }

        private readonly Dictionary<LogMsgIdx, Rect> _logRects = new();

        private float _buildStepsHeightPercent = 0.5f;

        private Rect  _dividerRect;
        private bool  _draggingDivider;
        private float _mouseYOnDividerDragStart;
        private float _heightOnDividerDragStart;

        private int _logMessageToShowStartOffset = 0;

        private bool _showPageNumbers = true;

        // ================================================================================================

        public override void DrawGUI(
            Rect                      position,
            BuildInfo                 buildReportToDisplay,
            AssetDependencies         assetDependencies,
            TextureData               textureData,
            MeshData                  meshData,
            UnityBuildReport          unityBuildReport,
            BuildReportTool.ExtraData extraData,
            out bool                  requestRepaint
        )
        {
            requestRepaint = false;
            if (unityBuildReport == null) return;

            var steps = unityBuildReport.BuildProcessSteps;
            if (steps == null) return;

            if (this._logIcon == null)
            {
                var logIcons = GUI.skin.FindStyle("LogMessageIcons");
                if (logIcons != null)
                {
                    this._logIcon   = logIcons.normal.background;
                    this._warnIcon  = logIcons.hover.background;
                    this._errorIcon = logIcons.active.background;

                    this._logFilterLabel.image   = this._logIcon;
                    this._warnFilterLabel.image  = this._warnIcon;
                    this._errorFilterLabel.image = this._errorIcon;
                }
            }

            if (this._indentLine == null)
            {
                var indentStyle                           = GUI.skin.FindStyle("IndentStyle1");
                if (indentStyle != null) this._indentLine = indentStyle.normal.background;
            }

            Texture2D prevArrow;
            var       prevArrowStyle = GUI.skin.FindStyle(Settings.BIG_LEFT_ARROW_ICON_STYLE_NAME);
            if (prevArrowStyle != null)
                prevArrow = prevArrowStyle.normal.background;
            else
                prevArrow = null;

            Texture2D nextArrow;
            var       nextArrowStyle = GUI.skin.FindStyle(Settings.BIG_RIGHT_ARROW_ICON_STYLE_NAME);
            if (nextArrowStyle != null)
                nextArrow = nextArrowStyle.normal.background;
            else
                nextArrow = null;

            var buttonStyle                      = GUI.skin.FindStyle(Settings.TOP_BAR_BTN_STYLE_NAME);
            if (buttonStyle == null) buttonStyle = GUI.skin.button;

            var topBarBgStyle                        = GUI.skin.FindStyle(Settings.TOP_BAR_BG_STYLE_NAME);
            if (topBarBgStyle == null) topBarBgStyle = GUI.skin.label;

            var topBarLabelStyle                           = GUI.skin.FindStyle(Settings.TOP_BAR_LABEL_STYLE_NAME);
            if (topBarLabelStyle == null) topBarLabelStyle = GUI.skin.label;

            var columnHeaderStyle                            = GUI.skin.FindStyle(Settings.LIST_COLUMN_HEADER_STYLE_NAME);
            if (columnHeaderStyle == null) columnHeaderStyle = GUI.skin.label;

            var hiddenHorizontalScrollbarStyle                                         = GUI.skin.FindStyle(Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
            if (hiddenHorizontalScrollbarStyle == null) hiddenHorizontalScrollbarStyle = GUI.skin.horizontalScrollbar;

            var hiddenVerticalScrollbarStyle                                       = GUI.skin.FindStyle(Settings.HIDDEN_SCROLLBAR_STYLE_NAME);
            if (hiddenVerticalScrollbarStyle == null) hiddenVerticalScrollbarStyle = GUI.skin.verticalScrollbar;

            var verticalScrollbarStyle = GUI.skin.verticalScrollbar;

            var listNormalStyle                          = GUI.skin.FindStyle(Settings.LIST_SMALL_STYLE_NAME);
            if (listNormalStyle == null) listNormalStyle = GUI.skin.label;

            var listAltStyle                       = GUI.skin.FindStyle(Settings.LIST_SMALL_ALT_STYLE_NAME);
            if (listAltStyle == null) listAltStyle = GUI.skin.label;

            var listSelectedStyle                            = GUI.skin.FindStyle(Settings.LIST_SMALL_SELECTED_NAME);
            if (listSelectedStyle == null) listSelectedStyle = GUI.skin.label;

            // --------------------------------

            #region Steps

            var height                     = position.height * this._buildStepsHeightPercent;
            var maxHeight                  = (steps.Length + 1) * 20;
            if (height > maxHeight) height = maxHeight;
            GUILayout.BeginHorizontal(GUILayout.Height(height));

            #region Column 1 (Step Name)

            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Step", columnHeaderStyle);
            this._scrollPos = GUILayout.BeginScrollView(this._scrollPos,
                hiddenHorizontalScrollbarStyle,
                hiddenVerticalScrollbarStyle,
                BRT_BuildReportWindow.LayoutNone);

            var useAlt = true;
            for (var i = 0; i < steps.Length; ++i)
            {
                var styleToUse                             = useAlt ? listAltStyle : listNormalStyle;
                if (i == this._selectedStepIdx) styleToUse = listSelectedStyle;

                GUILayout.BeginHorizontal(styleToUse);
                GUILayout.Space(steps[i].Depth * 20);
                if (GUILayout.Button(steps[i].Name, styleToUse, BRT_BuildReportWindow.LayoutListHeight)) this.SelectStep(i, steps);
                GUILayout.EndHorizontal();
                if (Event.current.type == EventType.Repaint && this._indentLine != null)
                {
                    var labelRect = GUILayoutUtility.GetLastRect();

                    var prevColor = GUI.color;
                    GUI.color = new(0, 0, 0, 0.5f);
                    for (int indentN = 0, indentLen = steps[i].Depth;
                        indentN < indentLen;
                        ++indentN)
                    {
                        var indentRect = new Rect(indentN * 20, labelRect.y, 20, labelRect.height);
                        GUI.DrawTexture(indentRect, this._indentLine, ScaleMode.ScaleAndCrop);
                    }

                    GUI.color = prevColor;
                }

                useAlt = !useAlt;
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            #endregion

            #region Column 2 (Warning Count)

            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Warning Count", columnHeaderStyle);
            this._scrollPos = GUILayout.BeginScrollView(this._scrollPos,
                hiddenHorizontalScrollbarStyle,
                hiddenVerticalScrollbarStyle,
                BRT_BuildReportWindow.LayoutNone);
            useAlt = true;
            for (var i = 0; i < steps.Length; ++i)
            {
                var styleToUse                             = useAlt ? listAltStyle : listNormalStyle;
                if (i == this._selectedStepIdx) styleToUse = listSelectedStyle;

                if (steps[i].WarnLogCount > 0)
                {
                    if (GUILayout.Button(steps[i].WarnLogCount.ToString(), styleToUse, BRT_BuildReportWindow.LayoutListHeight)) this.SelectStep(i, steps);
                }
                else
                {
                    GUILayout.Label(GUIContent.none, styleToUse, BRT_BuildReportWindow.LayoutListHeight);
                }
                useAlt = !useAlt;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            #endregion

            #region Column 3 (Error Count)

            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Error Count", columnHeaderStyle);
            this._scrollPos = GUILayout.BeginScrollView(this._scrollPos,
                hiddenHorizontalScrollbarStyle,
                hiddenVerticalScrollbarStyle,
                BRT_BuildReportWindow.LayoutNone);
            useAlt = true;
            for (var i = 0; i < steps.Length; ++i)
            {
                var styleToUse                             = useAlt ? listAltStyle : listNormalStyle;
                if (i == this._selectedStepIdx) styleToUse = listSelectedStyle;

                if (steps[i].ErrorLogCount > 0)
                {
                    if (GUILayout.Button(steps[i].ErrorLogCount.ToString(), styleToUse, BRT_BuildReportWindow.LayoutListHeight)) this.SelectStep(i, steps);
                }
                else
                {
                    GUILayout.Label(GUIContent.none, styleToUse, BRT_BuildReportWindow.LayoutListHeight);
                }
                useAlt = !useAlt;
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            #endregion

            #region Last Column (Duration)

            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Duration", columnHeaderStyle);
            this._scrollPos = GUILayout.BeginScrollView(this._scrollPos,
                hiddenHorizontalScrollbarStyle,
                verticalScrollbarStyle,
                BRT_BuildReportWindow.LayoutNone);

            useAlt = true;
            for (var i = 0; i < steps.Length; ++i)
            {
                var styleToUse                             = useAlt ? listAltStyle : listNormalStyle;
                if (i == this._selectedStepIdx) styleToUse = listSelectedStyle;

                string duration;
                if (i == 0)
                    duration = unityBuildReport.TotalBuildTime.ToReadableString();
                else
                    duration = steps[i].Duration.ToReadableString();

                GUILayout.Label(duration, styleToUse, BRT_BuildReportWindow.LayoutListHeight);

                useAlt = !useAlt;
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            #endregion

            GUILayout.EndHorizontal();
            if (Event.current.type == EventType.Repaint) this._stepsViewRect = GUILayoutUtility.GetLastRect();

            #endregion

            // --------------------------------

            #region Logs

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            #region Logs Toolbar

            GUILayout.BeginHorizontal(topBarBgStyle, BRT_BuildReportWindow.LayoutHeight18);
            GUILayout.Space(10);
            string logMessagesTitle;
            var    hasStepSelected = this._selectedStepIdx != -1 && steps[this._selectedStepIdx].BuildLogMessages != null && steps[this._selectedStepIdx].BuildLogMessages.Length > 0;
            if (hasStepSelected)
                logMessagesTitle = string.Format("Log Messages of: <i>{0}</i>", steps[this._selectedStepIdx].Name);
            else
                logMessagesTitle = "Log Messages (Total)";
            GUILayout.Label(logMessagesTitle, topBarLabelStyle, BRT_BuildReportWindow.LayoutNoExpandWidth);
            if (Event.current.type == EventType.Repaint) this._dividerRect = GUILayoutUtility.GetLastRect();
            GUILayout.FlexibleSpace();

            var messagePaginationLength = BuildReportTool.Options.LogMessagePaginationLength;

            var prevButton = prevArrow != null
                ? GUILayout.Button(prevArrow, buttonStyle)
                : GUILayout.Button("Previous", buttonStyle);
            if (prevButton && this._logMessageToShowStartOffset - messagePaginationLength >= 0) this._logMessageToShowStartOffset -= messagePaginationLength;
            if (Event.current.type == EventType.Repaint)
            {
                var prevArrowRect = GUILayoutUtility.GetLastRect();
                this._dividerRect.xMax = prevArrowRect.x;
            }

            string paginateLabel;
            if (this._showPageNumbers)
            {
                var totalPageNumbers = this._totalVisibleMessageCount / messagePaginationLength;
                if (this._totalVisibleMessageCount % messagePaginationLength > 0) ++totalPageNumbers;

                // the max number of digits for the displayed offset counters
                var assetCountDigitNumFormat = string.Format("D{0}", totalPageNumbers.ToString().Length.ToString());

                paginateLabel = string.Format("Page {0} of {1}",
                    (this._logMessageToShowStartOffset / messagePaginationLength + 1).ToString(assetCountDigitNumFormat),
                    totalPageNumbers.ToString());
            }
            else
            {
                // number of assets in current page
                var pageLength = Mathf.Min(this._logMessageToShowStartOffset + messagePaginationLength, this._totalVisibleMessageCount);

                var offsetNonZeroBased = this._logMessageToShowStartOffset + (pageLength > 0 ? 1 : 0);

                // the max number of digits for the displayed offset counters
                var assetCountDigitNumFormat = string.Format("D{0}", this._totalVisibleMessageCount.ToString().Length.ToString());

                paginateLabel = string.Format("Page {0} - {1} of {2}",
                    offsetNonZeroBased.ToString(assetCountDigitNumFormat),
                    pageLength.ToString(assetCountDigitNumFormat),
                    this._totalVisibleMessageCount.ToString(assetCountDigitNumFormat));
            }

            if (GUILayout.Button(paginateLabel, topBarLabelStyle, BRT_BuildReportWindow.LayoutNone)) this._showPageNumbers = !this._showPageNumbers;

            var nextButton = nextArrow != null
                ? GUILayout.Button(nextArrow, buttonStyle)
                : GUILayout.Button("Next", buttonStyle);
            if (nextButton && this._logMessageToShowStartOffset + messagePaginationLength < this._totalVisibleMessageCount) this._logMessageToShowStartOffset += messagePaginationLength;

            GUILayout.Space(8);

            var newShowLogMessagesCollapsed = GUILayout.Toggle(this._showLogMessagesCollapsed,
                "Collapse",
                buttonStyle,
                BRT_BuildReportWindow.LayoutNoExpandWidth);
            if (newShowLogMessagesCollapsed != this._showLogMessagesCollapsed)
            {
                this._showLogMessagesCollapsed = newShowLogMessagesCollapsed;
                this.RefreshTotalVisibleMessageCount();
            }
            GUILayout.Space(8);
            var newShowLogMessages = GUILayout.Toggle(this._showLogMessages,
                this._logFilterLabel,
                buttonStyle,
                BRT_BuildReportWindow.LayoutNoExpandWidth);
            var newShowWarnMessages = GUILayout.Toggle(this._showWarnMessages,
                this._warnFilterLabel,
                buttonStyle,
                BRT_BuildReportWindow.LayoutNoExpandWidth);
            var newShowErrorMessages = GUILayout.Toggle(this._showErrorMessages,
                this._errorFilterLabel,
                buttonStyle,
                BRT_BuildReportWindow.LayoutNoExpandWidth);
            if (newShowLogMessages != this._showLogMessages)
            {
                this._showLogMessages = newShowLogMessages;
                this.RefreshTotalVisibleMessageCount();
            }
            if (newShowWarnMessages != this._showWarnMessages)
            {
                this._showWarnMessages = newShowWarnMessages;
                this.RefreshTotalVisibleMessageCount();
            }
            if (newShowErrorMessages != this._showErrorMessages)
            {
                this._showErrorMessages = newShowErrorMessages;
                this.RefreshTotalVisibleMessageCount();
            }
            GUILayout.Space(8);
            GUILayout.EndHorizontal();

            EditorGUIUtility.AddCursorRect(this._dividerRect, MouseCursor.ResizeVertical);

            #endregion

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && this._dividerRect.Contains(Event.current.mousePosition))
            {
                this._draggingDivider          = true;
                this._mouseYOnDividerDragStart = Event.current.mousePosition.y;
                this._heightOnDividerDragStart = height;
                requestRepaint                 = true;
            }

            if (Event.current.type == EventType.MouseUp)
            {
                this._draggingDivider = false;
                requestRepaint        = true;
            }

            if (this._draggingDivider)
            {
                var newHeight = this._heightOnDividerDragStart + (Event.current.mousePosition.y - this._mouseYOnDividerDragStart);
                this._buildStepsHeightPercent = newHeight / position.height;
                requestRepaint                = true;
            }

            this._logMessagesScrollPos = GUILayout.BeginScrollView(this._logMessagesScrollPos,
                hiddenHorizontalScrollbarStyle,
                verticalScrollbarStyle,
                BRT_BuildReportWindow.LayoutNone);

            if (this._showLogMessages || this._showWarnMessages || this._showErrorMessages)
            {
                if (hasStepSelected)
                {
                    BuildLogMessage[] messages;
                    if (this._showLogMessagesCollapsed)
                        messages = steps[this._selectedStepIdx].CollapsedBuildLogMessages;
                    else
                        messages = steps[this._selectedStepIdx].BuildLogMessages;

                    var totalToShow                          = 0;
                    if (this._showLogMessages) totalToShow   += steps[this._selectedStepIdx].InfoLogCount;
                    if (this._showWarnMessages) totalToShow  += steps[this._selectedStepIdx].WarnLogCount;
                    if (this._showErrorMessages) totalToShow += steps[this._selectedStepIdx].ErrorLogCount;

                    if (totalToShow > 0)
                    {
                        useAlt = true;

                        var messageToStartIdx = 0;

                        var messageToStartCount = 0;
                        for (var m = 0; m < messages.Length; ++m)
                        {
                            var logTypeIcon = this.GetLogIcon(messages[m].LogType);
                            if (logTypeIcon == this._logIcon && !this._showLogMessages) continue;
                            if (logTypeIcon == this._warnIcon && !this._showWarnMessages) continue;
                            if (logTypeIcon == this._errorIcon && !this._showErrorMessages) continue;

                            ++messageToStartCount;
                            if (messageToStartCount - 1 == this._logMessageToShowStartOffset)
                            {
                                messageToStartIdx = m;
                                break;
                            }
                        }

                        this.DrawMessages(messages,
                            messageToStartIdx,
                            messagePaginationLength,
                            this._selectedStepIdx,
                            0,
                            ref useAlt,
                            ref requestRepaint);
                    }
                }
                else
                {
                    useAlt = true;

                    var messageToStartIdx = 0;
                    var stepToStartIdx    = 0;

                    var messageToStartCount = 0;
                    for (var s = 0; s < steps.Length; ++s)
                    {
                        var step = steps[s];

                        BuildLogMessage[] messages;
                        if (this._showLogMessagesCollapsed)
                            messages = step.CollapsedBuildLogMessages;
                        else
                            messages = step.BuildLogMessages;

                        if (messages == null || messages.Length == 0) continue;

                        var totalToShow                        = 0;
                        if (this._showLogMessages) totalToShow += step.InfoLogCount;

                        if (this._showWarnMessages) totalToShow += step.WarnLogCount;

                        if (this._showErrorMessages) totalToShow += step.ErrorLogCount;

                        if (totalToShow == 0) continue;

                        for (var m = 0; m < messages.Length; ++m)
                        {
                            var logTypeIcon = this.GetLogIcon(messages[m].LogType);
                            if (logTypeIcon == this._logIcon && !this._showLogMessages) continue;

                            if (logTypeIcon == this._warnIcon && !this._showWarnMessages) continue;

                            if (logTypeIcon == this._errorIcon && !this._showErrorMessages) continue;

                            ++messageToStartCount;
                            if (messageToStartCount - 1 == this._logMessageToShowStartOffset)
                            {
                                messageToStartIdx = m;
                                stepToStartIdx    = s;
                                break;
                            }
                        }
                    }

                    var totalShownSoFar = 0;
                    for (var s = stepToStartIdx; s < steps.Length; ++s)
                    {
                        var step = steps[s];

                        BuildLogMessage[] messages;
                        if (this._showLogMessagesCollapsed)
                            messages = step.CollapsedBuildLogMessages;
                        else
                            messages = step.BuildLogMessages;

                        if (messages == null || messages.Length == 0) continue;

                        var totalToShow = 0;
                        if (this._showLogMessages)
                        {
                            if (this._showLogMessagesCollapsed)
                                totalToShow += step.CollapsedInfoLogCount;
                            else
                                totalToShow += step.InfoLogCount;
                        }
                        if (this._showWarnMessages)
                        {
                            if (this._showLogMessagesCollapsed)
                                totalToShow += step.CollapsedWarnLogCount;
                            else
                                totalToShow += step.WarnLogCount;
                        }
                        if (this._showErrorMessages)
                        {
                            if (this._showLogMessagesCollapsed)
                                totalToShow += step.CollapsedErrorLogCount;
                            else
                                totalToShow += step.ErrorLogCount;
                        }

                        if (totalToShow == 0) continue;

                        var styleToUse = useAlt ? listAltStyle : listNormalStyle;

                        GUILayout.BeginHorizontal(styleToUse, BRT_BuildReportWindow.LayoutNone);
                        GUILayout.Space(8);
                        GUILayout.Button(step.Name, styleToUse, BRT_BuildReportWindow.LayoutHeight25);
                        GUILayout.EndHorizontal();

                        useAlt = !useAlt;

                        this.DrawMessages(messages,
                            messageToStartIdx,
                            messagePaginationLength - totalShownSoFar,
                            s,
                            20,
                            ref useAlt,
                            ref requestRepaint);

                        totalShownSoFar += totalToShow;

                        if (totalShownSoFar >= messagePaginationLength) break;
                    }
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            #endregion

            // if clicked on nothing interactable, then remove selection
            if (GUI.Button(this._stepsViewRect, GUIContent.none, hiddenVerticalScrollbarStyle)) this.SelectStep(-1, steps);
        }

        private void DrawMessages(BuildLogMessage[] messages, int messagesStartIdx, int messageLength, int stepIdx, int leftIndent, ref bool useAlt, ref bool requestRepaint)
        {
            var nativeSkin =
                EditorGUIUtility.GetBuiltinSkin(EditorGUIUtility.isProSkin ? EditorSkin.Scene : EditorSkin.Inspector);
            var logCountStyle = nativeSkin.FindStyle("CN CountBadge");

            var listNormalStyle                          = GUI.skin.FindStyle(Settings.LIST_NORMAL_STYLE_NAME);
            if (listNormalStyle == null) listNormalStyle = GUI.skin.label;

            var listAltStyle                       = GUI.skin.FindStyle(Settings.LIST_NORMAL_ALT_STYLE_NAME);
            if (listAltStyle == null) listAltStyle = GUI.skin.label;

            var listSelectedStyle                            = GUI.skin.FindStyle(Settings.LIST_NORMAL_SELECTED_NAME);
            if (listSelectedStyle == null) listSelectedStyle = GUI.skin.label;

            var textureStyle                       = GUI.skin.FindStyle("DrawTexture");
            if (textureStyle == null) textureStyle = GUI.skin.label;

            var textStyle                    = GUI.skin.FindStyle("Text");
            if (textStyle == null) textStyle = GUI.skin.label;

            var textSelectedStyle                            = GUI.skin.FindStyle("TextSelected");
            if (textSelectedStyle == null) textSelectedStyle = GUI.skin.label;

            var messagesShown = 0;
            for (var m = messagesStartIdx; m < messages.Length; ++m)
            {
                if (messagesShown >= messageLength) break;

                var logTypeIcon = this.GetLogIcon(messages[m].LogType);
                if (logTypeIcon == this._logIcon && !this._showLogMessages) continue;
                if (logTypeIcon == this._warnIcon && !this._showWarnMessages) continue;
                if (logTypeIcon == this._errorIcon && !this._showErrorMessages) continue;

                var logStyleToUse        = useAlt ? listAltStyle : listNormalStyle;
                var logMessageStyleToUse = textStyle;
                if (stepIdx == this._selectedLogStepIdx && m == this._selectedLogIdx)
                {
                    logStyleToUse        = listSelectedStyle;
                    logMessageStyleToUse = textSelectedStyle;
                }

                GUILayout.BeginHorizontal(logStyleToUse, BRT_BuildReportWindow.LayoutMinHeight30);
                GUILayout.Space(leftIndent);
                GUILayout.Label(logTypeIcon, textureStyle, BRT_BuildReportWindow.Layout20x16);
                GUILayout.Label(messages[m].Message, logMessageStyleToUse, BRT_BuildReportWindow.LayoutNone);

                if (this._showLogMessagesCollapsed && messages[m].Count > 0)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(messages[m].Count.ToString(), logCountStyle, BRT_BuildReportWindow.LayoutNoExpandWidth);
                }

                GUILayout.EndHorizontal();

                ++messagesShown;

                var logMsgIdx = MakeLogMsgIdx(stepIdx, m);
                if (Event.current.type == EventType.Repaint)
                {
                    if (this._logRects.ContainsKey(logMsgIdx))
                        this._logRects[logMsgIdx] = GUILayoutUtility.GetLastRect();
                    else
                        this._logRects.Add(logMsgIdx, GUILayoutUtility.GetLastRect());
                }

                if (this._logRects.ContainsKey(logMsgIdx) && this._logRects[logMsgIdx].Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    requestRepaint = true;
                    this.SelectLogMessage(stepIdx, m);

                    if (Event.current.clickCount == 2)
                    {
                        if (messages[m].Message.StartsWith("Script attached to '"))
                            SearchPrefabFromLog(messages[m].Message);
                        else
                            OpenScriptFromLog(messages[m].Message);
                    }
                }
                useAlt = !useAlt;
            }
        }

        private void SelectStep(int stepIdx, BuildProcessStep[] steps)
        {
            this._selectedStepIdx = stepIdx;

            // count info, warn, and error messages
            this._infoMessageCount  = 0;
            this._warnMessageCount  = 0;
            this._errorMessageCount = 0;

            this._collapsedInfoMessageCount  = 0;
            this._collapsedWarnMessageCount  = 0;
            this._collapsedErrorMessageCount = 0;

            if (this._selectedStepIdx > -1 && steps[this._selectedStepIdx].BuildLogMessages != null && steps[this._selectedStepIdx].BuildLogMessages.Length > 0)
            {
                this._infoMessageCount  = steps[this._selectedStepIdx].InfoLogCount;
                this._warnMessageCount  = steps[this._selectedStepIdx].WarnLogCount;
                this._errorMessageCount = steps[this._selectedStepIdx].ErrorLogCount;

                this._collapsedInfoMessageCount  = steps[this._selectedStepIdx].CollapsedInfoLogCount;
                this._collapsedWarnMessageCount  = steps[this._selectedStepIdx].CollapsedWarnLogCount;
                this._collapsedErrorMessageCount = steps[this._selectedStepIdx].CollapsedErrorLogCount;
            }
            else
            {
                for (var i = 0; i < steps.Length; ++i)
                {
                    this._infoMessageCount  += steps[i].InfoLogCount;
                    this._warnMessageCount  += steps[i].WarnLogCount;
                    this._errorMessageCount += steps[i].ErrorLogCount;

                    this._collapsedInfoMessageCount  += steps[i].CollapsedInfoLogCount;
                    this._collapsedWarnMessageCount  += steps[i].CollapsedWarnLogCount;
                    this._collapsedErrorMessageCount += steps[i].CollapsedErrorLogCount;
                }
            }

            this.RefreshTotalVisibleMessageCount();

            this._logFilterLabel.text   = this._infoMessageCount.ToString();
            this._warnFilterLabel.text  = this._warnMessageCount.ToString();
            this._errorFilterLabel.text = this._errorMessageCount.ToString();
        }

        private void RefreshTotalVisibleMessageCount()
        {
            this._totalVisibleMessageCount = 0;

            if (this._showLogMessages)
            {
                if (this._showLogMessagesCollapsed)
                    this._totalVisibleMessageCount += this._collapsedInfoMessageCount;
                else
                    this._totalVisibleMessageCount += this._infoMessageCount;
            }
            if (this._showWarnMessages)
            {
                if (this._showLogMessagesCollapsed)
                    this._totalVisibleMessageCount += this._collapsedWarnMessageCount;
                else
                    this._totalVisibleMessageCount += this._warnMessageCount;
            }
            if (this._showErrorMessages)
            {
                if (this._showLogMessagesCollapsed)
                    this._totalVisibleMessageCount += this._collapsedErrorMessageCount;
                else
                    this._totalVisibleMessageCount += this._errorMessageCount;
            }

            // ------------------------

            if (this._logMessageToShowStartOffset > this._totalVisibleMessageCount)
            {
                var messagePaginationLength = BuildReportTool.Options.LogMessagePaginationLength;
                this._logMessageToShowStartOffset = messagePaginationLength * (this._totalVisibleMessageCount / messagePaginationLength);
            }
        }

        private void SelectLogMessage(int stepIdx, int logMessageIdx)
        {
            this._selectedLogStepIdx = stepIdx;
            this._selectedLogIdx     = logMessageIdx;
        }

        private Texture2D GetLogIcon(string logType)
        {
            if (logType.Contains("Warn"))
                return this._warnIcon;
            else if (logType.Contains("Log"))
                return this._logIcon;
            else
                return this._errorIcon;
        }

        private static void OpenScriptFromLog(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            var lineNumIdx = message.IndexOf(".cs(", StringComparison.OrdinalIgnoreCase);
            if (lineNumIdx < 0) return;
            lineNumIdx += 4;
            var lineNumEndIdx = message.IndexOf(",", lineNumIdx, StringComparison.OrdinalIgnoreCase);

            var filename    = message.Substring(0, lineNumIdx - 1);
            var lineNumText = message.Substring(lineNumIdx, lineNumEndIdx - lineNumIdx);
            //Debug.Log(string.Format("filename: {0} lineNumText: {1}", filename, lineNumText));

            var line = int.Parse(lineNumText);
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filename, line);
        }

        private static void SearchPrefabFromLog(string message)
        {
            if (!message.StartsWith("Script attached to '")) return;

            var lastQuote = message.IndexOf("'", 20, StringComparison.OrdinalIgnoreCase);
            if (lastQuote > -1)
            {
                var prefabName = message.Substring(20, lastQuote - 20);
                //Debug.Log(prefabName);
                SearchPrefab(prefabName);
            }
        }

        /// <summary>
        /// <see cref="UnityEditor.ProjectBrowser"/>
        /// </summary>
        private static readonly Type ProjectBrowserType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");

        /// <summary>
        /// <see cref="UnityEditor.ProjectBrowser.SetSearch(string)"/>
        /// </summary>
        private static readonly MethodInfo ProjectBrowserSetSearchMethod = ProjectBrowserType.GetMethod("SetSearch",
            BindingFlags.Public | BindingFlags.Instance,
            null,
            CallingConventions.Any,
            new[] { typeof(string) },
            null);

        /// <summary>
        /// <see cref="UnityEditor.ProjectBrowser.SelectAll()"/>
        /// </summary>
        private static readonly MethodInfo ProjectBrowserSelectAllMethod = ProjectBrowserType.GetMethod("SelectAll",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static void SearchPrefab(string prefabName)
        {
            if (ProjectBrowserType == null) return;

            var projectWindow = EditorWindow.GetWindow(ProjectBrowserType, false, "Project", true);
            if (projectWindow == null) return;

            if (ProjectBrowserSetSearchMethod == null) return;
            ProjectBrowserSetSearchMethod.Invoke(projectWindow, new object[] { prefabName });

            if (ProjectBrowserSelectAllMethod != null) ProjectBrowserSelectAllMethod.Invoke(projectWindow, null);
        }
    }
}