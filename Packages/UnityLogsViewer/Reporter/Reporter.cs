namespace UITemplate.Packages.UnityLogsViewer.Reporter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.SceneManagement;

    [Serializable]
    public class Images
    {
        public Texture2D clearImage;
        public Texture2D collapseImage;
        public Texture2D clearOnNewSceneImage;
        public Texture2D showTimeImage;
        public Texture2D showSceneImage;
        public Texture2D userImage;
        public Texture2D showMemoryImage;
        public Texture2D softwareImage;
        public Texture2D dateImage;
        public Texture2D showFpsImage;
        public Texture2D infoImage;
        public Texture2D saveLogsImage;
        public Texture2D searchImage;
        public Texture2D copyImage;
        public Texture2D closeImage;

        public Texture2D buildFromImage;
        public Texture2D systemInfoImage;
        public Texture2D graphicsInfoImage;
        public Texture2D backImage;

        public Texture2D logImage;
        public Texture2D warningImage;
        public Texture2D errorImage;

        public Texture2D barImage;
        public Texture2D button_activeImage;
        public Texture2D even_logImage;
        public Texture2D odd_logImage;
        public Texture2D selectedImage;

        public GUISkin reporterScrollerSkin;
    }

    //To use Reporter just create reporter from menu (Reporter->Create) at first scene your game start.
    //then set the ” Scrip execution order ” in (Edit -> Project Settings ) of Reporter.cs to be the highest.

    //Now to view logs all what you have to do is to make a circle gesture using your mouse (click and drag) 
    //or your finger (touch and drag) on the screen to show all these logs
    //no coding is required 

    public class Reporter : MonoBehaviour
    {
        public enum _LogType
        {
            Assert    = LogType.Assert,
            Error     = LogType.Error,
            Exception = LogType.Exception,
            Log       = LogType.Log,
            Warning   = LogType.Warning,
        }

        public class Sample
        {
            public float  time;
            public byte   loadedScene;
            public float  memory;
            public float  fps;
            public string fpsText;

            public static float MemSize()
            {
                float s = sizeof(float) + sizeof(byte) + sizeof(float) + sizeof(float);
                return s;
            }

            public string GetSceneName()
            {
                if (this.loadedScene == 255)
                    return "AssetBundleScene";

                return scenes[this.loadedScene];
            }
        }

        List<Sample> samples = new List<Sample>();

        public class Log
        {
            public int      count = 1;
            public _LogType logType;
            public string   condition;
            public string   stacktrace;

            public int sampleId;
            //public string   objectName="" ;//object who send error
            //public string   rootName =""; //root of object send error

            public Log CreateCopy() { return (Log)this.MemberwiseClone(); }

            public float GetMemoryUsage()
            {
                return (float)(sizeof(int) +
                               sizeof(_LogType) +
                               this.condition.Length * sizeof(char) +
                               this.stacktrace.Length * sizeof(char) +
                               sizeof(int));
            }
        }

        //contains all uncollapsed log
        List<Log> logs = new List<Log>();

        //contains all collapsed logs
        List<Log> collapsedLogs = new List<Log>();

        //contain logs which should only appear to user , for example if you switch off show logs + switch off show warnings
        //and your mode is collapse,then this list will contains only collapsed errors
        List<Log> currentLog = new List<Log>();

        //used to check if the new coming logs is already exist or new one
        MultiKeyDictionary<string, string, Log> logsDic = new MultiKeyDictionary<string, string, Log>();

        //to save memory
        Dictionary<string, string> cachedString = new Dictionary<string, string>();

        [HideInInspector]
        //show hide In Game Logs
        public bool show = false;

        //collapse logs
        bool collapse;

        //to decide if you want to clean logs for new loaded scene
        bool clearOnNewSceneLoaded;

        bool showTime;

        bool showScene;

        bool showMemory;

        bool showFps;

        bool showGraph;

        //show or hide logs
        bool showLog = true;

        //show or hide warnings
        bool showWarning = true;

        //show or hide errors
        bool showError = true;

        //total number of logs
        int numOfLogs = 0;

        //total number of warnings
        int numOfLogsWarning = 0;

        //total number of errors
        int numOfLogsError = 0;

        //total number of collapsed logs
        int numOfCollapsedLogs = 0;

        //total number of collapsed warnings
        int numOfCollapsedLogsWarning = 0;

        //total number of collapsed errors
        int numOfCollapsedLogsError = 0;

        //maximum number of allowed logs to view
        //public int maxAllowedLog = 1000 ;

        bool showClearOnNewSceneLoadedButton = true;
        bool showTimeButton                  = true;
        bool showSceneButton                 = true;
        bool showMemButton                   = true;
        bool showFpsButton                   = true;
        bool showSearchText                  = true;
        bool showCopyButton                  = true;
        bool showSaveButton                  = true;

        string       buildDate;
        string       logDate;
        float        logsMemUsage;
        float        graphMemUsage;
        public float TotalMemUsage { get { return this.logsMemUsage + this.graphMemUsage; } }
        float        gcTotalMemory;

        public string UserData = "";

        //frame rate per second
        public float  fps;
        public string fpsText;

        //List<Texture2D> snapshots = new List<Texture2D>() ;

        enum ReportView
        {
            None,
            Logs,
            Info,
            Snapshot,
        }

        ReportView currentView = ReportView.Logs;

        enum DetailView
        {
            None,
            StackTrace,
            Graph,
        }

        //used to check if you have In Game Logs multiple time in different scene
        //only one should work and other should be deleted
        static bool created = false;
        //public delegate void OnLogHandler( string condition, string stack-trace, LogType type );
        //public event OnLogHandler OnLog ;

        public Images images;

        // gui
        GUIContent clearContent;
        GUIContent collapseContent;
        GUIContent clearOnNewSceneContent;
        GUIContent showTimeContent;
        GUIContent showSceneContent;
        GUIContent userContent;
        GUIContent showMemoryContent;
        GUIContent softwareContent;
        GUIContent dateContent;

        GUIContent showFpsContent;

        //GUIContent graphContent;
        GUIContent infoContent;
        GUIContent saveLogsContent;
        GUIContent searchContent;
        GUIContent copyContent;
        GUIContent closeContent;

        GUIContent buildFromContent;
        GUIContent systemInfoContent;
        GUIContent graphicsInfoContent;
        GUIContent backContent;

        //GUIContent cameraContent;

        GUIContent logContent;
        GUIContent warningContent;
        GUIContent errorContent;
        GUIStyle   barStyle;
        GUIStyle   buttonActiveStyle;

        GUIStyle nonStyle;
        GUIStyle lowerLeftFontStyle;
        GUIStyle backStyle;
        GUIStyle evenLogStyle;
        GUIStyle oddLogStyle;
        GUIStyle logButtonStyle;
        GUIStyle selectedLogStyle;
        GUIStyle selectedLogFontStyle;
        GUIStyle stackLabelStyle;
        GUIStyle scrollerStyle;
        GUIStyle searchStyle;
        GUIStyle sliderBackStyle;
        GUIStyle sliderThumbStyle;
        GUISkin  toolbarScrollerSkin;
        GUISkin  logScrollerSkin;
        GUISkin  graphScrollerSkin;

        public Vector2  size              = new Vector2(32, 32);
        public float    maxSize           = 20;
        public int      numOfCircleToShow = 1;
        static string[] scenes;
        string          currentScene;
        string          filterText = "";

        string deviceModel;
        string deviceType;
        string deviceName;
        string graphicsMemorySize;
#if !UNITY_CHANGE1
        string maxTextureSize;
#endif
        string systemMemorySize;

        void Awake()
        {
            if (!this.Initialized)
                this.Initialize();

#if UNITY_CHANGE3
            SceneManager.sceneLoaded += this._OnLevelWasLoaded;
#endif
        }

        private void OnDestroy()
        {
#if UNITY_CHANGE3
            SceneManager.sceneLoaded -= this._OnLevelWasLoaded;
#endif
        }

        void OnEnable()
        {
            if (this.logs.Count == 0) //if recompile while in play mode
                this.clear();
        }

        void OnDisable() { }

        void addSample()
        {
            Sample sample = new Sample();
            sample.fps     = this.fps;
            sample.fpsText = this.fpsText;
#if UNITY_CHANGE3
            sample.loadedScene = (byte)SceneManager.GetActiveScene().buildIndex;
#else
		sample.loadedScene = (byte)Application.loadedLevel;
#endif
            sample.time   = Time.realtimeSinceStartup;
            sample.memory = this.gcTotalMemory;
            this.samples.Add(sample);

            this.graphMemUsage = (this.samples.Count * Sample.MemSize()) / 1024 / 1024;
        }

        public bool Initialized = false;

        public void Initialize()
        {
            if (!created)
            {
                try
                {
                    this.gameObject.SendMessage("OnPreStart");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
#if UNITY_CHANGE3
                scenes            = new string[SceneManager.sceneCountInBuildSettings];
                this.currentScene = SceneManager.GetActiveScene().name;
#else
			scenes = new string[Application.levelCount];
			currentScene = Application.loadedLevelName;
#endif
                DontDestroyOnLoad(this.gameObject);
#if UNITY_CHANGE1
			Application.RegisterLogCallback (new Application.LogCallback (CaptureLog));
			Application.RegisterLogCallbackThreaded (new Application.LogCallback (CaptureLogThread));
#else
                //Application.logMessageReceived += CaptureLog ;
                Application.logMessageReceivedThreaded += this.CaptureLogThread;
#endif
                created = true;
                //addSample();
            }
            else
            {
                Debug.LogWarning("tow manager is exists delete the second");
                DestroyImmediate(this.gameObject, true);
                return;
            }

            //initialize gui and styles for gui purpose

            this.clearContent           = new GUIContent("", this.images.clearImage, "Clear logs");
            this.collapseContent        = new GUIContent("", this.images.collapseImage, "Collapse logs");
            this.clearOnNewSceneContent = new GUIContent("", this.images.clearOnNewSceneImage, "Clear logs on new scene loaded");
            this.showTimeContent        = new GUIContent("", this.images.showTimeImage, "Show Hide Time");
            this.showSceneContent       = new GUIContent("", this.images.showSceneImage, "Show Hide Scene");
            this.showMemoryContent      = new GUIContent("", this.images.showMemoryImage, "Show Hide Memory");
            this.softwareContent        = new GUIContent("", this.images.softwareImage, "Software");
            this.dateContent            = new GUIContent("", this.images.dateImage, "Date");
            this.showFpsContent         = new GUIContent("", this.images.showFpsImage, "Show Hide fps");
            this.infoContent            = new GUIContent("", this.images.infoImage, "Information about application");
            this.saveLogsContent        = new GUIContent("", this.images.saveLogsImage, "Save logs to device");
            this.searchContent          = new GUIContent("", this.images.searchImage, "Search for logs");
            this.copyContent            = new GUIContent("", this.images.copyImage, "Copy log to clipboard");
            this.closeContent           = new GUIContent("", this.images.closeImage, "Hide logs");
            this.userContent            = new GUIContent("", this.images.userImage, "User");

            this.buildFromContent    = new GUIContent("", this.images.buildFromImage, "Build From");
            this.systemInfoContent   = new GUIContent("", this.images.systemInfoImage, "System Info");
            this.graphicsInfoContent = new GUIContent("", this.images.graphicsInfoImage, "Graphics Info");
            this.backContent         = new GUIContent("", this.images.backImage, "Back");

            //snapshotContent = new GUIContent("",images.cameraImage,"show or hide logs");
            this.logContent     = new GUIContent("", this.images.logImage, "show or hide logs");
            this.warningContent = new GUIContent("", this.images.warningImage, "show or hide warnings");
            this.errorContent   = new GUIContent("", this.images.errorImage, "show or hide errors");

            this.currentView           = (ReportView)PlayerPrefs.GetInt("Reporter_currentView", 1);
            this.show                  = (PlayerPrefs.GetInt("Reporter_show") == 1) ? true : false;
            this.collapse              = (PlayerPrefs.GetInt("Reporter_collapse") == 1) ? true : false;
            this.clearOnNewSceneLoaded = (PlayerPrefs.GetInt("Reporter_clearOnNewSceneLoaded") == 1) ? true : false;
            this.showTime              = (PlayerPrefs.GetInt("Reporter_showTime") == 1) ? true : false;
            this.showScene             = (PlayerPrefs.GetInt("Reporter_showScene") == 1) ? true : false;
            this.showMemory            = (PlayerPrefs.GetInt("Reporter_showMemory") == 1) ? true : false;
            this.showFps               = (PlayerPrefs.GetInt("Reporter_showFps") == 1) ? true : false;
            this.showGraph             = (PlayerPrefs.GetInt("Reporter_showGraph") == 1) ? true : false;
            this.showLog               = (PlayerPrefs.GetInt("Reporter_showLog", 1) == 1) ? true : false;
            this.showWarning           = (PlayerPrefs.GetInt("Reporter_showWarning", 1) == 1) ? true : false;
            this.showError             = (PlayerPrefs.GetInt("Reporter_showError", 1) == 1) ? true : false;
            this.filterText            = PlayerPrefs.GetString("Reporter_filterText");
            this.size.x                = this.size.y = PlayerPrefs.GetFloat("Reporter_size", 32);

            this.showClearOnNewSceneLoadedButton = (PlayerPrefs.GetInt("Reporter_showClearOnNewSceneLoadedButton", 1) == 1) ? true : false;
            this.showTimeButton                  = (PlayerPrefs.GetInt("Reporter_showTimeButton", 1) == 1) ? true : false;
            this.showSceneButton                 = (PlayerPrefs.GetInt("Reporter_showSceneButton", 1) == 1) ? true : false;
            this.showMemButton                   = (PlayerPrefs.GetInt("Reporter_showMemButton", 1) == 1) ? true : false;
            this.showFpsButton                   = (PlayerPrefs.GetInt("Reporter_showFpsButton", 1) == 1) ? true : false;
            this.showSearchText                  = (PlayerPrefs.GetInt("Reporter_showSearchText", 1) == 1) ? true : false;
            this.showCopyButton                  = (PlayerPrefs.GetInt("Reporter_showCopyButton", 1) == 1) ? true : false;
            this.showSaveButton                  = (PlayerPrefs.GetInt("Reporter_showSaveButton", 1) == 1) ? true : false;

            this.initializeStyle();

            this.Initialized = true;

            if (this.show)
            {
                this.doShow();
            }

            this.deviceModel        = SystemInfo.deviceModel.ToString();
            this.deviceType         = SystemInfo.deviceType.ToString();
            this.deviceName         = SystemInfo.deviceName.ToString();
            this.graphicsMemorySize = SystemInfo.graphicsMemorySize.ToString();
#if !UNITY_CHANGE1
            this.maxTextureSize = SystemInfo.maxTextureSize.ToString();
#endif
            this.systemMemorySize = SystemInfo.systemMemorySize.ToString();
        }

        void initializeStyle()
        {
            int paddingX = (int)(this.size.x * 0.2f);
            int paddingY = (int)(this.size.y * 0.2f);
            this.nonStyle                   = new GUIStyle();
            this.nonStyle.clipping          = TextClipping.Clip;
            this.nonStyle.border            = new RectOffset(0, 0, 0, 0);
            this.nonStyle.normal.background = null;
            this.nonStyle.fontSize          = (int)(this.size.y / 2);
            this.nonStyle.alignment         = TextAnchor.MiddleCenter;

            this.lowerLeftFontStyle                   = new GUIStyle();
            this.lowerLeftFontStyle.clipping          = TextClipping.Clip;
            this.lowerLeftFontStyle.border            = new RectOffset(0, 0, 0, 0);
            this.lowerLeftFontStyle.normal.background = null;
            this.lowerLeftFontStyle.fontSize          = (int)(this.size.y / 2);
            this.lowerLeftFontStyle.fontStyle         = FontStyle.Bold;
            this.lowerLeftFontStyle.alignment         = TextAnchor.LowerLeft;

            this.barStyle                   = new GUIStyle();
            this.barStyle.border            = new RectOffset(1, 1, 1, 1);
            this.barStyle.normal.background = this.images.barImage;
            this.barStyle.active.background = this.images.button_activeImage;
            this.barStyle.alignment         = TextAnchor.MiddleCenter;
            this.barStyle.margin            = new RectOffset(1, 1, 1, 1);

            //barStyle.padding = new RectOffset(paddingX,paddingX,paddingY,paddingY); 
            //barStyle.wordWrap = true ;
            this.barStyle.clipping = TextClipping.Clip;
            this.barStyle.fontSize = (int)(this.size.y / 2);

            this.buttonActiveStyle                   = new GUIStyle();
            this.buttonActiveStyle.border            = new RectOffset(1, 1, 1, 1);
            this.buttonActiveStyle.normal.background = this.images.button_activeImage;
            this.buttonActiveStyle.alignment         = TextAnchor.MiddleCenter;
            this.buttonActiveStyle.margin            = new RectOffset(1, 1, 1, 1);
            //buttonActiveStyle.padding = new RectOffset(4,4,4,4);
            this.buttonActiveStyle.fontSize = (int)(this.size.y / 2);

            this.backStyle                   = new GUIStyle();
            this.backStyle.normal.background = this.images.even_logImage;
            this.backStyle.clipping          = TextClipping.Clip;
            this.backStyle.fontSize          = (int)(this.size.y / 2);

            this.evenLogStyle                   = new GUIStyle();
            this.evenLogStyle.normal.background = this.images.even_logImage;
            this.evenLogStyle.fixedHeight       = this.size.y;
            this.evenLogStyle.clipping          = TextClipping.Clip;
            this.evenLogStyle.alignment         = TextAnchor.UpperLeft;
            this.evenLogStyle.imagePosition     = ImagePosition.ImageLeft;
            this.evenLogStyle.fontSize          = (int)(this.size.y / 2);
            //evenLogStyle.wordWrap = true;

            this.oddLogStyle                   = new GUIStyle();
            this.oddLogStyle.normal.background = this.images.odd_logImage;
            this.oddLogStyle.fixedHeight       = this.size.y;
            this.oddLogStyle.clipping          = TextClipping.Clip;
            this.oddLogStyle.alignment         = TextAnchor.UpperLeft;
            this.oddLogStyle.imagePosition     = ImagePosition.ImageLeft;
            this.oddLogStyle.fontSize          = (int)(this.size.y / 2);
            //oddLogStyle.wordWrap = true ;

            this.logButtonStyle = new GUIStyle();
            //logButtonStyle.wordWrap = true;
            this.logButtonStyle.fixedHeight = this.size.y;
            this.logButtonStyle.clipping    = TextClipping.Clip;
            this.logButtonStyle.alignment   = TextAnchor.UpperLeft;
            //logButtonStyle.imagePosition = ImagePosition.ImageLeft ;
            //logButtonStyle.wordWrap = true;
            this.logButtonStyle.fontSize = (int)(this.size.y / 2);
            this.logButtonStyle.padding  = new RectOffset(paddingX, paddingX, paddingY, paddingY);

            this.selectedLogStyle                   = new GUIStyle();
            this.selectedLogStyle.normal.background = this.images.selectedImage;
            this.selectedLogStyle.fixedHeight       = this.size.y;
            this.selectedLogStyle.clipping          = TextClipping.Clip;
            this.selectedLogStyle.alignment         = TextAnchor.UpperLeft;
            this.selectedLogStyle.normal.textColor  = Color.white;
            //selectedLogStyle.wordWrap = true;
            this.selectedLogStyle.fontSize = (int)(this.size.y / 2);

            this.selectedLogFontStyle                   = new GUIStyle();
            this.selectedLogFontStyle.normal.background = this.images.selectedImage;
            this.selectedLogFontStyle.fixedHeight       = this.size.y;
            this.selectedLogFontStyle.clipping          = TextClipping.Clip;
            this.selectedLogFontStyle.alignment         = TextAnchor.UpperLeft;
            this.selectedLogFontStyle.normal.textColor  = Color.white;
            //selectedLogStyle.wordWrap = true;
            this.selectedLogFontStyle.fontSize = (int)(this.size.y / 2);
            this.selectedLogFontStyle.padding  = new RectOffset(paddingX, paddingX, paddingY, paddingY);

            this.stackLabelStyle          = new GUIStyle();
            this.stackLabelStyle.wordWrap = true;
            this.stackLabelStyle.fontSize = (int)(this.size.y / 2);
            this.stackLabelStyle.padding  = new RectOffset(paddingX, paddingX, paddingY, paddingY);

            this.scrollerStyle                   = new GUIStyle();
            this.scrollerStyle.normal.background = this.images.barImage;

            this.searchStyle           = new GUIStyle();
            this.searchStyle.clipping  = TextClipping.Clip;
            this.searchStyle.alignment = TextAnchor.LowerCenter;
            this.searchStyle.fontSize  = (int)(this.size.y / 2);
            this.searchStyle.wordWrap  = true;

            this.sliderBackStyle                   = new GUIStyle();
            this.sliderBackStyle.normal.background = this.images.barImage;
            this.sliderBackStyle.fixedHeight       = this.size.y;
            this.sliderBackStyle.border            = new RectOffset(1, 1, 1, 1);

            this.sliderThumbStyle                   = new GUIStyle();
            this.sliderThumbStyle.normal.background = this.images.selectedImage;
            this.sliderThumbStyle.fixedWidth        = this.size.x;

            GUISkin skin = this.images.reporterScrollerSkin;

            this.toolbarScrollerSkin                                      = (GUISkin)Instantiate(skin);
            this.toolbarScrollerSkin.verticalScrollbar.fixedWidth         = 0f;
            this.toolbarScrollerSkin.horizontalScrollbar.fixedHeight      = 0f;
            this.toolbarScrollerSkin.verticalScrollbarThumb.fixedWidth    = 0f;
            this.toolbarScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

            this.logScrollerSkin                                      = (GUISkin)Instantiate(skin);
            this.logScrollerSkin.verticalScrollbar.fixedWidth         = this.size.x * 2f;
            this.logScrollerSkin.horizontalScrollbar.fixedHeight      = 0f;
            this.logScrollerSkin.verticalScrollbarThumb.fixedWidth    = this.size.x * 2f;
            this.logScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

            this.graphScrollerSkin                                      = (GUISkin)Instantiate(skin);
            this.graphScrollerSkin.verticalScrollbar.fixedWidth         = 0f;
            this.graphScrollerSkin.horizontalScrollbar.fixedHeight      = this.size.x * 2f;
            this.graphScrollerSkin.verticalScrollbarThumb.fixedWidth    = 0f;
            this.graphScrollerSkin.horizontalScrollbarThumb.fixedHeight = this.size.x * 2f;
            //inGameLogsScrollerSkin.verticalScrollbarThumb.fixedWidth = size.x * 2;
            //inGameLogsScrollerSkin.verticalScrollbar.fixedWidth = size.x * 2;
        }

        void Start()
        {
            this.logDate = DateTime.Now.ToString();
            this.StartCoroutine("readInfo");
        }

        //clear all logs
        void clear()
        {
            this.logs.Clear();
            this.collapsedLogs.Clear();
            this.currentLog.Clear();
            this.logsDic.Clear();
            //selectedIndex = -1;
            this.selectedLog               = null;
            this.numOfLogs                 = 0;
            this.numOfLogsWarning          = 0;
            this.numOfLogsError            = 0;
            this.numOfCollapsedLogs        = 0;
            this.numOfCollapsedLogsWarning = 0;
            this.numOfCollapsedLogsError   = 0;
            this.logsMemUsage              = 0;
            this.graphMemUsage             = 0;
            this.samples.Clear();
            GC.Collect();
            this.selectedLog = null;
        }

        Rect    screenRect   = Rect.zero;
        Rect    toolBarRect  = Rect.zero;
        Rect    logsRect     = Rect.zero;
        Rect    stackRect    = Rect.zero;
        Rect    graphRect    = Rect.zero;
        Rect    graphMinRect = Rect.zero;
        Rect    graphMaxRect = Rect.zero;
        Rect    buttomRect   = Rect.zero;
        Vector2 stackRectTopLeft;
        Rect    detailRect = Rect.zero;

        Vector2 scrollPosition;
        Vector2 scrollPosition2;
        Vector2 toolbarScrollPosition;

        //int 	selectedIndex = -1;
        Log selectedLog;

        float toolbarOldDrag = 0;
        float oldDrag;
        float oldDrag2;
        float oldDrag3;
        int   startIndex;

        //calculate what is the currentLog : collapsed or not , hide or view warnings ......
        void calculateCurrentLog()
        {
            bool   filter      = !string.IsNullOrEmpty(this.filterText);
            string _filterText = "";
            if (filter)
                _filterText = this.filterText.ToLower();
            this.currentLog.Clear();
            if (this.collapse)
            {
                for (int i = 0; i < this.collapsedLogs.Count; i++)
                {
                    Log log = this.collapsedLogs[i];
                    if (log.logType == _LogType.Log && !this.showLog)
                        continue;
                    if (log.logType == _LogType.Warning && !this.showWarning)
                        continue;
                    if (log.logType == _LogType.Error && !this.showError)
                        continue;
                    if (log.logType == _LogType.Assert && !this.showError)
                        continue;
                    if (log.logType == _LogType.Exception && !this.showError)
                        continue;

                    if (filter)
                    {
                        if (log.condition.ToLower().Contains(_filterText))
                            this.currentLog.Add(log);
                    }
                    else
                    {
                        this.currentLog.Add(log);
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.logs.Count; i++)
                {
                    Log log = this.logs[i];
                    if (log.logType == _LogType.Log && !this.showLog)
                        continue;
                    if (log.logType == _LogType.Warning && !this.showWarning)
                        continue;
                    if (log.logType == _LogType.Error && !this.showError)
                        continue;
                    if (log.logType == _LogType.Assert && !this.showError)
                        continue;
                    if (log.logType == _LogType.Exception && !this.showError)
                        continue;

                    if (filter)
                    {
                        if (log.condition.ToLower().Contains(_filterText))
                            this.currentLog.Add(log);
                    }
                    else
                    {
                        this.currentLog.Add(log);
                    }
                }
            }

            if (this.selectedLog != null)
            {
                int newSelectedIndex = this.currentLog.IndexOf(this.selectedLog);
                if (newSelectedIndex == -1)
                {
                    Log collapsedSelected = this.logsDic[this.selectedLog.condition][this.selectedLog.stacktrace];
                    newSelectedIndex = this.currentLog.IndexOf(collapsedSelected);
                    if (newSelectedIndex != -1)
                        this.scrollPosition.y = newSelectedIndex * this.size.y;
                }
                else
                {
                    this.scrollPosition.y = newSelectedIndex * this.size.y;
                }
            }
        }

        Rect       countRect       = Rect.zero;
        Rect       timeRect        = Rect.zero;
        Rect       timeLabelRect   = Rect.zero;
        Rect       sceneRect       = Rect.zero;
        Rect       sceneLabelRect  = Rect.zero;
        Rect       memoryRect      = Rect.zero;
        Rect       memoryLabelRect = Rect.zero;
        Rect       fpsRect         = Rect.zero;
        Rect       fpsLabelRect    = Rect.zero;
        GUIContent tempContent     = new GUIContent();

        Vector2 infoScrollPosition;
        Vector2 oldInfoDrag;

        void DrawInfo()
        {
            GUILayout.BeginArea(this.screenRect, this.backStyle);

            Vector2 drag = this.getDrag();
            if ((drag.x != 0) && (this.downPos != Vector2.zero))
            {
                this.infoScrollPosition.x -= (drag.x - this.oldInfoDrag.x);
            }

            if ((drag.y != 0) && (this.downPos != Vector2.zero))
            {
                this.infoScrollPosition.y += (drag.y - this.oldInfoDrag.y);
            }

            this.oldInfoDrag = drag;

            GUI.skin                = this.toolbarScrollerSkin;
            this.infoScrollPosition = GUILayout.BeginScrollView(this.infoScrollPosition);
            GUILayout.Space(this.size.x);
            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.buildFromContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(this.buildDate, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.systemInfoContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(this.deviceModel, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(this.deviceType, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(this.deviceName, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.graphicsInfoContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(SystemInfo.graphicsDeviceName, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(this.graphicsMemorySize, this.nonStyle, GUILayout.Height(this.size.y));
#if !UNITY_CHANGE1
            GUILayout.Space(this.size.x);
            GUILayout.Label(this.maxTextureSize, this.nonStyle, GUILayout.Height(this.size.y));
#endif
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Space(this.size.x);
            GUILayout.Space(this.size.x);
            GUILayout.Label("Screen Width " + Screen.width, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label("Screen Height " + Screen.height, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.showMemoryContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(this.systemMemorySize + " mb", this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Space(this.size.x);
            GUILayout.Space(this.size.x);
            GUILayout.Label("Mem Usage Of Logs " + this.logsMemUsage.ToString("0.000") + " mb", this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            //GUILayout.Label( "Mem Usage Of Graph " + graphMemUsage.ToString("0.000")  + " mb", nonStyle , GUILayout.Height(size.y));
            //GUILayout.Space( size.x);
            GUILayout.Label("GC Memory " + this.gcTotalMemory.ToString("0.000") + " mb", this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.softwareContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(SystemInfo.operatingSystem, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.dateContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(DateTime.Now.ToString(), this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.Label(" - Application Started At " + this.logDate, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.showTimeContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(Time.realtimeSinceStartup.ToString("000"), this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.showFpsContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(this.fpsText, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.userContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(this.UserData, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.showSceneContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label(this.currentScene, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Box(this.showSceneContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.Label("Unity Version = " + Application.unityVersion, this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            /*GUILayout.BeginHorizontal();
        GUILayout.Space( size.x);
        GUILayout.Box( graphContent ,nonStyle ,  GUILayout.Width(size.x) , GUILayout.Height(size.y));
        GUILayout.Space( size.x);
        GUILayout.Label( "frame " + samples.Count , nonStyle , GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();*/

            this.drawInfo_enableDisableToolBarButtons();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Label("Size = " + this.size.x.ToString("0.0"), this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            float _size = GUILayout.HorizontalSlider(this.size.x, 16, 64, this.sliderBackStyle, this.sliderThumbStyle, GUILayout.Width(Screen.width * 0.5f));
            if (this.size.x != _size)
            {
                this.size.x = this.size.y = _size;
                this.initializeStyle();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            if (GUILayout.Button(this.backContent, this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.currentView = ReportView.Logs;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        void drawInfo_enableDisableToolBarButtons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);
            GUILayout.Label("Hide or Show tool bar buttons", this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.Space(this.size.x);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(this.size.x);

            if (GUILayout.Button(this.clearOnNewSceneContent, (this.showClearOnNewSceneLoadedButton) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showClearOnNewSceneLoadedButton = !this.showClearOnNewSceneLoadedButton;
            }

            if (GUILayout.Button(this.showTimeContent, (this.showTimeButton) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showTimeButton = !this.showTimeButton;
            }

            this.tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(this.tempRect, Time.realtimeSinceStartup.ToString("0.0"), this.lowerLeftFontStyle);
            if (GUILayout.Button(this.showSceneContent, (this.showSceneButton) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showSceneButton = !this.showSceneButton;
            }

            this.tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(this.tempRect, this.currentScene, this.lowerLeftFontStyle);
            if (GUILayout.Button(this.showMemoryContent, (this.showMemButton) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showMemButton = !this.showMemButton;
            }

            this.tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(this.tempRect, this.gcTotalMemory.ToString("0.0"), this.lowerLeftFontStyle);

            if (GUILayout.Button(this.showFpsContent, (this.showFpsButton) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showFpsButton = !this.showFpsButton;
            }

            this.tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(this.tempRect, this.fpsText, this.lowerLeftFontStyle);
            /*if( GUILayout.Button( graphContent , (showGraph)?buttonActiveStyle:barStyle , GUILayout.Width(size.x*2) ,GUILayout.Height(size.y*2)))
        {
            showGraph = !showGraph ;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label( tempRect , samples.Count.ToString() , lowerLeftFontStyle );*/
            if (GUILayout.Button(this.searchContent, (this.showSearchText) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showSearchText = !this.showSearchText;
            }

            if (GUILayout.Button(this.copyContent, (this.showCopyButton) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showCopyButton = !this.showCopyButton;
            }

            if (GUILayout.Button(this.saveLogsContent, (this.showSaveButton) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showSaveButton = !this.showSaveButton;
            }

            this.tempRect = GUILayoutUtility.GetLastRect();
            GUI.TextField(this.tempRect, this.filterText, this.searchStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void DrawReport()
        {
            this.screenRect.x      = 0f;
            this.screenRect.y      = 0f;
            this.screenRect.width  = Screen.width;
            this.screenRect.height = Screen.height;
            GUILayout.BeginArea(this.screenRect, this.backStyle);
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            /*GUILayout.Box( cameraContent ,nonStyle ,  GUILayout.Width(size.x) , GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();*/
            GUILayout.Label("Select Photo", this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Coming Soon", this.nonStyle, GUILayout.Height(this.size.y));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(this.backContent, this.barStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y)))
            {
                this.currentView = ReportView.Logs;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        void drawToolBar()
        {
            var safeAreaHeight = Screen.height - Screen.safeArea.height;

            this.toolBarRect.x      = 0f;
            this.toolBarRect.y      = safeAreaHeight;
            this.toolBarRect.width  = Screen.width;
            this.toolBarRect.height = this.size.y * 2f;

            //toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
            //toolbarScrollerSkin.horizontalScrollbar.fixedHeight= 0f  ;

            GUI.skin = this.toolbarScrollerSkin;
            Vector2 drag = this.getDrag();
            if ((drag.x != 0) && (this.downPos != Vector2.zero) && (this.downPos.y > Screen.height - this.size.y * 2f))
            {
                this.toolbarScrollPosition.x -= (drag.x - this.toolbarOldDrag);
            }

            this.toolbarOldDrag = drag.x;
            GUILayout.BeginArea(this.toolBarRect);
            this.toolbarScrollPosition = GUILayout.BeginScrollView(this.toolbarScrollPosition);
            GUILayout.BeginHorizontal(this.barStyle);

            if (GUILayout.Button(this.clearContent, this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.clear();
            }

            if (GUILayout.Button(this.collapseContent, (this.collapse) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.collapse = !this.collapse;
                this.calculateCurrentLog();
            }

            if (this.showClearOnNewSceneLoadedButton &&
                GUILayout.Button(this.clearOnNewSceneContent, (this.clearOnNewSceneLoaded) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.clearOnNewSceneLoaded = !this.clearOnNewSceneLoaded;
            }

            if (this.showTimeButton && GUILayout.Button(this.showTimeContent, (this.showTime) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showTime = !this.showTime;
            }

            if (this.showSceneButton)
            {
                this.tempRect = GUILayoutUtility.GetLastRect();
                GUI.Label(this.tempRect, Time.realtimeSinceStartup.ToString("0.0"), this.lowerLeftFontStyle);
                if (GUILayout.Button(this.showSceneContent, (this.showScene) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
                {
                    this.showScene = !this.showScene;
                }

                this.tempRect = GUILayoutUtility.GetLastRect();
                GUI.Label(this.tempRect, this.currentScene, this.lowerLeftFontStyle);
            }

            if (this.showMemButton)
            {
                if (GUILayout.Button(this.showMemoryContent, (this.showMemory) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
                {
                    this.showMemory = !this.showMemory;
                }

                this.tempRect = GUILayoutUtility.GetLastRect();
                GUI.Label(this.tempRect, this.gcTotalMemory.ToString("0.0"), this.lowerLeftFontStyle);
            }

            if (this.showFpsButton)
            {
                if (GUILayout.Button(this.showFpsContent, (this.showFps) ? this.buttonActiveStyle : this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
                {
                    this.showFps = !this.showFps;
                }

                this.tempRect = GUILayoutUtility.GetLastRect();
                GUI.Label(this.tempRect, this.fpsText, this.lowerLeftFontStyle);
            }
            /*if( GUILayout.Button( graphContent , (showGraph)?buttonActiveStyle:barStyle , GUILayout.Width(size.x*2) ,GUILayout.Height(size.y*2)))
        {
            showGraph = !showGraph ;
        }
        tempRect = GUILayoutUtility.GetLastRect();
        GUI.Label( tempRect , samples.Count.ToString() , lowerLeftFontStyle );*/

            if (this.showSearchText)
            {
                GUILayout.Box(this.searchContent, this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2));
                this.tempRect = GUILayoutUtility.GetLastRect();
                string newFilterText = GUI.TextField(this.tempRect, this.filterText, this.searchStyle);
                if (newFilterText != this.filterText)
                {
                    this.filterText = newFilterText;
                    this.calculateCurrentLog();
                }
            }

            if (this.showCopyButton)
            {
                if (GUILayout.Button(this.copyContent, this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
                {
                    if (this.selectedLog == null)
                        GUIUtility.systemCopyBuffer = "No log selected";
                    else
                        GUIUtility.systemCopyBuffer = this.selectedLog.condition + Environment.NewLine + Environment.NewLine + this.selectedLog.stacktrace;
                }
            }

            if (this.showSaveButton)
            {
                if (GUILayout.Button(this.saveLogsContent, this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
                {
                    this.SaveLogsToDevice();
                }
            }

            if (GUILayout.Button(this.infoContent, this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.currentView = ReportView.Info;
            }

            GUILayout.FlexibleSpace();

            string logsText = " ";
            if (this.collapse)
            {
                logsText += this.numOfCollapsedLogs;
            }
            else
            {
                logsText += this.numOfLogs;
            }

            string logsWarningText = " ";
            if (this.collapse)
            {
                logsWarningText += this.numOfCollapsedLogsWarning;
            }
            else
            {
                logsWarningText += this.numOfLogsWarning;
            }

            string logsErrorText = " ";
            if (this.collapse)
            {
                logsErrorText += this.numOfCollapsedLogsError;
            }
            else
            {
                logsErrorText += this.numOfLogsError;
            }

            GUILayout.BeginHorizontal((this.showLog) ? this.buttonActiveStyle : this.barStyle);
            if (GUILayout.Button(this.logContent, this.nonStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showLog = !this.showLog;
                this.calculateCurrentLog();
            }

            if (GUILayout.Button(logsText, this.nonStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showLog = !this.showLog;
                this.calculateCurrentLog();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal((this.showWarning) ? this.buttonActiveStyle : this.barStyle);
            if (GUILayout.Button(this.warningContent, this.nonStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showWarning = !this.showWarning;
                this.calculateCurrentLog();
            }

            if (GUILayout.Button(logsWarningText, this.nonStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showWarning = !this.showWarning;
                this.calculateCurrentLog();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal((this.showError) ? this.buttonActiveStyle : this.nonStyle);
            if (GUILayout.Button(this.errorContent, this.nonStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showError = !this.showError;
                this.calculateCurrentLog();
            }

            if (GUILayout.Button(logsErrorText, this.nonStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.showError = !this.showError;
                this.calculateCurrentLog();
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button(this.closeContent, this.barStyle, GUILayout.Width(this.size.x * 2), GUILayout.Height(this.size.y * 2)))
            {
                this.show = false;
                ReporterGUI gui = this.gameObject.GetComponent<ReporterGUI>();
                DestroyImmediate(gui);

                try
                {
                    this.gameObject.SendMessage("OnHideReporter");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        Rect tempRect;

        void DrawLogs()
        {
            GUILayout.BeginArea(this.logsRect, this.backStyle);

            GUI.skin = this.logScrollerSkin;
            //setStartPos();
            Vector2 drag = this.getDrag();

            if (drag.y != 0 && this.logsRect.Contains(new Vector2(this.downPos.x, Screen.height - this.downPos.y)))
            {
                this.scrollPosition.y += (drag.y - this.oldDrag);
            }

            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);

            this.oldDrag = drag.y;

            int totalVisibleCount = (int)(Screen.height * 0.75f / this.size.y);
            int totalCount        = this.currentLog.Count;
            /*if( totalCount < 100 )
            inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 0;
        else
            inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 64;*/

            totalVisibleCount = Mathf.Min(totalVisibleCount, totalCount - this.startIndex);
            int index        = 0;
            int beforeHeight = (int)(this.startIndex * this.size.y);
            //selectedIndex = Mathf.Clamp( selectedIndex , -1 , totalCount -1);
            if (beforeHeight > 0)
            {
                //fill invisible gap before scroller to make proper scroller pos
                GUILayout.BeginHorizontal(GUILayout.Height(beforeHeight));
                GUILayout.Label("---");
                GUILayout.EndHorizontal();
            }

            int endIndex = this.startIndex + totalVisibleCount;
            endIndex = Mathf.Clamp(endIndex, 0, totalCount);
            bool scrollerVisible = (totalVisibleCount < totalCount);
            for (int i = this.startIndex; (this.startIndex + index) < endIndex; i++)
            {
                if (i >= this.currentLog.Count)
                    break;
                Log log = this.currentLog[i];

                if (log.logType == _LogType.Log && !this.showLog)
                    continue;
                if (log.logType == _LogType.Warning && !this.showWarning)
                    continue;
                if (log.logType == _LogType.Error && !this.showError)
                    continue;
                if (log.logType == _LogType.Assert && !this.showError)
                    continue;
                if (log.logType == _LogType.Exception && !this.showError)
                    continue;

                if (index >= totalVisibleCount)
                {
                    break;
                }

                GUIContent content = null;
                if (log.logType == _LogType.Log)
                    content = this.logContent;
                else if (log.logType == _LogType.Warning)
                    content = this.warningContent;
                else
                    content = this.errorContent;
                //content.text = log.condition ;

                GUIStyle currentLogStyle = ((this.startIndex + index) % 2 == 0) ? this.evenLogStyle : this.oddLogStyle;
                if (log == this.selectedLog)
                {
                    //selectedLog = log ;
                    currentLogStyle = this.selectedLogStyle;
                }
                else
                {
                }

                this.tempContent.text = log.count.ToString();
                float w = 0f;
                if (this.collapse)
                    w = this.barStyle.CalcSize(this.tempContent).x + 3;
                this.countRect.x = Screen.width - w;
                this.countRect.y = this.size.y * i;
                if (beforeHeight > 0)
                    this.countRect.y += 8; //i will check later why
                this.countRect.width  = w;
                this.countRect.height = this.size.y;

                if (scrollerVisible)
                    this.countRect.x -= this.size.x * 2;

                Sample sample = this.samples[log.sampleId];
                this.fpsRect = this.countRect;
                if (this.showFps)
                {
                    this.tempContent.text   =  sample.fpsText;
                    w                       =  currentLogStyle.CalcSize(this.tempContent).x + this.size.x;
                    this.fpsRect.x          -= w;
                    this.fpsRect.width      =  this.size.x;
                    this.fpsLabelRect       =  this.fpsRect;
                    this.fpsLabelRect.x     += this.size.x;
                    this.fpsLabelRect.width =  w - this.size.x;
                }

                this.memoryRect = this.fpsRect;
                if (this.showMemory)
                {
                    this.tempContent.text      =  sample.memory.ToString("0.000");
                    w                          =  currentLogStyle.CalcSize(this.tempContent).x + this.size.x;
                    this.memoryRect.x          -= w;
                    this.memoryRect.width      =  this.size.x;
                    this.memoryLabelRect       =  this.memoryRect;
                    this.memoryLabelRect.x     += this.size.x;
                    this.memoryLabelRect.width =  w - this.size.x;
                }

                this.sceneRect = this.memoryRect;
                if (this.showScene)
                {
                    this.tempContent.text     =  sample.GetSceneName();
                    w                         =  currentLogStyle.CalcSize(this.tempContent).x + this.size.x;
                    this.sceneRect.x          -= w;
                    this.sceneRect.width      =  this.size.x;
                    this.sceneLabelRect       =  this.sceneRect;
                    this.sceneLabelRect.x     += this.size.x;
                    this.sceneLabelRect.width =  w - this.size.x;
                }

                this.timeRect = this.sceneRect;
                if (this.showTime)
                {
                    this.tempContent.text    =  sample.time.ToString("0.000");
                    w                        =  currentLogStyle.CalcSize(this.tempContent).x + this.size.x;
                    this.timeRect.x          -= w;
                    this.timeRect.width      =  this.size.x;
                    this.timeLabelRect       =  this.timeRect;
                    this.timeLabelRect.x     += this.size.x;
                    this.timeLabelRect.width =  w - this.size.x;
                }

                GUILayout.BeginHorizontal(currentLogStyle);
                if (log == this.selectedLog)
                {
                    GUILayout.Box(content, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
                    GUILayout.Label(log.condition, this.selectedLogFontStyle);
                    //GUILayout.FlexibleSpace();
                    if (this.showTime)
                    {
                        GUI.Box(this.timeRect, this.showTimeContent, currentLogStyle);
                        GUI.Label(this.timeLabelRect, sample.time.ToString("0.000"), currentLogStyle);
                    }

                    if (this.showScene)
                    {
                        GUI.Box(this.sceneRect, this.showSceneContent, currentLogStyle);
                        GUI.Label(this.sceneLabelRect, sample.GetSceneName(), currentLogStyle);
                    }

                    if (this.showMemory)
                    {
                        GUI.Box(this.memoryRect, this.showMemoryContent, currentLogStyle);
                        GUI.Label(this.memoryLabelRect, sample.memory.ToString("0.000") + " mb", currentLogStyle);
                    }

                    if (this.showFps)
                    {
                        GUI.Box(this.fpsRect, this.showFpsContent, currentLogStyle);
                        GUI.Label(this.fpsLabelRect, sample.fpsText, currentLogStyle);
                    }
                }
                else
                {
                    if (GUILayout.Button(content, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y)))
                    {
                        //selectedIndex = startIndex + index ;
                        this.selectedLog = log;
                    }

                    if (GUILayout.Button(log.condition, this.logButtonStyle))
                    {
                        //selectedIndex = startIndex + index ;
                        this.selectedLog = log;
                    }

                    //GUILayout.FlexibleSpace();
                    if (this.showTime)
                    {
                        GUI.Box(this.timeRect, this.showTimeContent, currentLogStyle);
                        GUI.Label(this.timeLabelRect, sample.time.ToString("0.000"), currentLogStyle);
                    }

                    if (this.showScene)
                    {
                        GUI.Box(this.sceneRect, this.showSceneContent, currentLogStyle);
                        GUI.Label(this.sceneLabelRect, sample.GetSceneName(), currentLogStyle);
                    }

                    if (this.showMemory)
                    {
                        GUI.Box(this.memoryRect, this.showMemoryContent, currentLogStyle);
                        GUI.Label(this.memoryLabelRect, sample.memory.ToString("0.000") + " mb", currentLogStyle);
                    }

                    if (this.showFps)
                    {
                        GUI.Box(this.fpsRect, this.showFpsContent, currentLogStyle);
                        GUI.Label(this.fpsLabelRect, sample.fpsText, currentLogStyle);
                    }
                }

                if (this.collapse)
                    GUI.Label(this.countRect, log.count.ToString(), this.barStyle);
                GUILayout.EndHorizontal();
                index++;
            }

            int afterHeight = (int)((totalCount - (this.startIndex + totalVisibleCount)) * this.size.y);
            if (afterHeight > 0)
            {
                //fill invisible gap after scroller to make proper scroller pos
                GUILayout.BeginHorizontal(GUILayout.Height(afterHeight));
                GUILayout.Label(" ");
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            this.buttomRect.x      = 0f;
            this.buttomRect.y      = Screen.height - this.size.y;
            this.buttomRect.width  = Screen.width;
            this.buttomRect.height = this.size.y;

            if (this.showGraph)
                this.drawGraph();
            else
                this.drawStack();
        }

        float   graphSize    = 4f;
        int     startFrame   = 0;
        int     currentFrame = 0;
        Vector3 tempVector1;
        Vector3 tempVector2;
        Vector2 graphScrollerPos;
        float   maxFpsValue;
        float   minFpsValue;
        float   maxMemoryValue;
        float   minMemoryValue;

        void drawGraph()
        {
            this.graphRect        = this.stackRect;
            this.graphRect.height = Screen.height * 0.25f; //- size.y ;

            //startFrame = samples.Count - (int)(Screen.width / graphSize) ;
            //if( startFrame < 0 ) startFrame = 0 ;
            GUI.skin = this.graphScrollerSkin;

            Vector2 drag = this.getDrag();
            if (this.graphRect.Contains(new Vector2(this.downPos.x, Screen.height - this.downPos.y)))
            {
                if (drag.x != 0)
                {
                    this.graphScrollerPos.x -= drag.x - this.oldDrag3;
                    this.graphScrollerPos.x =  Mathf.Max(0, this.graphScrollerPos.x);
                }

                Vector2 p = this.downPos;
                if (p != Vector2.zero)
                {
                    this.currentFrame = this.startFrame + (int)(p.x / this.graphSize);
                }
            }

            this.oldDrag3 = drag.x;
            GUILayout.BeginArea(this.graphRect, this.backStyle);

            this.graphScrollerPos = GUILayout.BeginScrollView(this.graphScrollerPos);
            this.startFrame       = (int)(this.graphScrollerPos.x / this.graphSize);
            if (this.graphScrollerPos.x >= (this.samples.Count * this.graphSize - Screen.width))
                this.graphScrollerPos.x += this.graphSize;

            GUILayout.Label(" ", GUILayout.Width(this.samples.Count * this.graphSize));
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            this.maxFpsValue    = 0;
            this.minFpsValue    = 100000;
            this.maxMemoryValue = 0;
            this.minMemoryValue = 100000;
            for (int i = 0; i < Screen.width / this.graphSize; i++)
            {
                int index = this.startFrame + i;
                if (index >= this.samples.Count)
                    break;
                Sample s                                                = this.samples[index];
                if (this.maxFpsValue < s.fps) this.maxFpsValue          = s.fps;
                if (this.minFpsValue > s.fps) this.minFpsValue          = s.fps;
                if (this.maxMemoryValue < s.memory) this.maxMemoryValue = s.memory;
                if (this.minMemoryValue > s.memory) this.minMemoryValue = s.memory;
            }

            //GUI.BeginGroup(graphRect);

            if (this.currentFrame != -1 && this.currentFrame < this.samples.Count)
            {
                Sample selectedSample = this.samples[this.currentFrame];
                GUILayout.BeginArea(this.buttomRect, this.backStyle);
                GUILayout.BeginHorizontal();

                GUILayout.Box(this.showTimeContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
                GUILayout.Label(selectedSample.time.ToString("0.0"), this.nonStyle);
                GUILayout.Space(this.size.x);

                GUILayout.Box(this.showSceneContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
                GUILayout.Label(selectedSample.GetSceneName(), this.nonStyle);
                GUILayout.Space(this.size.x);

                GUILayout.Box(this.showMemoryContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
                GUILayout.Label(selectedSample.memory.ToString("0.000"), this.nonStyle);
                GUILayout.Space(this.size.x);

                GUILayout.Box(this.showFpsContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
                GUILayout.Label(selectedSample.fpsText, this.nonStyle);
                GUILayout.Space(this.size.x);

                /*GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
            GUILayout.Label( currentFrame.ToString() ,nonStyle  );*/
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            this.graphMaxRect        = this.stackRect;
            this.graphMaxRect.height = this.size.y;
            GUILayout.BeginArea(this.graphMaxRect);
            GUILayout.BeginHorizontal();

            GUILayout.Box(this.showMemoryContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Label(this.maxMemoryValue.ToString("0.000"), this.nonStyle);

            GUILayout.Box(this.showFpsContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
            GUILayout.Label(this.maxFpsValue.ToString("0.000"), this.nonStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            this.graphMinRect        = this.stackRect;
            this.graphMinRect.y      = this.stackRect.y + this.stackRect.height - this.size.y;
            this.graphMinRect.height = this.size.y;
            GUILayout.BeginArea(this.graphMinRect);
            GUILayout.BeginHorizontal();

            GUILayout.Box(this.showMemoryContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));

            GUILayout.Label(this.minMemoryValue.ToString("0.000"), this.nonStyle);

            GUILayout.Box(this.showFpsContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));

            GUILayout.Label(this.minFpsValue.ToString("0.000"), this.nonStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //GUI.EndGroup();
        }

        void drawStack()
        {
            if (this.selectedLog != null)
            {
                Vector2 drag = this.getDrag();
                if (drag.y != 0 && this.stackRect.Contains(new Vector2(this.downPos.x, Screen.height - this.downPos.y)))
                {
                    this.scrollPosition2.y += drag.y - this.oldDrag2;
                }

                this.oldDrag2 = drag.y;

                GUILayout.BeginArea(this.stackRect, this.backStyle);
                this.scrollPosition2 = GUILayout.BeginScrollView(this.scrollPosition2);
                Sample selectedSample = null;
                try
                {
                    selectedSample = this.samples[this.selectedLog.sampleId];
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(this.selectedLog.condition, this.stackLabelStyle);
                GUILayout.EndHorizontal();
                GUILayout.Space(this.size.y * 0.25f);
                GUILayout.BeginHorizontal();
                GUILayout.Label(this.selectedLog.stacktrace, this.stackLabelStyle);
                GUILayout.EndHorizontal();
                GUILayout.Space(this.size.y);
                GUILayout.EndScrollView();
                GUILayout.EndArea();

                GUILayout.BeginArea(this.buttomRect, this.backStyle);
                GUILayout.BeginHorizontal();

                GUILayout.Box(this.showTimeContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
                GUILayout.Label(selectedSample.time.ToString("0.000"), this.nonStyle);
                GUILayout.Space(this.size.x);

                GUILayout.Box(this.showSceneContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
                GUILayout.Label(selectedSample.GetSceneName(), this.nonStyle);
                GUILayout.Space(this.size.x);

                GUILayout.Box(this.showMemoryContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
                GUILayout.Label(selectedSample.memory.ToString("0.000"), this.nonStyle);
                GUILayout.Space(this.size.x);

                GUILayout.Box(this.showFpsContent, this.nonStyle, GUILayout.Width(this.size.x), GUILayout.Height(this.size.y));
                GUILayout.Label(selectedSample.fpsText, this.nonStyle);
                /*GUILayout.Space( size.x );
            GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
            GUILayout.Label( selectedLog.sampleId.ToString() ,nonStyle  );*/
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            else
            {
                GUILayout.BeginArea(this.stackRect, this.backStyle);
                GUILayout.EndArea();
                GUILayout.BeginArea(this.buttomRect, this.backStyle);
                GUILayout.EndArea();
            }
        }

        public void OnGUIDraw()
        {
            if (!this.show)
            {
                return;
            }
            var safeAreaHeight = Screen.height - Screen.safeArea.height;

            this.screenRect.x      = 0;
            this.screenRect.y      = safeAreaHeight +0;
            this.screenRect.width  = Screen.width;
            this.screenRect.height = Screen.height;

            this.getDownPos();

            this.logsRect.x      = 0f;
            this.logsRect.y      = safeAreaHeight + this.size.y * 2f;
            this.logsRect.width  = Screen.width;
            this.logsRect.height = Screen.height * 0.75f - this.size.y * 2f;

            this.stackRectTopLeft.x = 0f;
            this.stackRect.x        = 0f;
            this.stackRectTopLeft.y = Screen.height * 0.75f;
            this.stackRect.y        = safeAreaHeight +Screen.height * 0.75f;
            this.stackRect.width    = Screen.width;
            this.stackRect.height   = Screen.height * 0.25f - this.size.y;

            this.detailRect.x      = 0f;
            this.detailRect.y      = safeAreaHeight +Screen.height - this.size.y * 3;
            this.detailRect.width  = Screen.width;
            this.detailRect.height = this.size.y * 3;

            if (this.currentView == ReportView.Info)
                this.DrawInfo();
            else if (this.currentView == ReportView.Logs)
            {
                this.drawToolBar();
                this.DrawLogs();
            }
        }

        List<Vector2> gestureDetector = new List<Vector2>();
        Vector2       gestureSum      = Vector2.zero;
        float         gestureLength   = 0;
        int           gestureCount    = 0;

        bool isGestureDone()
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touches.Length != 1)
                {
                    this.gestureDetector.Clear();
                    this.gestureCount = 0;
                }
                else
                {
                    if (Input.touches[0].phase == TouchPhase.Canceled || Input.touches[0].phase == TouchPhase.Ended)
                        this.gestureDetector.Clear();
                    else if (Input.touches[0].phase == TouchPhase.Moved)
                    {
                        Vector2 p = Input.touches[0].position;
                        if (this.gestureDetector.Count == 0 || (p - this.gestureDetector[this.gestureDetector.Count - 1]).magnitude > 10)
                            this.gestureDetector.Add(p);
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    this.gestureDetector.Clear();
                    this.gestureCount = 0;
                }
                else
                {
                    if (Input.GetMouseButton(0))
                    {
                        Vector2 p = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                        if (this.gestureDetector.Count == 0 || (p - this.gestureDetector[this.gestureDetector.Count - 1]).magnitude > 10)
                            this.gestureDetector.Add(p);
                    }
                }
            }

            if (this.gestureDetector.Count < 10)
                return false;

            this.gestureSum    = Vector2.zero;
            this.gestureLength = 0;
            Vector2 prevDelta = Vector2.zero;
            for (int i = 0; i < this.gestureDetector.Count - 2; i++)
            {
                Vector2 delta       = this.gestureDetector[i + 1] - this.gestureDetector[i];
                float   deltaLength = delta.magnitude;
                this.gestureSum    += delta;
                this.gestureLength += deltaLength;

                float dot = Vector2.Dot(delta, prevDelta);
                if (dot < 0f)
                {
                    this.gestureDetector.Clear();
                    this.gestureCount = 0;
                    return false;
                }

                prevDelta = delta;
            }

            int gestureBase = (Screen.width + Screen.height) / 4;

            if (this.gestureLength > gestureBase && this.gestureSum.magnitude < gestureBase / 2)
            {
                this.gestureDetector.Clear();
                this.gestureCount++;
                if (this.gestureCount >= this.numOfCircleToShow)
                    return true;
            }

            return false;
        }

        float lastClickTime = -1;

        bool isDoubleClickDone()
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touches.Length != 1)
                {
                    this.lastClickTime = -1;
                }
                else
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        if (this.lastClickTime == -1)
                            this.lastClickTime = Time.realtimeSinceStartup;
                        else if (Time.realtimeSinceStartup - this.lastClickTime < 0.2f)
                        {
                            this.lastClickTime = -1;
                            return true;
                        }
                        else
                        {
                            this.lastClickTime = Time.realtimeSinceStartup;
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (this.lastClickTime == -1)
                        this.lastClickTime = Time.realtimeSinceStartup;
                    else if (Time.realtimeSinceStartup - this.lastClickTime < 0.2f)
                    {
                        this.lastClickTime = -1;
                        return true;
                    }
                    else
                    {
                        this.lastClickTime = Time.realtimeSinceStartup;
                    }
                }
            }

            return false;
        }

        //calculate  pos of first click on screen
        Vector2 startPos;

        Vector2 downPos;

        Vector2 getDownPos()
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Began)
                {
                    this.downPos = Input.touches[0].position;
                    return this.downPos;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    this.downPos.x = Input.mousePosition.x;
                    this.downPos.y = Input.mousePosition.y;
                    return this.downPos;
                }
            }

            return Vector2.zero;
        }
        //calculate drag amount , this is used for scrolling

        Vector2 mousePosition;

        Vector2 getDrag()
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touches.Length != 1)
                {
                    return Vector2.zero;
                }

                return Input.touches[0].position - this.downPos;
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    this.mousePosition = Input.mousePosition;
                    return this.mousePosition - this.downPos;
                }
                else
                {
                    return Vector2.zero;
                }
            }
        }

        //calculate the start index of visible log
        void calculateStartIndex()
        {
            this.startIndex = (int)(this.scrollPosition.y / this.size.y);
            this.startIndex = Mathf.Clamp(this.startIndex, 0, this.currentLog.Count);
        }

        // For FPS Counter
        private       int   frames         = 0;
        private       bool  firstTime      = true;
        private       float lastUpdate     = 0f;
        private const int   requiredFrames = 10;
        private const float updateInterval = 0.25f;

#if UNITY_CHANGE1
	float lastUpdate2 = 0;
#endif

        void doShow()
        {
            this.show        = true;
            this.currentView = ReportView.Logs;
            this.gameObject.AddComponent<ReporterGUI>();

            try
            {
                this.gameObject.SendMessage("OnShowReporter");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void Update()
        {
            this.fpsText       = this.fps.ToString("0.000");
            this.gcTotalMemory = (((float)GC.GetTotalMemory(false)) / 1024 / 1024);
            //addSample();

#if UNITY_CHANGE3
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (sceneIndex != -1 && string.IsNullOrEmpty(scenes[sceneIndex]))
                scenes[SceneManager.GetActiveScene().buildIndex] = SceneManager.GetActiveScene().name;
#else
		int sceneIndex = Application.loadedLevel;
		if (sceneIndex != -1 && string.IsNullOrEmpty(scenes[Application.loadedLevel]))
			scenes[Application.loadedLevel] = Application.loadedLevelName;
#endif

            this.calculateStartIndex();
            if (!this.show && this.isGestureDone())
            {
                this.doShow();
            }

            if (this.threadedLogs.Count > 0)
            {
                lock (this.threadedLogs)
                {
                    for (int i = 0; i < this.threadedLogs.Count; i++)
                    {
                        Log l = this.threadedLogs[i];
                        this.AddLog(l.condition, l.stacktrace, (LogType)l.logType);
                    }

                    this.threadedLogs.Clear();
                }
            }

#if UNITY_CHANGE1
		float elapsed2 = Time.realtimeSinceStartup - lastUpdate2;
		if (elapsed2 > 1) {
			lastUpdate2 = Time.realtimeSinceStartup;
			//be sure no body else take control of log 
			Application.RegisterLogCallback (new Application.LogCallback (CaptureLog));
			Application.RegisterLogCallbackThreaded (new Application.LogCallback (CaptureLogThread));
		}
#endif

            // FPS Counter
            if (this.firstTime)
            {
                this.firstTime  = false;
                this.lastUpdate = Time.realtimeSinceStartup;
                this.frames     = 0;
                return;
            }

            this.frames++;
            float dt = Time.realtimeSinceStartup - this.lastUpdate;
            if (dt > updateInterval && this.frames > requiredFrames)
            {
                this.fps        = (float)this.frames / dt;
                this.lastUpdate = Time.realtimeSinceStartup;
                this.frames     = 0;
            }
        }

        void CaptureLog(string condition, string stacktrace, LogType type) { this.AddLog(condition, stacktrace, type); }

        void AddLog(string condition, string stacktrace, LogType type)
        {
            float  memUsage   = 0f;
            string _condition = "";
            if (this.cachedString.ContainsKey(condition))
            {
                _condition = this.cachedString[condition];
            }
            else
            {
                _condition = condition;
                this.cachedString.Add(_condition, _condition);
                memUsage += (string.IsNullOrEmpty(_condition) ? 0 : _condition.Length * sizeof(char));
                memUsage += IntPtr.Size;
            }

            string _stacktrace = "";
            if (this.cachedString.ContainsKey(stacktrace))
            {
                _stacktrace = this.cachedString[stacktrace];
            }
            else
            {
                _stacktrace = stacktrace;
                this.cachedString.Add(_stacktrace, _stacktrace);
                memUsage += (string.IsNullOrEmpty(_stacktrace) ? 0 : _stacktrace.Length * sizeof(char));
                memUsage += IntPtr.Size;
            }

            bool newLogAdded = false;

            this.addSample();
            Log log = new Log() { logType = (_LogType)type, condition = _condition, stacktrace = _stacktrace, sampleId = this.samples.Count - 1 };
            memUsage += log.GetMemoryUsage();
            //memUsage += samples.Count * 13 ;

            this.logsMemUsage += memUsage / 1024 / 1024;

            if (this.TotalMemUsage > this.maxSize)
            {
                this.clear();
                Debug.Log("Memory Usage Reach" + this.maxSize + " mb So It is Cleared");
                return;
            }

            bool isNew = false;
            //string key = _condition;// + "_!_" + _stacktrace ;
            if (this.logsDic.ContainsKey(_condition, stacktrace))
            {
                isNew = false;
                this.logsDic[_condition][stacktrace].count++;
            }
            else
            {
                isNew = true;
                this.collapsedLogs.Add(log);
                this.logsDic[_condition][stacktrace] = log;

                if (type == LogType.Log)
                    this.numOfCollapsedLogs++;
                else if (type == LogType.Warning)
                    this.numOfCollapsedLogsWarning++;
                else
                    this.numOfCollapsedLogsError++;
            }

            if (type == LogType.Log)
                this.numOfLogs++;
            else if (type == LogType.Warning)
                this.numOfLogsWarning++;
            else
                this.numOfLogsError++;

            this.logs.Add(log);
            if (!this.collapse || isNew)
            {
                bool skip = false;
                if (log.logType == _LogType.Log && !this.showLog)
                    skip = true;
                if (log.logType == _LogType.Warning && !this.showWarning)
                    skip = true;
                if (log.logType == _LogType.Error && !this.showError)
                    skip = true;
                if (log.logType == _LogType.Assert && !this.showError)
                    skip = true;
                if (log.logType == _LogType.Exception && !this.showError)
                    skip = true;

                if (!skip)
                {
                    if (string.IsNullOrEmpty(this.filterText) || log.condition.ToLower().Contains(this.filterText.ToLower()))
                    {
                        this.currentLog.Add(log);
                        newLogAdded = true;
                    }
                }
            }

            if (newLogAdded)
            {
                this.calculateStartIndex();
                int totalCount        = this.currentLog.Count;
                int totalVisibleCount = (int)(Screen.height * 0.75f / this.size.y);
                if (this.startIndex >= (totalCount - totalVisibleCount))
                    this.scrollPosition.y += this.size.y;
            }

            try
            {
                this.gameObject.SendMessage("OnLog", log);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        List<Log> threadedLogs = new List<Log>();

        void CaptureLogThread(string condition, string stacktrace, LogType type)
        {
            Log log = new Log() { condition = condition, stacktrace = stacktrace, logType = (_LogType)type };
            lock (this.threadedLogs)
            {
                this.threadedLogs.Add(log);
            }
        }

#if !UNITY_CHANGE3
    class Scene
    {
    }
    class LoadSceneMode
    {
    }
    void OnLevelWasLoaded()
    {
        _OnLevelWasLoaded( null, null );
    }
#endif
        //new scene is loaded
        void _OnLevelWasLoaded(Scene _null1, LoadSceneMode _null2)
        {
            if (this.clearOnNewSceneLoaded)
                this.clear();

#if UNITY_CHANGE3
            this.currentScene = SceneManager.GetActiveScene().name;
            Debug.Log("Scene " + SceneManager.GetActiveScene().name + " is loaded");
#else
		currentScene = Application.loadedLevelName;
		Debug.Log("Scene " + Application.loadedLevelName + " is loaded");
#endif
        }

        //save user config
        void OnApplicationQuit()
        {
            PlayerPrefs.SetInt("Reporter_currentView", (int)this.currentView);
            PlayerPrefs.SetInt("Reporter_show", (this.show == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_collapse", (this.collapse == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_clearOnNewSceneLoaded", (this.clearOnNewSceneLoaded == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showTime", (this.showTime == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showScene", (this.showScene == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showMemory", (this.showMemory == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showFps", (this.showFps == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showGraph", (this.showGraph == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showLog", (this.showLog == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showWarning", (this.showWarning == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showError", (this.showError == true) ? 1 : 0);
            PlayerPrefs.SetString("Reporter_filterText", this.filterText);
            PlayerPrefs.SetFloat("Reporter_size", this.size.x);

            PlayerPrefs.SetInt("Reporter_showClearOnNewSceneLoadedButton", (this.showClearOnNewSceneLoadedButton == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showTimeButton", (this.showTimeButton == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showSceneButton", (this.showSceneButton == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showMemButton", (this.showMemButton == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showFpsButton", (this.showFpsButton == true) ? 1 : 0);
            PlayerPrefs.SetInt("Reporter_showSearchText", (this.showSearchText == true) ? 1 : 0);

            PlayerPrefs.Save();
        }

        //read build information 
        IEnumerator readInfo()
        {
            string prefFile = "build_info";
            string url      = prefFile;

            if (prefFile.IndexOf("://") == -1)
            {
                string streamingAssetsPath = Application.streamingAssetsPath;
                if (streamingAssetsPath == "")
                    streamingAssetsPath = Application.dataPath + "/StreamingAssets/";
                url = Path.Combine(streamingAssetsPath, prefFile);
            }

            //if (Application.platform != RuntimePlatform.OSXWebPlayer && Application.platform != RuntimePlatform.WindowsWebPlayer)
            if (!url.Contains("://"))
                url = "file://" + url;

            // float startTime = Time.realtimeSinceStartup;
#if UNITY_CHANGE4
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
#else
		WWW www = new WWW(url);
		yield return www;
#endif

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
            }
            else
            {
#if UNITY_CHANGE4
                this.buildDate = www.downloadHandler.text;
#else
			buildDate = www.text;
#endif
            }

            yield break;
        }

        private void SaveLogsToDevice()
        {
            string       filePath         = Application.persistentDataPath + "/logs.txt";
            List<string> fileContentsList = new List<string>();
            Debug.Log("Saving logs to " + filePath);
            File.Delete(filePath);
            for (int i = 0; i < this.logs.Count; i++)
            {
                fileContentsList.Add(this.logs[i].logType + "\n" + this.logs[i].condition + "\n" + this.logs[i].stacktrace);
            }

            File.WriteAllLines(filePath, fileContentsList.ToArray());
        }
    }
}