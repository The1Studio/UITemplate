using UnityEngine;
using UnityEditor;

namespace BuildReportTool.Window.Screen
{
    public class BuildSettings : BaseScreen
    {
        public override string Name => Labels.BUILD_SETTINGS_CATEGORY_LABEL;

        public override void RefreshData(BuildInfo buildReport, AssetDependencies assetDependencies, TextureData textureData, MeshData meshData, UnityBuildReport unityBuildReport)
        {
            this._selectedSettingsIdxFromDropdownBox = UnityBuildSettingsUtility.GetIdxFromBuildReportValues(buildReport);
        }

        private Vector2 _scrollPos;

        private const int SETTING_SPACING              = 4;
        private const int SETTINGS_GROUP_TITLE_SPACING = 3;
        private const int SETTINGS_GROUP_SPACING       = 18;
        private const int SETTINGS_GROUP_MINOR_SPACING = 12;

        private const int DEFAULT_SHORT_COMMIT_HASH_LENGTH_DISPLAYED = 10;

        private void DrawSetting(string name, bool val, bool showEvenIfEmpty = true)
        {
            this.DrawSetting(name, val.ToString(), showEvenIfEmpty);
        }

        private void DrawSetting(string name, int val, bool showEvenIfEmpty = true)
        {
            this.DrawSetting(name, val.ToString(), showEvenIfEmpty);
        }

        private void DrawSetting(string name, uint val, bool showEvenIfEmpty = true)
        {
            this.DrawSetting(name, val.ToString(), showEvenIfEmpty);
        }

        private void DrawSetting(string name, string val, bool showEvenIfEmpty = true)
        {
            if (string.IsNullOrEmpty(val) && !showEvenIfEmpty) return;

            var nameStyle                    = GUI.skin.FindStyle(Settings.SETTING_NAME_STYLE_NAME);
            if (nameStyle == null) nameStyle = GUI.skin.label;

            var valueStyle                     = GUI.skin.FindStyle(Settings.SETTING_VALUE_NO_WRAP_STYLE_NAME);
            if (valueStyle == null) valueStyle = GUI.skin.label;

            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;

            GUILayout.BeginHorizontal(GUIContent.none, groupStyle, NoExpandWidth);
            GUILayout.Label(name, nameStyle, BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(2);
            if (!string.IsNullOrEmpty(val)) GUILayout.TextField(val, valueStyle, BRT_BuildReportWindow.LayoutNone);

            GUILayout.EndHorizontal();
            GUILayout.Space(SETTING_SPACING);
        }

        private void DrawSetting(string name, string[] val, bool showEvenIfEmpty = true)
        {
            if ((val == null || val.Length == 0) && !showEvenIfEmpty) return;

            var nameStyle                    = GUI.skin.FindStyle(Settings.SETTING_NAME_STYLE_NAME);
            if (nameStyle == null) nameStyle = GUI.skin.label;

            var valueStyle                     = GUI.skin.FindStyle(Settings.SETTING_VALUE_NO_WRAP_STYLE_NAME);
            if (valueStyle == null) valueStyle = GUI.skin.label;

            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;

            GUILayout.BeginHorizontal(GUIContent.none, groupStyle, BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label(name, nameStyle, BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(2);

            if (val != null)
            {
                GUILayout.BeginVertical(GUIContent.none, groupStyle, BRT_BuildReportWindow.LayoutNone);
                for (int n = 0, len = val.Length; n < len; ++n) GUILayout.TextField(val[n], valueStyle, BRT_BuildReportWindow.LayoutNone);

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(SETTING_SPACING);
        }

        private void DrawSetting2Lines(string name, string val, bool showEvenIfEmpty = true)
        {
            if (string.IsNullOrEmpty(val) && !showEvenIfEmpty) return;

            var nameStyle                    = GUI.skin.FindStyle(Settings.SETTING_NAME_STYLE_NAME);
            if (nameStyle == null) nameStyle = GUI.skin.label;

            var valueStyle                     = GUI.skin.FindStyle(Settings.SETTING_VALUE_NO_WRAP_STYLE_NAME);
            if (valueStyle == null) valueStyle = GUI.skin.label;

            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;

            GUILayout.Label(name, nameStyle, BRT_BuildReportWindow.LayoutNone);
            if (!string.IsNullOrEmpty(val))
            {
                GUILayout.BeginHorizontal(GUIContent.none, groupStyle, NoExpandWidth);
                GUILayout.Space(10);
                GUILayout.TextField(val, valueStyle, BRT_BuildReportWindow.LayoutNone);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(SETTING_SPACING);
        }

        private void DrawSettingsGroupTitle(string name)
        {
            var titleStyle                     = GUI.skin.FindStyle(Settings.INFO_TITLE_STYLE_NAME);
            if (titleStyle == null) titleStyle = GUI.skin.label;

            GUILayout.Label(name, titleStyle, BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(SETTINGS_GROUP_TITLE_SPACING);
        }

        // =================================================================================

        private BuildSettingCategory _settingsShown = BuildSettingCategory.None;

        // ----------------------------------------------

        private bool IsShowingWebPlayerSettings => this._settingsShown == BuildSettingCategory.WebPlayer;

        private bool IsShowingWebGlSettings => this._settingsShown == BuildSettingCategory.WebGL;

        // ----------------------------------------------

        private bool IsShowingStandaloneSettings => this.IsShowingWindowsDesktopSettings || this.IsShowingMacSettings || this.IsShowingLinuxSettings;

        private bool IsShowingWindowsDesktopSettings => this._settingsShown == BuildSettingCategory.WindowsDesktopStandalone;

        private bool IsShowingWindowsStoreAppSettings => this._settingsShown == BuildSettingCategory.WindowsStoreApp;

        private bool IsShowingMacSettings => this._settingsShown == BuildSettingCategory.MacStandalone;

        private bool IsShowingLinuxSettings => this._settingsShown == BuildSettingCategory.LinuxStandalone;

        // ----------------------------------------------

        private bool IsShowingMobileSettings => this.IsShowingiOSSettings || this.IsShowingAndroidSettings || this.IsShowingBlackberrySettings;

        private bool IsShowingiOSSettings => this._settingsShown == BuildSettingCategory.iOS;

        private bool IsShowingAndroidSettings => this._settingsShown == BuildSettingCategory.Android;

        private bool IsShowingBlackberrySettings => this._settingsShown == BuildSettingCategory.Blackberry;

        // ----------------------------------------------

        private bool IsShowingXbox360Settings => this._settingsShown == BuildSettingCategory.Xbox360;

        private bool IsShowingXboxOneSettings => this._settingsShown == BuildSettingCategory.XboxOne;

        private bool IsShowingPS3Settings => this._settingsShown == BuildSettingCategory.PS3;

        private bool IsShowingPS4Settings => this._settingsShown == BuildSettingCategory.PS4;

        private bool IsShowingPSVitaSettings => this._settingsShown == BuildSettingCategory.PSVita;

        // ----------------------------------------------

        private bool IsShowingSamsungTvSettings => this._settingsShown == BuildSettingCategory.SamsungTV;

        // =================================================================================

        private int _selectedSettingsIdxFromDropdownBox;

        private GUIContent[] _settingDropdownBoxLabels;
        private string       _buildTargetOfReport = string.Empty;

        private void InitializeDropdownBoxLabelsIfNeeded()
        {
            if (this._settingDropdownBoxLabels != null) return;

            this._settingDropdownBoxLabels = UnityBuildSettingsUtility.GetBuildSettingsCategoryListForDropdownBox();
        }

        private static readonly GUILayoutOption[] NoExpandWidth = { GUILayout.ExpandWidth(false) };

        // =================================================================================

        private Rect _projectSettingsRect;
        private Rect _pathSettingsRect;
        private Rect _buildSettingsRect;
        private Rect _runtimeSettingsRect;
        private Rect _debugSettingsRect;
        private Rect _codeSettingsRect;
        private Rect _graphicsSettingsRect;

        private float _column1Width;

        private void DrawProjectSettings(BuildInfo buildReportToDisplay, UnityBuildSettings settings)
        {
            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;
            GUILayout.BeginVertical(GUIContent.none, groupStyle, NoExpandWidth);
            this.DrawSettingsGroupTitle("Project");

            this.DrawSetting("Product name:", settings.ProductName);
            this.DrawSetting("Company name:", settings.CompanyName);
            this.DrawSetting("Build type:", buildReportToDisplay.BuildType);
            this.DrawSetting("Unity version:", buildReportToDisplay.UnityVersion);
            this.DrawSetting("Using Pro license:", settings.UsingAdvancedLicense);

            if (this.IsShowingiOSSettings)
            {
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
                this.DrawSetting("App display name:", settings.iOSAppDisplayName);
                this.DrawSetting("Bundle identifier:", settings.MobileBundleIdentifier);
                this.DrawSetting("Bundle version:", settings.MobileBundleVersion);
            }
            else if (this.IsShowingAndroidSettings)
            {
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
                this.DrawSetting("Package identifier:", settings.MobileBundleIdentifier);
                this.DrawSetting("Version name:", settings.MobileBundleVersion);
                this.DrawSetting("Version code:", settings.AndroidVersionCode);
            }
            else if (this.IsShowingXbox360Settings)
            {
                this.DrawSetting("Title ID:", settings.Xbox360TitleId);
            }
            else if (this.IsShowingXboxOneSettings)
            {
                this.DrawSetting("Title ID:", settings.XboxOneTitleId);
                this.DrawSetting("Content ID:", settings.XboxOneContentId);
                this.DrawSetting("Product ID:", settings.XboxOneProductId);
                this.DrawSetting("Sandbox ID:", settings.XboxOneSandboxId);
                this.DrawSetting("Service Configuration ID:", settings.XboxOneServiceConfigId);
                this.DrawSetting("Xbox One version:", settings.XboxOneVersion);
                this.DrawSetting("Description:", settings.XboxOneDescription);
            }
            else if (this.IsShowingPS4Settings)
            {
                this.DrawSetting("App type:", settings.PS4AppType);
                this.DrawSetting("App version:", settings.PS4AppVersion);
                this.DrawSetting("Category:", settings.PS4Category);
                this.DrawSetting("Content ID:", settings.PS4ContentId);
                this.DrawSetting("Master version:", settings.PS4MasterVersion);
            }
            else if (this.IsShowingPSVitaSettings)
            {
                this.DrawSetting("Short title:", settings.PSVShortTitle);
                this.DrawSetting("App version:", settings.PSVAppVersion);
                this.DrawSetting("App category:", settings.PSVAppCategory);
                this.DrawSetting("Content ID:", settings.PSVContentId);
                this.DrawSetting("Master version:", settings.PSVMasterVersion);
            }
            GUILayout.EndVertical();
            if (Event.current.type == EventType.Repaint) this._projectSettingsRect = GUILayoutUtility.GetLastRect();
        }

        private void DrawBuildSettings(BuildInfo buildReportToDisplay, UnityBuildSettings settings, UnityBuildReport unityBuildReport)
        {
            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;

            GUILayout.BeginVertical(GUIContent.none, groupStyle, NoExpandWidth);
            this.DrawSettingsGroupTitle("Build Settings");

            // --------------------------------------------------
            // build settings
            if (this.IsShowingStandaloneSettings)
            {
                this.DrawSetting("Headless (server) build:", settings.EnableHeadlessMode);
            }
            else if (this.IsShowingWindowsStoreAppSettings)
            {
                this.DrawSetting("Generate reference projects:", settings.WSAGenerateReferenceProjects);
                this.DrawSetting("Target Windows Store App SDK:", settings.WSASDK);
            }
            else if (this.IsShowingWebPlayerSettings)
            {
                this.DrawSetting("Web player streaming:", settings.WebPlayerEnableStreaming);
                this.DrawSetting("Web player offline deployment:", settings.WebPlayerDeployOffline);
                this.DrawSetting("First streamed level with \"Resources\" assets:",
                    settings.WebPlayerFirstStreamedLevelWithResources);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingWebGlSettings)
            {
                this.DrawSetting("WebGL Template used:", settings.WebGLTemplatePath);
                this.DrawSetting("WebGL optimization level:",
                    UnityBuildSettingsUtility.GetReadableWebGLOptimizationLevel(settings.WebGLOptimizationLevel),
                    false);

                this.DrawSetting("Compression format:", settings.WebGLCompressionFormat);

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingiOSSettings)
            {
                this.DrawSetting("SDK version:", settings.iOSSDKVersionUsed);
                this.DrawSetting("Target iOS version:", settings.iOSTargetOSVersion);
                this.DrawSetting("Target device:", settings.iOSTargetDevice);
                this.DrawSetting("Symlink libraries:", settings.iOSSymlinkLibraries);

                if (unityBuildReport != null)
                    this.DrawSetting("Is appended build:",
                        unityBuildReport.HasBuildOption(BuildOptions.AcceptExternalModificationsToPlayer));

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingAndroidSettings)
            {
                this.DrawSetting("Build subtarget:", settings.AndroidBuildSubtarget);
                this.DrawSetting("Min SDK version:", settings.AndroidMinSDKVersion);
                this.DrawSetting("Target device:", settings.AndroidTargetDevice);
                this.DrawSetting("Automatically create APK Expansion File:", settings.AndroidUseAPKExpansionFiles);
                this.DrawSetting("Export Android project:", settings.AndroidAsAndroidProject);
                if (unityBuildReport != null)
                    this.DrawSetting("New Eclipse project on each build:",
                        unityBuildReport.HasBuildOption(BuildOptions.AcceptExternalModificationsToPlayer));
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting("Is game:", settings.AndroidIsGame);
                this.DrawSetting("TV-compatible:", settings.AndroidTvCompatible);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting("Force Internet permission:", settings.AndroidForceInternetPermission);
                this.DrawSetting("Force SD card permission:", settings.AndroidForceSDCardPermission);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting("Key alias name:", settings.AndroidKeyAliasName);
                this.DrawSetting("Keystore name:", settings.AndroidKeystoreName);
            }
            else if (this.IsShowingBlackberrySettings)
            {
                this.DrawSetting("Build subtarget:", settings.BlackBerryBuildSubtarget);
                this.DrawSetting("Build type:", settings.BlackBerryBuildType);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                if (buildReportToDisplay.IsUnityVersionAtMost(4, 0, 0)) this.DrawSetting("Author ID:", settings.BlackBerryAuthorID);

                this.DrawSetting("Device address:", settings.BlackBerryDeviceAddress);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting("Save log path:", settings.BlackBerrySaveLogPath);
                this.DrawSetting("Token path:", settings.BlackBerryTokenPath);

                this.DrawSetting("Token author:", settings.BlackBerryTokenAuthor);
                this.DrawSetting("Token expiration:", settings.BlackBerryTokenExpiration);
            }
            else if (this.IsShowingXbox360Settings)
            {
                this.DrawSetting("Build subtarget:", settings.Xbox360BuildSubtarget);
                this.DrawSetting("Run method:", settings.Xbox360RunMethod);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting("Image .xex filepath:", settings.Xbox360ImageXexFilePath);
                this.DrawSetting(".spa filepath:", settings.Xbox360SpaFilePath);
                this.DrawSetting("Auto-generate .spa:", settings.Xbox360AutoGenerateSpa);
                this.DrawSetting("Additional title memory size:", settings.Xbox360AdditionalTitleMemSize);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingXboxOneSettings)
            {
                this.DrawSetting("Deploy method:", settings.XboxOneDeployMethod);
                this.DrawSetting("Is content package:", settings.XboxOneIsContentPackage);
                this.DrawSetting("Packaging encryption level:", settings.XboxOnePackagingEncryptionLevel);
                this.DrawSetting("Allowed product IDs:", settings.XboxOneAllowedProductIds);
                this.DrawSetting("Disable Kinect GPU reservation:", settings.XboxOneDisableKinectGpuReservation);
                this.DrawSetting("Enable variable GPU:", settings.XboxOneEnableVariableGPU);
                this.DrawSetting("Streaming install launch range:", settings.XboxOneStreamingInstallLaunchRange);
                this.DrawSetting("Persistent local storage size:", settings.XboxOnePersistentLocalStorageSize);
                this.DrawSetting("Socket names:", settings.XboxOneSocketNames);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting("Game OS override path:", settings.XboxOneGameOsOverridePath);
                this.DrawSetting("App manifest override path:", settings.XboxOneAppManifestOverridePath);
                this.DrawSetting("Packaging override path:", settings.XboxOnePackagingOverridePath);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingPS3Settings)
            {
                this.DrawSetting("Build subtarget:", settings.SCEBuildSubtarget);

                this.DrawSetting("NP Communications ID:", settings.PS3NpCommsId);
                this.DrawSetting("NP Communications Signature:", settings.PS3NpCommsSig);
                this.DrawSetting("NP Age Rating:", settings.PS3NpAgeRating);

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting("Title config filepath:", settings.PS3TitleConfigFilePath);
                this.DrawSetting("DLC config filepath:", settings.PS3DLCConfigFilePath);
                this.DrawSetting("Thumbnail filepath:", settings.PS3ThumbnailFilePath);
                this.DrawSetting("Background image filepath:", settings.PS3BackgroundImageFilePath);
                this.DrawSetting("Background sound filepath:", settings.PS3BackgroundSoundFilePath);
                this.DrawSetting("Trophy package path:", settings.PS3TrophyPackagePath);

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting("Compress build with PS Arc:", settings.CompressBuildWithPsArc);
                this.DrawSetting("Need submission materials:", settings.NeedSubmissionMaterials);

                this.DrawSetting("In trial mode:", settings.PS3InTrialMode);
                this.DrawSetting("Disable Dolby encoding:", settings.PS3DisableDolbyEncoding);
                this.DrawSetting("Enable Move support:", settings.PS3EnableMoveSupport);
                this.DrawSetting("Use SPU for Umbra:", settings.PS3UseSPUForUmbra);

                this.DrawSetting("Video memory for vertex buffers:", settings.PS3VideoMemoryForVertexBuffers);
                this.DrawSetting("Video memory for audio:", settings.PS3VideoMemoryForAudio);
                this.DrawSetting("Boot check max save game size (KB):", settings.PS3BootCheckMaxSaveGameSizeKB);
                this.DrawSetting("Save game slots:", settings.PS3SaveGameSlots);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingPS4Settings)
            {
                this.DrawSetting("Build subtarget:", settings.PS4BuildSubtarget);

                this.DrawSetting("App parameter 1:", settings.PS4AppParameter1);
                this.DrawSetting("App parameter 2:", settings.PS4AppParameter2);
                this.DrawSetting("App parameter 3:", settings.PS4AppParameter3);
                this.DrawSetting("App parameter 4:", settings.PS4AppParameter4);

                this.DrawSetting("Enter button assignment:", settings.PS4EnterButtonAssignment);
                this.DrawSetting("Remote play key assignment:", settings.PS4RemotePlayKeyAssignment);

                this.DrawSetting("NP Age rating:", settings.PS4NpAgeRating);
                this.DrawSetting("Parental level:", settings.PS4ParentalLevel);

                this.DrawSetting("Enable friend push notifications:", settings.PS4EnableFriendPushNotifications);
                this.DrawSetting("Enable presence push notifications:", settings.PS4EnablePresencePushNotifications);
                this.DrawSetting("Enable session push notifications:", settings.PS4EnableSessionPushNotifications);
                this.DrawSetting("Enable game custom data push notifications:",
                    settings.PS4EnableGameCustomDataPushNotifications);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting("Background image path:", settings.PS4BgImagePath);
                this.DrawSetting("Background music path:", settings.PS4BgMusicPath);
                this.DrawSetting("Startup image path:", settings.PS4StartupImagePath);
                this.DrawSetting("Save data image path:", settings.PS4SaveDataImagePath);

                this.DrawSetting("Params sfx path:", settings.PS4ParamSfxPath);
                this.DrawSetting("NP Title dat path:", settings.PS4NpTitleDatPath);
                this.DrawSetting("NP Trophy Package path:", settings.PS4NpTrophyPackagePath);
                this.DrawSetting("Pronunciations SIG path:", settings.PS4PronunciationSigPath);
                this.DrawSetting("Pronunciations XML path:", settings.PS4PronunciationXmlPath);

                this.DrawSetting("Share file path:", settings.PS4ShareFilePath);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingPSVitaSettings)
            {
                this.DrawSetting("Build subtarget:", settings.PSVBuildSubtarget);

                this.DrawSetting("DRM type:", settings.PSVDrmType);
                this.DrawSetting("Upgradable:", settings.PSVUpgradable);
                this.DrawSetting("TV boot mode:", settings.PSVTvBootMode);
                this.DrawSetting("Parental Level:", settings.PSVParentalLevel);
                this.DrawSetting("Health warning:", settings.PSVHealthWarning);
                this.DrawSetting("Enter button assignment:", settings.PSVEnterButtonAssignment);

                this.DrawSetting("Acquire BGM:", settings.PSVAcquireBgm);
                this.DrawSetting("Allow Twitter Dialog:", settings.PSVAllowTwitterDialog);

                this.DrawSetting("NP Communications ID:", settings.PSVNpCommsId);
                this.DrawSetting("NP Communications Signature:", settings.PSVNpCommsSig);
                this.DrawSetting("Age Rating:", settings.PSVNpAgeRating);

                this.DrawSetting("Power mode:", settings.PSVPowerMode);
                this.DrawSetting("Media capacity:", settings.PSVMediaCapacity);
                this.DrawSetting("Storage type:", settings.PSVStorageType);
                this.DrawSetting("TV disable emu:", settings.PSVTvDisableEmu);
                this.DrawSetting("Support Game Boot Message or Game Joining Presence:", settings.PSVNpSupportGbmOrGjp);
                this.DrawSetting("Use lib location:", settings.PSVUseLibLocation);

                this.DrawSetting("Info bar color:", settings.PSVInfoBarColor);
                this.DrawSetting("Show info bar on startup:", settings.PSVShowInfoBarOnStartup);
                this.DrawSetting("Save data quota:", settings.PSVSaveDataQuota);

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting("Manual filepath:", settings.PSVManualPath);
                this.DrawSetting("Trophy package filepath:", settings.PSVTrophyPackagePath);
                this.DrawSetting("Params Sfx filepath:", settings.PSVParamSfxPath);
                this.DrawSetting("Patch change info filepath:", settings.PSVPatchChangeInfoPath);
                this.DrawSetting("Patch original filepath:", settings.PSVPatchOriginalPackPath);
                this.DrawSetting("Keystone filepath:", settings.PSVKeystoneFilePath);
                this.DrawSetting("Live Area BG image filepath:", settings.PSVLiveAreaBgImagePath);
                this.DrawSetting("Live Area Gate image filepath:", settings.PSVLiveAreaGateImagePath);
                this.DrawSetting("Custom Live Area path:", settings.PSVCustomLiveAreaPath);
                this.DrawSetting("Live Area trial path:", settings.PSVLiveAreaTrialPath);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingSamsungTvSettings)
            {
                this.DrawSetting("Device address:", settings.SamsungTVDeviceAddress);
                this.DrawSetting("Author:", settings.SamsungTVAuthor);
                this.DrawSetting("Author email:", settings.SamsungTVAuthorEmail);
                this.DrawSetting("Website:", settings.SamsungTVAuthorWebsiteUrl);
                this.DrawSetting("Category:", settings.SamsungTVCategory);
                this.DrawSetting("Description:", settings.SamsungTVDescription);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }

            if (unityBuildReport != null) this.DrawSetting("Scripts only build:", unityBuildReport.HasBuildOption(BuildOptions.BuildScriptsOnly));

            this.DrawSetting("Install in build folder:", settings.InstallInBuildFolder);

            if (buildReportToDisplay.IsUnityVersionAtMost(4, 0, 0)) this.DrawSetting("Physics code stripped:", settings.StripPhysicsCode);

            this.DrawSetting("Prebake collision meshes:", settings.BakeCollisionMeshes);
            this.DrawSetting("Optimize mesh data:", settings.StripUnusedMeshComponents);

            if (unityBuildReport != null)
            {
                #if UNITY_5_6_OR_NEWER
                if (unityBuildReport.HasBuildOption(BuildOptions.CompressWithLz4))
                    this.DrawSetting("Compression Method:", "LZ4");
                #endif
                #if UNITY_2017_2_OR_NEWER
                else if (unityBuildReport.HasBuildOption(BuildOptions.CompressWithLz4HC))
                    this.DrawSetting("Compression Method:", "LZ4HC");
                #endif
                #if UNITY_5_6_OR_NEWER
                else
                    #endif
                    this.DrawSetting("Compression Method:", "Default");
                #if UNITY_2018_1_OR_NEWER
                this.DrawSetting("Test Assemblies included in build:", unityBuildReport.HasBuildOption(BuildOptions.IncludeTestAssemblies));
                #endif
                #if UNITY_5_6_OR_NEWER
                this.DrawSetting("No Unique Identifier (force build GUID to all zeros):", unityBuildReport.HasBuildOption(BuildOptions.NoUniqueIdentifier));
                #endif
                #if UNITY_2020_1_OR_NEWER
                this.DrawSetting("Detailed Build Report:", unityBuildReport.HasBuildOption(BuildOptions.DetailedBuildReport));
                #endif
            }

            if (this.IsShowingMobileSettings)
                this.DrawSetting("Stripping level:", settings.StrippingLevelUsed);
            else if (this.IsShowingWebGlSettings) this.DrawSetting("Strip engine code (IL2CPP):", settings.StripEngineCode);
            GUILayout.EndVertical();
            if (Event.current.type == EventType.Repaint) this._buildSettingsRect = GUILayoutUtility.GetLastRect();
        }

        private void DrawRuntimeSettings(BuildInfo buildReportToDisplay, UnityBuildSettings settings)
        {
            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;

            GUILayout.BeginVertical(GUIContent.none, groupStyle, NoExpandWidth);
            this.DrawSettingsGroupTitle("Runtime Settings");

            if (this.IsShowingiOSSettings)
            {
                this.DrawSetting("Hide status bar:", settings.MobileHideStatusBar);
                this.DrawSetting("Status bar style:", settings.iOSStatusBarStyle);
                this.DrawSetting("Accelerometer frequency:", settings.MobileAccelerometerFrequency);
                this.DrawSetting("Requires persistent Wi-Fi:", settings.iOSRequiresPersistentWiFi);

                if (buildReportToDisplay.IsUnityVersionAtMost(4, 0, 0)) this.DrawSetting("Exit on suspend:", settings.iOSExitOnSuspend);

                if (buildReportToDisplay.IsUnityVersionAtLeast(5, 0, 0)) this.DrawSetting("App-in-background behavior:", settings.iOSAppInBackgroundBehavior);

                this.DrawSetting("Activity indicator on loading:", settings.iOSShowProgressBarInLoadingScreen);

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingAndroidSettings)
            {
                this.DrawSetting("Hide status bar:", settings.MobileHideStatusBar);
                this.DrawSetting("Accelerometer frequency:", settings.MobileAccelerometerFrequency);
                this.DrawSetting("Activity indicator on loading:", settings.AndroidShowProgressBarInLoadingScreen);
                this.DrawSetting("Splash screen scale:", settings.AndroidSplashScreenScaleMode);

                this.DrawSetting("Preferred install location:", settings.AndroidPreferredInstallLocation);

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingWebGlSettings)
            {
                this.DrawSetting("Automatically cache WebGL assets data:", settings.WebGLAutoCacheAssetsData);
                this.DrawSetting("WebGL Memory Size:", settings.WebGLMemorySize);

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }

            if (!this.IsShowingiOSSettings && !this.IsShowingAndroidSettings && this.IsShowingMobileSettings) // any mobile except iOS, Android
            {
                this.DrawSetting("Hide status bar:", settings.MobileHideStatusBar);
                this.DrawSetting("Accelerometer frequency:", settings.MobileAccelerometerFrequency);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }

            if (this.IsShowingXbox360Settings)
            {
                this.DrawSetting("Enable avatar:", settings.Xbox360EnableAvatar);
                this.DrawSetting("Enable Kinect:", settings.Xbox360EnableKinect);
                this.DrawSetting("Enable Kinect auto-tracking:", settings.Xbox360EnableKinectAutoTracking);

                this.DrawSetting("Deploy Kinect resources:", settings.Xbox360DeployKinectResources);
                this.DrawSetting("Deploy Kinect head orientation:", settings.Xbox360DeployKinectHeadOrientation);
                this.DrawSetting("Deploy Kinect head position:", settings.Xbox360DeployKinectHeadPosition);

                this.DrawSetting("Enable speech:", settings.Xbox360EnableSpeech);
                this.DrawSetting("Speech DB:", settings.Xbox360SpeechDB);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingBlackberrySettings)
            {
                this.DrawSetting("Has camera permissions:", settings.BlackBerryHasCamPermissions);
                this.DrawSetting("Has microphone permissions:", settings.BlackBerryHasMicPermissions);
                this.DrawSetting("Has GPS permissions:", settings.BlackBerryHasGpsPermissions);
                this.DrawSetting("Has ID permissions:", settings.BlackBerryHasIdPermissions);
                this.DrawSetting("Has shared permissions:", settings.BlackBerryHasSharedPermissions);
            }

            if (this.IsShowingStandaloneSettings || this.IsShowingWebPlayerSettings || this.IsShowingBlackberrySettings)
            {
                this.DrawSetting("Run in background:", settings.RunInBackground);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }

            // --------------------------------------------------
            // security settings
            if (this.IsShowingMacSettings)
                this.DrawSetting("Use App Store validation:", settings.MacUseAppStoreValidation);
            else if (this.IsShowingAndroidSettings) this.DrawSetting("Use license verification:", settings.AndroidUseLicenseVerification);

            GUILayout.EndVertical();
            if (Event.current.type == EventType.Repaint) this._runtimeSettingsRect = GUILayoutUtility.GetLastRect();
        }

        private void DrawDebugSettings(BuildInfo buildReportToDisplay, UnityBuildSettings settings, UnityBuildReport unityBuildReport)
        {
            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;

            GUILayout.BeginVertical(GUIContent.none, groupStyle, NoExpandWidth);
            this.DrawSettingsGroupTitle("Debug Settings");

            this.DrawSetting("Is development build:", settings.EnableDevelopmentBuild);
            if (this.IsShowingWindowsDesktopSettings)
            {
                this.DrawSetting("PDB files for native DLLs included in build:", settings.WinIncludeNativePdbFilesInBuild);
                this.DrawSetting("Create Visual Studio Solution:", settings.WinCreateVisualStudioSolution);
            }
            this.DrawSetting("Debug Log enabled:", settings.EnableDebugLog);

            if (buildReportToDisplay.IsUnityVersionAtLeast(5, 4, 0))
            {
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

                this.DrawSetting2Lines("Stack trace for regular logs:",
                    UnityBuildSettingsUtility.GetReadableStackTraceType(settings.StackTraceForLog),
                    false);
                this.DrawSetting2Lines("Stack trace for warning logs:",
                    UnityBuildSettingsUtility.GetReadableStackTraceType(settings.StackTraceForWarning),
                    false);
                this.DrawSetting2Lines("Stack trace for error logs:",
                    UnityBuildSettingsUtility.GetReadableStackTraceType(settings.StackTraceForError),
                    false);
                this.DrawSetting2Lines("Stack trace for assert logs:",
                    UnityBuildSettingsUtility.GetReadableStackTraceType(settings.StackTraceForAssert),
                    false);
                this.DrawSetting2Lines("Stack trace for exception logs:",
                    UnityBuildSettingsUtility.GetReadableStackTraceType(settings.StackTraceForException),
                    false);
            }

            GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

            if (this.IsShowingPS3Settings)
            {
                this.DrawSetting("Enable verbose memory stats:", settings.PS3EnableVerboseMemoryStats);

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingiOSSettings)
            {
                if (buildReportToDisplay.IsUnityVersionAtLeast(5, 0, 0)) this.DrawSetting("Log Objective-C uncaught exceptions:", settings.iOSLogObjCUncaughtExceptions);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingWebGlSettings)
            {
                this.DrawSetting("Use pre-built WebGL Unity engine:", settings.WebGLUsePreBuiltUnityEngine);
                this.DrawSetting("Create WebGL debug symbols file:", settings.WebGLCreateDebugSymbolsFile);
                this.DrawSetting("WebGL debug symbols mode:", settings.WebGLDebugSymbolMode);
                this.DrawSetting("WebGL exception support:", settings.WebGLExceptionSupportType);

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }

            this.DrawSetting("Enable explicit null checks:", settings.EnableExplicitNullChecks);

            if (buildReportToDisplay.IsUnityVersionAtLeast(5, 4, 0)) this.DrawSetting("Enable explicit divide-by-zero checks:", settings.EnableExplicitDivideByZeroChecks);

            if (buildReportToDisplay.IsUnityVersionAtLeast(5, 0, 0))
            {
                this.DrawSetting("Action on unhandled .NET exception:", settings.ActionOnDotNetUnhandledException);

                this.DrawSetting("Enable internal profiler:", settings.EnableInternalProfiler);

                this.DrawSetting("Enable CrashReport API:", settings.EnableCrashReportApi);
            }

            GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

            this.DrawSetting("Auto-connect to Unity Editor Profiler:", settings.ConnectProfiler);
            if (unityBuildReport != null)
            {
                #if UNITY_2019_3_OR_NEWER
                this.DrawSetting("Deep Profiling Support:", unityBuildReport.HasBuildOption(BuildOptions.EnableDeepProfilingSupport));
                #endif
                #if UNITY_5_2 || UNITY_5_3_OR_NEWER
                this.DrawSetting("Force enable assertions in release build:", unityBuildReport.HasBuildOption(BuildOptions.ForceEnableAssertions));
                #endif
            }
            this.DrawSetting("Allow script Debugger:", settings.EnableSourceDebugging);
            this.DrawSetting("Wait for Managed Debugger before executing scripts:", settings.WaitForManagedDebugger);

            //DrawSetting("Force script optimization on debug builds:", settings.ForceOptimizeScriptCompilation);

            GUILayout.EndVertical();
            if (Event.current.type == EventType.Repaint) this._debugSettingsRect = GUILayoutUtility.GetLastRect();
        }

        private void DrawCodeSettings(BuildInfo buildReportToDisplay, UnityBuildSettings settings)
        {
            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;

            GUILayout.BeginVertical(GUIContent.none, groupStyle, NoExpandWidth);
            this.DrawSettingsGroupTitle("Code Settings");

            this.DrawSetting("Script Compilation Defines:", settings.CompileDefines);

            this.DrawSetting(".NET API compatibility level:", settings.NETApiCompatibilityLevel);
            this.DrawSetting("AOT options:", settings.AOTOptions);
            this.DrawSetting("Location usage description:", settings.LocationUsageDescription);

            if (this.IsShowingiOSSettings)
            {
                this.DrawSetting("Script call optimized:", settings.iOSScriptCallOptimizationUsed);
            }
            //GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            else if (this.IsShowingPS4Settings)
            {
                this.DrawSetting("Mono environment variables:", settings.PS4MonoEnvVars);
                this.DrawSetting("Enable Player Prefs support:", settings.PS4EnablePlayerPrefsSupport);
            }
            GUILayout.EndVertical();
            if (Event.current.type == EventType.Repaint) this._codeSettingsRect = GUILayoutUtility.GetLastRect();
        }

        private void DrawGraphicsSettings(BuildInfo buildReportToDisplay, UnityBuildSettings settings, UnityBuildReport unityBuildReport)
        {
            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;

            GUILayout.BeginVertical(GUIContent.none, groupStyle, NoExpandWidth);
            this.DrawSettingsGroupTitle("Graphics Settings");

            this.DrawSetting("Use 32-bit display buffer:", settings.Use32BitDisplayBuffer);
            this.DrawSetting("Rendering path:", settings.RenderingPathUsed);
            this.DrawSetting("Color space:", settings.ColorSpaceUsed);
            this.DrawSetting("Use multi-threaded rendering:", settings.UseMultithreadedRendering);
            this.DrawSetting("Use graphics jobs:", settings.UseGraphicsJobs);
            this.DrawSetting("Graphics jobs mode:", settings.GraphicsJobsType);
            this.DrawSetting("Use GPU skinning:", settings.UseGPUSkinning);
            this.DrawSetting("Enable Virtual Reality Support:", settings.EnableVirtualRealitySupport);

            #if UNITY_2020_2_OR_NEWER && !UNITY_2023_1_OR_NEWER
            if (unityBuildReport != null)
                this.DrawSetting("Enable Shader Livelink Support:",
                    unityBuildReport.HasBuildOption(BuildOptions.ShaderLivelinkSupport));
            #endif

            if (buildReportToDisplay.IsUnityVersionAtLeast(5, 2, 0)) this.DrawSetting("Graphics APIs Used:", settings.GraphicsAPIsUsed);

            GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

            if (this.IsShowingMobileSettings)
            {
                this.DrawSetting("Default interface orientation:", settings.MobileDefaultOrientationUsed);

                this.DrawSetting("Use OS screen auto-rotate:", settings.MobileEnableOSAutorotation);
                this.DrawSetting("Auto-rotate to portrait:", settings.MobileEnableAutorotateToPortrait);
                this.DrawSetting("Auto-rotate to reverse portrait:", settings.MobileEnableAutorotateToReversePortrait);
                this.DrawSetting("Auto-rotate to landscape left:", settings.MobileEnableAutorotateToLandscapeLeft);
                this.DrawSetting("Auto-rotate to landscape right:", settings.MobileEnableAutorotateToLandscapeRight);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingStandaloneSettings)
            {
                var standaloneScreenSize =
                    string.Format("{0} x {1}", settings.StandaloneDefaultScreenWidth.ToString(), settings.StandaloneDefaultScreenHeight.ToString());
                this.DrawSetting("Default screen size:", standaloneScreenSize);
                this.DrawSetting("Resolution dialog:", settings.StandaloneResolutionDialogSettingUsed);

                // removed in Unity 2018
                if (buildReportToDisplay.IsUnityVersionAtLeast(2017, 0, 0)) this.DrawSetting("Full-screen by default:", settings.StandaloneFullScreenByDefault);

                this.DrawSetting("Resizable window:", settings.StandaloneEnableResizableWindow);

                // added in Unity 2018
                if (buildReportToDisplay.IsUnityVersionAtLeast(2018, 0, 0)) this.DrawSetting("Fullscreen Mode:", settings.StandaloneFullScreenModeUsed);

                if (this.IsShowingWindowsDesktopSettings)
                {
                    // not needed in Unity 5.3 since settings.GraphicsAPIsUsed shows better information
                    if (buildReportToDisplay.IsUnityVersionAtMost(5, 2, 0)) this.DrawSetting("Use Direct3D11 if available:", settings.WinUseDirect3D11IfAvailable);

                    // removed in 2017
                    if (buildReportToDisplay.IsUnityVersionAtLeast(5, 0, 0)) this.DrawSetting("Direct3D9 Fullscreen Mode:", settings.WinDirect3D9FullscreenModeUsed);

                    // removed in 2018
                    if (buildReportToDisplay.IsUnityVersionAtLeast(2017, 0, 0)) this.DrawSetting("Direct3D11 Fullscreen Mode:", settings.WinDirect3D11FullscreenModeUsed);

                    this.DrawSetting("Visible in background (for Fullscreen Windowed mode):", settings.VisibleInBackground);
                }
                else if (this.IsShowingMacSettings)
                    // removed in 2018
                {
                    if (buildReportToDisplay.IsUnityVersionAtLeast(2017, 0, 0))
                    {
                        this.DrawSetting("Fullscreen mode:", settings.MacFullscreenModeUsed);
                        GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
                    }
                }

                this.DrawSetting("Allow OS switching between full-screen and window mode:",
                    settings.StandaloneAllowFullScreenSwitch);
                this.DrawSetting("Darken secondary monitors on full-screen:", settings.StandaloneCaptureSingleScreen);
                this.DrawSetting("Force single instance:", settings.StandaloneForceSingleInstance);

                this.DrawSetting("Stereoscopic Rendering:", settings.StandaloneUseStereoscopic3d);
                this.DrawSetting("Supported aspect ratios:", settings.AspectRatiosAllowed);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }

            if (this.IsShowingWebPlayerSettings)
            {
                var webScreenSize = string.Format("{0} x {1}", settings.WebPlayerDefaultScreenWidth.ToString(), settings.WebPlayerDefaultScreenHeight.ToString());
                this.DrawSetting("Screen size:", webScreenSize);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingiOSSettings)
            {
                if (buildReportToDisplay.IsUnityVersionAtMost(5, 2, 0))
                    // Unity 5.3 has a Screen.resolutions but I don't know
                    // which of those in the array would be the iOS target resolution
                    this.DrawSetting("Target resolution:", settings.iOSTargetResolution);

                if (buildReportToDisplay.IsUnityVersionAtMost(5, 1, 0))
                    // not used in Unity 5.2 since settings.GraphicsAPIsUsed shows better information
                    this.DrawSetting("Target graphics:", settings.iOSTargetGraphics);

                this.DrawSetting("App icon pre-rendered:", settings.iOSIsIconPrerendered);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingAndroidSettings)
            {
                if (buildReportToDisplay.IsUnityVersionAtMost(4, 0, 0)) this.DrawSetting("Use 24-bit depth buffer:", settings.AndroidUse24BitDepthBuffer);

                if (buildReportToDisplay.IsUnityVersionAtLeast(5, 0, 0)) this.DrawSetting("Disable depth and stencil buffers:", settings.AndroidDisableDepthAndStencilBuffers);

                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            else if (this.IsShowingPS4Settings)
            {
                this.DrawSetting("Video out pixel format:", settings.PS4VideoOutPixelFormat);
                this.DrawSetting("Video out resolution:", settings.PS4VideoOutResolution);
                GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
            }
            GUILayout.EndVertical();
            if (Event.current.type == EventType.Repaint) this._graphicsSettingsRect = GUILayoutUtility.GetLastRect();
        }

        private void DrawPackageSettings(BuildInfo buildReportToDisplay, UnityBuildSettings settings)
        {
            var packageList        = settings.PackageEntries;
            var builtInPackageList = settings.BuiltInPackageEntries;

            var packageListIsEmpty        = packageList == null || packageList.Count == 0;
            var builtInPackageListIsEmpty = builtInPackageList == null || builtInPackageList.Count == 0;

            if (packageListIsEmpty && builtInPackageListIsEmpty) return;

            var nameStyle                    = GUI.skin.FindStyle(Settings.SETTING_NAME_STYLE_NAME);
            if (nameStyle == null) nameStyle = GUI.skin.label;

            var valueStyle                     = GUI.skin.FindStyle(Settings.SETTING_VALUE_STYLE_NAME);
            if (valueStyle == null) valueStyle = GUI.skin.label;

            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;

            GUILayout.BeginVertical(GUIContent.none, groupStyle, NoExpandWidth);

            if (!packageListIsEmpty)
            {
                this.DrawSettingsGroupTitle("Packages");
                for (int n = 0, len = packageList.Count; n < len; ++n)
                {
                    if (!string.IsNullOrEmpty(packageList[n].DisplayName))
                    {
                        if (!string.IsNullOrEmpty(packageList[n].Location) && packageList[n].Location.EndsWith(".git") && packageList[n].VersionUsed.Length > 7)
                        {
                            // show commit hash as short
                            GUILayout.BeginHorizontal(GUIContent.none, groupStyle, NoExpandWidth);
                            GUILayout.Label(packageList[n].DisplayName, nameStyle);
                            GUILayout.Space(4);
                            GUILayout.TextField(packageList[n].VersionUsed.Substring(0, DEFAULT_SHORT_COMMIT_HASH_LENGTH_DISPLAYED), valueStyle);
                            GUILayout.Space(4);
                            this.DrawPackagePingButton(packageList[n]);
                            GUILayout.EndHorizontal();
                            GUILayout.TextField(packageList[n].PackageName, valueStyle);
                        }
                        else if (packageList[n].VersionUsed.Length <= 10)
                        {
                            // version is short enough, put it in the same line as the Package Name
                            GUILayout.BeginHorizontal(GUIContent.none, groupStyle, NoExpandWidth);
                            GUILayout.Label(packageList[n].DisplayName, nameStyle);
                            GUILayout.Space(4);
                            GUILayout.TextField(packageList[n].VersionUsed, valueStyle);
                            GUILayout.Space(4);
                            this.DrawPackagePingButton(packageList[n]);
                            GUILayout.EndHorizontal();
                            GUILayout.TextField(packageList[n].PackageName, valueStyle);
                        }
                        else
                        {
                            // version is too long, put it as a 2nd line after the Display Name
                            GUILayout.Label(packageList[n].DisplayName, nameStyle);
                            GUILayout.TextField(packageList[n].VersionUsed, valueStyle);
                            GUILayout.TextField(packageList[n].PackageName, valueStyle);
                            this.DrawPackagePingButton(packageList[n]);
                        }
                    }
                    else
                    {
                        // no display name
                        if (packageList[n].VersionUsed.Length <= 10)
                        {
                            // version is short enough, put it in the same line as the Package Name
                            GUILayout.BeginHorizontal(GUIContent.none, groupStyle, NoExpandWidth);
                            GUILayout.TextField(packageList[n].PackageName, nameStyle);
                            GUILayout.Space(4);
                            GUILayout.TextField(packageList[n].VersionUsed, valueStyle);
                            GUILayout.Space(4);
                            this.DrawPackagePingButton(packageList[n]);
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            // version is too long, put it as a 2nd line after the Package Name
                            GUILayout.TextField(packageList[n].PackageName, nameStyle);
                            GUILayout.TextField(packageList[n].VersionUsed, valueStyle);
                            this.DrawPackagePingButton(packageList[n]);
                        }
                    }

                    if (!string.IsNullOrEmpty(packageList[n].Location) && packageList[n].Location != UnityBuildSettingsUtility.DEFAULT_REGISTRY_URL) GUILayout.TextField(packageList[n].Location, valueStyle);

                    GUILayout.Space(10);
                }

                if (!builtInPackageListIsEmpty) GUILayout.Space(14);
            }

            if (!builtInPackageListIsEmpty)
            {
                this.DrawSettingsGroupTitle("Built-In Packages");
                for (int n = 0, len = builtInPackageList.Count; n < len; ++n)
                {
                    if (!string.IsNullOrEmpty(builtInPackageList[n].DisplayName))
                    {
                        GUILayout.Label(builtInPackageList[n].DisplayName, nameStyle);
                        GUILayout.TextField(builtInPackageList[n].PackageName, valueStyle);
                    }
                    else
                        // no display name
                    {
                        GUILayout.Label(builtInPackageList[n].PackageName, nameStyle);
                    }

                    GUILayout.Space(5);
                }
            }

            GUILayout.EndVertical();
            if (Event.current.type == EventType.Repaint) this._pathSettingsRect = GUILayoutUtility.GetLastRect();
        }

        private void DrawPackagePingButton(UnityBuildSettings.PackageEntry packageEntry)
        {
            if (!string.IsNullOrEmpty(packageEntry.LocalPath))
            {
                if (GUILayout.Button("Ping", "MiniButton")) Utility.PingAssetInProject(string.Format("Packages/{0}/package.json", packageEntry.PackageName));
                if (GUILayout.Button("Explore", "MiniButton")) Util.OpenInFileBrowser(packageEntry.LocalPath);
            }
        }

        private void DrawPathSettings(BuildInfo buildReportToDisplay, UnityBuildSettings settings)
        {
            var groupStyle                     = GUI.skin.FindStyle("ProjectSettingsGroup");
            if (groupStyle == null) groupStyle = GUI.skin.label;

            GUILayout.BeginVertical(GUIContent.none, groupStyle, NoExpandWidth);
            this.DrawSettingsGroupTitle("Paths");

            this.DrawSetting2Lines("Unity path:", buildReportToDisplay.EditorAppContentsPath);
            this.DrawSetting2Lines("Project path:", buildReportToDisplay.ProjectAssetsPath);
            this.DrawSetting2Lines("Build path:", buildReportToDisplay.BuildFilePath);
            GUILayout.EndVertical();
            if (Event.current.type == EventType.Repaint) this._pathSettingsRect = GUILayoutUtility.GetLastRect();
        }

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
            if (buildReportToDisplay == null)
            {
                requestRepaint = false;
                return;
            }

            var b = ReportGenerator.GetBuildSettingCategoryFromBuildValues(buildReportToDisplay);
            this._buildTargetOfReport = UnityBuildSettingsUtility.GetReadableBuildSettingCategory(b);

            var settings = buildReportToDisplay.UnityBuildSettings;

            if (settings == null)
            {
                Utility.DrawCentralMessage(position, "No \"Project Settings\" recorded in this build report.");
                requestRepaint = false;
                return;
            }

            var topBarBgStyle                        = GUI.skin.FindStyle(Settings.TOP_BAR_BG_STYLE_NAME);
            if (topBarBgStyle == null) topBarBgStyle = GUI.skin.box;

            var topBarLabelStyle                           = GUI.skin.FindStyle(Settings.TOP_BAR_LABEL_STYLE_NAME);
            if (topBarLabelStyle == null) topBarLabelStyle = GUI.skin.label;

            var fileFilterPopupStyle                               = GUI.skin.FindStyle(Settings.FILE_FILTER_POPUP_STYLE_NAME);
            if (fileFilterPopupStyle == null) fileFilterPopupStyle = GUI.skin.label;

            // ----------------------------------------------------------
            // top bar

            GUILayout.Space(1);
            GUILayout.BeginHorizontal();

            GUILayout.Label(" ", topBarBgStyle);

            GUILayout.Space(8);
            GUILayout.Label("Build Target: ", topBarLabelStyle);

            this.InitializeDropdownBoxLabelsIfNeeded();
            this._selectedSettingsIdxFromDropdownBox = EditorGUILayout.Popup(this._selectedSettingsIdxFromDropdownBox,
                this._settingDropdownBoxLabels,
                fileFilterPopupStyle);
            GUILayout.Space(15);

            GUILayout.Label(string.Format("Note: Project was built in {0} target", this._buildTargetOfReport), topBarLabelStyle);
            GUILayout.FlexibleSpace();

            BuildReportTool.Options.ShowProjectSettingsInMultipleColumns = GUILayout.Toggle(BuildReportTool.Options.ShowProjectSettingsInMultipleColumns, "Multiple Columns");
            GUILayout.Space(30);

            GUILayout.EndHorizontal();

            this._settingsShown = UnityBuildSettingsUtility.GetSettingsCategoryFromIdx(this._selectedSettingsIdxFromDropdownBox);

            // ----------------------------------------------------------

            var showMultiColumn = BuildReportTool.Options.ShowProjectSettingsInMultipleColumns;

            this._scrollPos = GUILayout.BeginScrollView(this._scrollPos);

            GUILayout.BeginHorizontal(NoExpandWidth);

            // left padding
            GUILayout.Space(10);
            GUILayout.BeginVertical(NoExpandWidth);

            // top padding
            GUILayout.Space(10);

            // columns
            GUILayout.BeginHorizontal(NoExpandWidth);

            // column 1
            GUILayout.BeginVertical(NoExpandWidth);

            var putCodeSettingsInColumn2     = showMultiColumn && this._codeSettingsRect.width > 0 && this._column1Width + this._codeSettingsRect.width < position.width;
            var putGraphicsSettingsInColumn3 = showMultiColumn && this._graphicsSettingsRect.width > 0 && this._column1Width + this._codeSettingsRect.width + this._graphicsSettingsRect.width < position.width;

            // =================================================================
            this.DrawProjectSettings(buildReportToDisplay, settings);
            GUILayout.Space(SETTINGS_GROUP_SPACING);

            // =================================================================
            this.DrawPathSettings(buildReportToDisplay, settings);
            GUILayout.Space(SETTINGS_GROUP_SPACING);

            // =================================================================
            this.DrawBuildSettings(buildReportToDisplay, settings, unityBuildReport);
            GUILayout.Space(SETTINGS_GROUP_SPACING);

            // =================================================================
            this.DrawRuntimeSettings(buildReportToDisplay, settings);
            GUILayout.Space(SETTINGS_GROUP_SPACING);

            // =================================================================
            this.DrawDebugSettings(buildReportToDisplay, settings, unityBuildReport);
            GUILayout.Space(SETTINGS_GROUP_SPACING);

            if (putGraphicsSettingsInColumn3)
            {
                GUILayout.EndVertical(); // end of column 1

                // column 2
                GUILayout.BeginVertical(NoExpandWidth);
            }
            // =================================================================
            this.DrawGraphicsSettings(buildReportToDisplay, settings, unityBuildReport);
            GUILayout.Space(SETTINGS_GROUP_SPACING);

            // =================================================================
            this.DrawPackageSettings(buildReportToDisplay, settings);
            GUILayout.Space(SETTINGS_GROUP_SPACING);

            if (putCodeSettingsInColumn2)
            {
                GUILayout.EndVertical(); // end of column 1 or 2

                // column 2 or 2
                GUILayout.BeginVertical(NoExpandWidth);
            }
            // =================================================================
            this.DrawCodeSettings(buildReportToDisplay, settings);
            GUILayout.Space(SETTINGS_GROUP_SPACING);

            GUILayout.EndVertical();   // end of last column
            GUILayout.EndHorizontal(); // end columns

            // bottom padding
            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            requestRepaint = false;

            // =================================================================

            if (Event.current.type == EventType.Repaint)
            {
                this._column1Width = 0;
                this._column1Width = Mathf.Max(this._projectSettingsRect.width, this._column1Width);
                this._column1Width = Mathf.Max(this._pathSettingsRect.width, this._column1Width);
                this._column1Width = Mathf.Max(this._buildSettingsRect.width, this._column1Width);
                this._column1Width = Mathf.Max(this._runtimeSettingsRect.width, this._column1Width);
                this._column1Width = Mathf.Max(this._debugSettingsRect.width, this._column1Width);
                //_column1Width = Mathf.Max(_graphicsSettingsRect.width, _column1Width);
            }
            //_column1Width = Mathf.Max(_codeSettingsRect.width, _column1Width);
        }
    }
}

public static class ScriptReference
{
    //
    //   Example usage:
    //
    //   if (GUILayout.Button("doc"))
    //   {
    //      ScriptReference.GoTo("EditorUserBuildSettings.development");
    //   }
    //
    public static void GoTo(string pageName)
    {
        var pageUrl = "file:///";

        pageUrl += EditorApplication.applicationContentsPath;

        // unity 3
        pageUrl += "/Documentation/Documentation/ScriptReference/";

        pageUrl += pageName.Replace(".", "-");
        pageUrl += ".html";

        Debug.Log("going to: " + pageUrl);

        Application.OpenURL(pageUrl);
    }
}