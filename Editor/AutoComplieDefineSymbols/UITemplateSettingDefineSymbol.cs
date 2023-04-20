namespace UITemplate.Editor.AutoComplieDefineSymbols
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using UnityEditor;
    using UnityEditor.Compilation;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "SettingDefineSymbol", menuName = "Configs/SettingDefineSymbols", order = 0)]
    public class UITemplateSettingDefineSymbol : ScriptableObject
    {
        static string                        SettingName = "SettingDefineSymbol";
        static UITemplateSettingDefineSymbol instance;

        public static UITemplateSettingDefineSymbol Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LoadSettingsAsset();

                    if (instance == null)
                    {
                        instance = CreateInstance<UITemplateSettingDefineSymbol>(); // Create a dummy scriptable object for temporary use.
                        SaveToResources();
                    }
                }

                return instance;
            }
        }

        private static UITemplateSettingDefineSymbol LoadSettingsAsset() { return Resources.Load(SettingName) as UITemplateSettingDefineSymbol; }

        private static void SaveToResources()
        {
            Debug.Log($"No found SettingDefineSymbol, create new one");
            AssetDatabase.CreateAsset(instance, $"Assets/Resources/{SettingName}.asset");
        }

        #region Field

        [HideInInspector] public bool IsEnable;

        public AnalyticAndTracking AnalyticAndTracking;
        public Monetization        Monetization;

        [HideInInspector] [AutoSettingDefine(true)]
        public Partner Partner;

        public UITemplateGameAndServices UITemplateGameAndServices;

        public List<CustomDefineSymbol> CustomDefineSymbols;

        #endregion

        public void Apply()
        {
            var currentDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var totalDefine   = currentDefine.Split(";").ToList();
            this.AddCustomDefineSymbol(totalDefine);
        }

        public void AddCustomDefineSymbol(List<string> originDefine)
        {
            if (!this.IsEnable)
            {
                Debug.LogError($"Not Enable");

                return;
            }

            var totalCurrentSettingDefine = new Dictionary<string, bool>();
            //get all define symbol from all class
            var analyticProperties     = this.AnalyticAndTracking.GetType().GetFields();
            var monetizationProperties = this.Monetization.GetType().GetFields();
            var partnerProperties      = this.Partner.GetType().GetFields();
            var gameAndServices        = this.UITemplateGameAndServices.GetType().GetFields();

            this.GetDefine(analyticProperties, this.AnalyticAndTracking, totalCurrentSettingDefine);
            this.GetDefine(monetizationProperties, this.Monetization, totalCurrentSettingDefine);
            this.GetDefine(partnerProperties, this.Partner, totalCurrentSettingDefine);
            this.GetDefine(gameAndServices, this.UITemplateGameAndServices, totalCurrentSettingDefine);

            //get all define symbol from custom define symbol
            foreach (var customDefineSymbol in this.CustomDefineSymbols)
            {
                if (customDefineSymbol.DefineSymbolName.IsNullOrEmpty()) continue;
                totalCurrentSettingDefine.Add(customDefineSymbol.DefineSymbolName, customDefineSymbol.IsEnable);
            }

            var totalDefineList = new List<string>(originDefine);

            //add define symbol to total define symbol
            foreach (var data in totalCurrentSettingDefine)
            {
                if (totalDefineList.Contains(data.Key) && !data.Value)
                {
                    totalDefineList.Remove(data.Key);
                }
                else if (!totalDefineList.Contains(data.Key) && data.Value)
                {
                    this.CheckToAddDefineSymbol(data.Key, totalDefineList);
                }
            }

            //default define for all project
            this.CheckToAddDefineSymbol("TextMeshPro", totalDefineList);
            this.CheckToAddDefineSymbol("ODIN_INSPECTOR", totalDefineList);
            this.CheckToAddDefineSymbol("ODIN_INSPECTOR_3", totalDefineList);
            this.CheckToAddDefineSymbol("ADDRESSABLES_ENABLED", totalDefineList);
            var finalDefine = string.Join(";", totalDefineList);

            //compare totalDefineList list and current originDefine list
            if (!finalDefine.Equals(string.Join(";", originDefine)))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, finalDefine);
            }
        }

        private void CheckToAddDefineSymbol(string defineSymbolName, List<string> totalDefine)
        {
            if (!totalDefine.Contains(defineSymbolName))
            {
                totalDefine.Add(defineSymbolName);
            }
        }

        public void GetDefine(FieldInfo[] propertyInfos, object objectContain, Dictionary<string, bool> totalDic)
        {
            foreach (var p in propertyInfos)
            {
                var isEnable   = p.GetValue(objectContain);
                var headerName = p.GetCustomAttribute<HeaderAttribute>().header;
                totalDic.Add(headerName, (bool)isEnable);
            }
        }
    }

    [Serializable]
    public class AnalyticAndTracking
    {
        [Header("APPSFLYER")]              public bool appFlyer;
        [Header("ADJUST")]                 public bool adjust;
        [Header("FIREBASE_REMOTE_CONFIG")] public bool fireBaseRemoteConfig;
        [Header("FIREBASE_SDK_EXISTS")]    public bool fireBaseSDKExists;
    }

    [Serializable]
    public class Monetization
    {
        [Header("EM_ADMOB")]              public bool admob;
        [Header("EM_APPLOVIN")]           public bool appLovin;
        [Header("EM_IRONSOURCE")]         public bool ironSource;
        [Header("ADMOB_NATIVE_ADS")]      public bool admobNativeAds;
        [Header("ENABLE_IAP")]            public bool iap;
        [Header("FAKE_RESTORE_PURCHASE")] public bool fakeRestorePurchase;
    }

    [Serializable]
    public class Partner
    {
        [Header("WIDO")]   public bool widoAnalytics;
        [Header("ROCKET")] public bool rocketAnalytics;
        [Header("ADONE")]  public bool adOneAnalytics;
        [Header("ABI")]    public bool abiAnalytics;
    }

    [Serializable]
    public class UITemplateGameAndServices
    {
        [Header("CREATIVE")]              public bool creative;
        [Header("UITEMPLATE_DECORATION")] public bool decoration;
        [Header("SHOW_FPS")]              public bool showFPS;
        [Header("ENABLE_REPORTER")]       public bool showReporterRuntime;
    }

    [Serializable]
    public class CustomDefineSymbol
    {
        public string DefineSymbolName;
        public bool   IsEnable;
    }
}