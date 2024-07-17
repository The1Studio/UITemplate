namespace TheOneStudio.UITemplate.UITemplate.Configs.GameEvents
{
    using System.Linq;
    using Sirenix.OdinInspector;
    using TheOneStudio.UITemplate.UITemplate.Creative.Cheat;
    using TheOneStudio.UITemplate.UITemplate.Services.Vibration;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using System.IO;
    using ServiceImplementation.Configs.Editor;
#endif

    [CreateAssetMenu(fileName = nameof(GameFeaturesSetting),
        menuName = "TheOne/ScriptableObjects/SpawnGameFeatruesSettingConfig", order = 1)]
    public class GameFeaturesSetting : ScriptableObject
    {
        #region essential

        private const string DailyRewardSymbol  = "THEONE_DAILY_REWARD";
        private const string NoInternetSymbol   = "THEONE_NO_INTERNET";
        private const string RateUsSymbol       = "THEONE_STORE_RATING";
        private const string NotificationSymbol = "THEONE_NOTIFICATION";

        #endregion
        
        private const string IAPSymbol              = "THEONE_IAP";
        private const string RacingEventSymbol      = "THEONE_RACING_EVENT";
        private const string BadgeNotifySymbol      = "THEONE_BADGE_NOTIFY";
        private const string QuestSystemSymbol      = "THEONE_QUEST_SYSTEM";
        private const string FireBaseAuthSymbol     = "THEONE_FIREBASE_AUTH";
        private const string DailyQueueRewardSymbol = "THEONE_DAILY_QUEUE_REWARD";

        public static string ResourcePath = $"GameConfigs/{nameof(GameFeaturesSetting)}";

        #region Racing Event

        [OnValueChanged("OnChangeRacingEvent")]
        public bool enableRacingEvent;

        [SerializeField] [ShowIf("enableRacingEvent")] [BoxGroup("Racing Event")]
        public GameEventRacingConfig racingConfig;

        public GameEventRacingConfig RacingConfig => this.racingConfig;

        #endregion

        #region Daily Reward

        [OnValueChanged("OnChangeDailyReward")] [FoldoutGroup("Essential", expanded:true)]
        public bool enableDailyReward;

        [SerializeField] [ShowIf(nameof(enableDailyReward))] [BoxGroup("Daily Reward")]
        private DailyRewardConfig dailyRewardConfig;

        public DailyRewardConfig DailyRewardConfig => this.dailyRewardConfig;

        #endregion

        #region No Internet 
            
        [OnValueChanged("OnChangeNoInternet")] [FoldoutGroup("Essential", expanded:true)]
        public bool enableNoInternet;
        
        [SerializeField] [ShowIf(nameof(enableNoInternet))] [BoxGroup("No Internet")]
        private NoInternetConfig noInternetConfig;
        
        public NoInternetConfig NoInternetConfig => this.noInternetConfig;
        #endregion

        #region IAP

        [OnValueChanged("OnChangeIAP")] [FoldoutGroup("Essential", expanded:true)]
        public bool enableIAP;
        
        [SerializeField] [ShowIf(nameof(enableIAP))] [BoxGroup("IAP")]
        private IAPConfig iapConfig;
        
        public IAPConfig IAPConfig => this.iapConfig;

        #endregion
        
        #region Rate Us
        [OnValueChanged("OnChangeRateUs")] [FoldoutGroup("Essential", expanded:true)]
        public bool enableRateUs;
        
        [SerializeField] [ShowIf(nameof(enableRateUs))] [BoxGroup("Rate Us")]
        private RateUsConfig rateUsConfig;
        
        public RateUsConfig RateUsConfig => this.rateUsConfig;
        #endregion
        
        #region Notification
        [OnValueChanged("OnChangeNotification")] [FoldoutGroup("Essential", expanded:true)]
        public bool enableNotification;
        #endregion

        #region Badge Notify

        [OnValueChanged("OnChangeBadgeNotify")]
        public bool enableBadgeNotify;

        #endregion

        #region Quest

        [OnValueChanged("OnChangeQuestSystem")]
        public bool enableQuestSystem;

        [SerializeField] [ShowIf(nameof(enableQuestSystem))] [BoxGroup("Quest System")]
        private QuestSystemConfig questSystemConfig;

        public QuestSystemConfig QuestSystemConfig => this.questSystemConfig;

        #endregion

        #region Misc

        [FoldoutGroup("Misc Settings")] public bool enableInitHomeScreenManually = true;
        [FoldoutGroup("Misc Settings")] public bool showBottomBarWithBanner      = false; //When enable this, we will show bottom bar with banner in them same time, that make the bottom bar taller than normal

        #endregion

        #region Button Experience

        [FoldoutGroup("Button Experience")] [Tooltip("Set to None to disable")]
        public VibrationPresetType vibrationPresetType = VibrationPresetType.Selection;

        [FoldoutGroup("Button Experience")] [Tooltip("Set to empty to disable")]
        public string clickButtonSound = "click_button";

        [FoldoutGroup("Button Experience")] public bool enableScaleAnimationOnCLicked = true;

        #endregion

        #region Firebase

        [OnValueChanged("OnChangeFirebaseAuth")] [BoxGroup("Firebase")]
        public bool enableFirebaseAuth;

        #endregion

        #region Daily Queue Reward

        [OnValueChanged("OnChangeDailyQueueReward")] [BoxGroup("Daily Queue Reward")]
        public bool enableDailyQueueReward;

        #endregion

        #region Cheat Settings

        [BoxGroup("Cheat Settings")] public TheOneCheatActiveType cheatActiveBy = TheOneCheatActiveType.DrawTripleCircle;

        #endregion

#if UNITY_EDITOR
        
        private void OnChangeDailyReward() { EditorUtils.SetDefineSymbol(DailyRewardSymbol, this.enableDailyReward); }
        
        private void OnChangeNoInternet() { EditorUtils.SetDefineSymbol(NoInternetSymbol, this.enableNoInternet); }

        private void OnChangeNotification() { EditorUtils.SetDefineSymbol(NotificationSymbol, this.enableNotification); }

        private void OnChangeRateUs() { EditorUtils.SetDefineSymbol(RateUsSymbol, this.enableRateUs); }
        
        private void OnChangeIAP() { EditorUtils.SetDefineSymbol(IAPSymbol, this.enableIAP); }

        private void OnChangeRacingEvent() { EditorUtils.SetDefineSymbol(RacingEventSymbol, this.enableRacingEvent); }

        private void OnChangeBadgeNotify() { EditorUtils.SetDefineSymbol(BadgeNotifySymbol, this.enableBadgeNotify); }

        private void OnChangeQuestSystem() { EditorUtils.SetDefineSymbol(QuestSystemSymbol, this.enableQuestSystem); }

        private void OnChangeFirebaseAuth() { EditorUtils.SetDefineSymbol(FireBaseAuthSymbol, this.enableFirebaseAuth); }

        private async void OnChangeDailyQueueReward()
        {
            const string filePath = "Assets/Resources/BlueprintData/UITemplateDailyQueueOffer.csv";
            EditorUtils.SetDefineSymbol(DailyQueueRewardSymbol, this.enableDailyQueueReward);

            if (!this.enableDailyQueueReward) return;
            if (File.Exists(filePath)) return;

            await using (var writer = new StreamWriter(filePath, true))
            {
                await writer.WriteLineAsync("Day,OfferId,ItemId,ImageId,Value,IsRewardedAds");
            }

            AssetDatabase.Refresh();
        }
#endif
    }
}