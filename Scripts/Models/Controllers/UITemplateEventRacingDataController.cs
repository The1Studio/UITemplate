namespace TheOneStudio.HyperCasual.GamePlay.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.Utilities.Extension;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using ServiceImplementation.FireBaseRemoteConfig;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Extension;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Services.CountryFlags.CountryFlags.Scripts;
    using UnityEngine;
    using Zenject;
    using Random = UnityEngine.Random;

    public class UITemplateEventRacingDataController : IUITemplateControllerData, IInitializable
    {
        private const string RacingEventMaxScoreKey = "racing_event_max_score";

        public const int TotalRacingPlayer = 5;

        private const string CountryFlagsPrefab = "CountryFlags";

        #region inject

        private readonly UITemplateEventRacingData         uiTemplateEventRacingData;
        private readonly IGameAssets                       gameAssets;
        private readonly UITemplateInventoryDataController uiTemplateInventoryDataController;
        private readonly SignalBus                         signalBus;
        private readonly IRemoteConfig                     remoteConfig;
        private readonly GameEventsSetting                 gameEventsSetting;

        #endregion

        private CountryFlags CountryFlags
        {
            get
            {
                if (this.countryFlags == null)
                {
                    this.countryFlags = this.gameAssets.LoadAssetAsync<GameObject>(CountryFlagsPrefab).WaitForCompletion()
                        .Spawn().GetComponent<CountryFlags>();
                }

                return this.countryFlags;
            }
            set => this.countryFlags = value;
        }

        private CountryFlags countryFlags;
        public  int          RacingScoreMax { get; set; }

        public UITemplateEventRacingDataController(UITemplateEventRacingData uiTemplateEventRacingData,
            IGameAssets gameAssets, UITemplateInventoryDataController uiTemplateInventoryDataController,
            SignalBus signalBus, IRemoteConfig remoteConfig, GameEventsSetting gameEventsSetting)
        {
            this.uiTemplateEventRacingData         = uiTemplateEventRacingData;
            this.gameAssets                        = gameAssets;
            this.uiTemplateInventoryDataController = uiTemplateInventoryDataController;
            this.signalBus                         = signalBus;
            this.remoteConfig                      = remoteConfig;
            this.gameEventsSetting                 = gameEventsSetting;
        }

        public int      YourOldShowScore => this.uiTemplateEventRacingData.YourOldShowScore;
        public int      YourNewScore     => this.uiTemplateInventoryDataController.GetCurrencyValue(this.gameEventsSetting.RacingConfig.RacingCurrency);
        public int      YourIndex        => this.uiTemplateEventRacingData.yourIndex;
        public long     RemainSecond     => (long)(this.uiTemplateEventRacingData.endDate - DateTime.Now).TotalSeconds;
        public DateTime StartDate        => this.uiTemplateEventRacingData.startDate;
        public DateTime EndDate
        {
            get
            {
                if (this.uiTemplateEventRacingData.endDate == DateTime.MinValue)
                {
                    this.uiTemplateEventRacingData.endDate = this.uiTemplateEventRacingData.startDate.AddDays(this.gameEventsSetting.RacingConfig.RacingDay - Random.Range(0, 1.5f));
                }
                return this.uiTemplateEventRacingData.endDate;
            }
        }

        public void UpdateUserOldShowScore()
        {
            this.uiTemplateEventRacingData.YourOldShowScore =
                this.uiTemplateInventoryDataController.GetCurrencyValue(this.gameEventsSetting.RacingConfig.RacingCurrency);
        }

        public void Initialize()
        {
            this.signalBus.Subscribe<RemoteConfigFetchedSucceededSignal>(this.OnFetchSucceedHandler);
        }

        private void OnFetchSucceedHandler(RemoteConfigFetchedSucceededSignal obj)
        {
            this.RacingScoreMax = this.remoteConfig.GetRemoteConfigIntValue(RacingEventMaxScoreKey, this.gameEventsSetting.racingConfig.RacingScoreMax);
        }

        public void AddPlayScore(int addedScore)
        {
            var yourIndex = this.uiTemplateEventRacingData.yourIndex;
            this.uiTemplateEventRacingData.playerIndexToData[yourIndex].Score += addedScore;
        }

        private void AddScore(int playIndex, int addedScore) { this.uiTemplateEventRacingData.playerIndexToData[playIndex].Score += addedScore; }

        public Sprite GetCountryFlagSprite(string countryCode) => this.CountryFlags.GetFlag(countryCode);

        public UITemplateRacingPlayerData GetPlayerData(int playIndex)
        {
            var isYou = playIndex == this.uiTemplateEventRacingData.yourIndex;
            var racingPlayerData = this.uiTemplateEventRacingData.playerIndexToData.GetOrAdd(playIndex, () =>
                new UITemplateRacingPlayerData
                {
                    Name  = isYou ? "You" : NVJOBNameGen.GiveAName(Random.Range(1, 8)),
                    Score = 0,
                    CountryCode =
                        isYou ? CountryFlags.GetCountryCodeByDeviceLang() : this.CountryFlags.RandomCountryCode(),
                    IconAddressable = this.gameEventsSetting.RacingConfig.IconAddressableSet.PickRandom()
                });

            if (racingPlayerData.IconAddressable.IsNullOrEmpty())
            {
                racingPlayerData.IconAddressable = this.gameEventsSetting.RacingConfig.IconAddressableSet.PickRandom();
            }

            if (isYou) racingPlayerData.Score = this.uiTemplateInventoryDataController.GetCurrencyValue(this.gameEventsSetting.RacingConfig.RacingCurrency);

            return racingPlayerData;
        }


        //Simulate score for all player except yourIndex and add to playerIndexToScore with to make them reach GameEventRacingValue.RacingMaxProgressionPercent of total score as max
        //The score of players will depend the time from lastRandomTime to now the gameEventRacingData.startDate (starting time of event) and gameEventRacingData.endDate (ending time of event
        public Dictionary<int, (int, int)> SimulatePlayerScore()
        {
            var maxScore              = this.RacingScoreMax * this.gameEventsSetting.racingConfig.RacingMaxProgressionPercent;
            var playIndexToAddedScore = new Dictionary<int, (int, int)>();

            var currentTime = DateTime.Now;

            foreach (var (playerIndex, racingPlayerData) in this.uiTemplateEventRacingData.playerIndexToData)
            {
                if (playerIndex == this.uiTemplateEventRacingData.yourIndex) continue;

                //calculate input data
                var totalSecondsFromLastSimulation =
                    (currentTime - this.uiTemplateEventRacingData.lastRandomTime).TotalSeconds;
                var totalSecondsUntilEndEventFromLastSimulation =
                    (this.uiTemplateEventRacingData.endDate - currentTime).TotalSeconds;
                var maxRandomScore = maxScore - racingPlayerData.Score;

                //calculate random score
                var randomAddingScore = (int)(totalSecondsFromLastSimulation /
                    totalSecondsUntilEndEventFromLastSimulation * maxRandomScore);
                randomAddingScore = (int)(randomAddingScore * Random.Range(0.3f, 1.1f));
                playIndexToAddedScore.Add(playerIndex,
                    (racingPlayerData.Score, racingPlayerData.Score + randomAddingScore));
                this.AddScore(playerIndex, randomAddingScore);
            }

            this.uiTemplateEventRacingData.lastRandomTime = currentTime;
            return playIndexToAddedScore;
        }
    }
}