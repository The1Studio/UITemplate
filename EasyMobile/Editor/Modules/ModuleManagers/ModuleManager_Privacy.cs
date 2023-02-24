using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace EasyMobile.Editor
{
    internal class ModuleManager_Privacy : CompositeModuleManager
    {
        private const string iOSATTLibPath = EM_Constants.RootPath + "/Plugins/iOS/libEasyMobile_AppTrackingTransparency.a";

        #region Singleton

        private static ModuleManager_Privacy sInstance;

        private ModuleManager_Privacy()
        {
        }

        public static ModuleManager_Privacy Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new ModuleManager_Privacy();
                return sInstance;
            }
        }

        #endregion

        #region implemented abstract members of ModuleManager

        public override Module SelfModule
        {
            get
            {
                return Module.Privacy;
            }
        }

        #endregion

        #region implemented abstract members of CompositeModuleManager

        public override List<Submodule> SelfSubmodules
        {
            get
            {
                return new List<Submodule> { Submodule.AppTracking };
            }
        }

        public override List<string> AndroidManifestTemplatePathsForSubmodule(Submodule submod)
        {
            return null;
        }

        public override IAndroidPermissionRequired AndroidPermissionHolderForSubmodule(Submodule submod)
        {
            return null;
        }

        public override IIOSInfoItemRequired iOSInfoItemsHolderForSubmodule(Submodule submod)
        {
            switch (submod)
            {
                case Submodule.AppTracking:
                    return EM_Settings.Privacy.AppTracking as IIOSInfoItemRequired;
                default:
                    return null;
            }
        }

        protected override void InternalEnableSubmodule(Submodule submod)
        {
            // Include iOS native lib for Contacts.
            var pluginImporter = AssetImporter.GetAtPath(iOSATTLibPath) as PluginImporter;
            pluginImporter.ClearSettings();
            pluginImporter.SetCompatibleWithAnyPlatform(false);
            pluginImporter.SetCompatibleWithPlatform(BuildTarget.iOS, true);

            // Define scripting symbol.
            GlobalDefineManager.SDS_AddDefineOnAllPlatforms(EM_ScriptingSymbols.AppTrackingSubmodule);
        }

        protected override void InternalDisableSubmodule(Submodule submod)
        {
            // Exclude iOS native lib for Contacts.
            var pluginImporter = AssetImporter.GetAtPath(iOSATTLibPath) as PluginImporter;
            pluginImporter.ClearSettings();
            pluginImporter.SetCompatibleWithAnyPlatform(false);

            // Remove associated scripting symbol on all platforms it was defined.
            GlobalDefineManager.SDS_RemoveDefineOnAllPlatforms(EM_ScriptingSymbols.AppTrackingSubmodule);
        }

        #endregion
    }
}
