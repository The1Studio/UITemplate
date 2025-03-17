namespace TheOneStudio.HyperCasual.GamePlay.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateEventRacingData : ILocalData, IUITemplateLocalData
    {
        [JsonProperty]                 internal DateTime                                    startDate;
        [JsonProperty]                 internal DateTime                                    endDate;
        [JsonProperty]                 internal DateTime                                    lastRandomTime;
        [JsonProperty]                 internal int                                         YourOldShowScore;
        [JsonProperty]                 internal int                                         yourIndex;
        [JsonProperty] [OdinSerialize] internal Dictionary<int, UITemplateRacingPlayerData> playerIndexToData = new();

        //set startDate at start of local today and endDate at end of 7 days from today
        //set playerIndexToScore to empty dictionary
        //set yourIndex to random from 1 to 5
        public void Init()
        {
            this.startDate         = DateTime.Today.AddDays(-0.75f);
            this.lastRandomTime    = this.startDate;
            this.endDate           = DateTime.MinValue;
            this.playerIndexToData = new();
            this.yourIndex         = new Random().Next(0, UITemplateEventRacingDataController.TotalRacingPlayer);
        }

        public Type ControllerType => typeof(UITemplateEventRacingDataController);
    }

    public class UITemplateRacingPlayerData
    {
        [JsonProperty] public string Name;
        [JsonProperty] public string CountryCode;
        [JsonProperty] public string IconAddressable;
        [JsonProperty] public int    Score;
        [JsonProperty] public bool   IsClaimItem;
    }
}