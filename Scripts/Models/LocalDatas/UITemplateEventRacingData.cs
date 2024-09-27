namespace TheOneStudio.HyperCasual.GamePlay.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Newtonsoft.Json;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Configs.GameEvents;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateEventRacingData : ILocalData, IUITemplateLocalData
    {
        public           DateTime          startDate;
        public           DateTime          endDate;
        public           DateTime          lastRandomTime;

        public int YourOldShowScore;
        public int yourIndex;

        [OdinSerialize] public Dictionary<int, UITemplateRacingPlayerData> playerIndexToData = new();

        //set startDate at start of local today and endDate at end of 7 days from today
        //set playerIndexToScore to empty dictionary
        //set yourIndex to random from 1 to 5
        public void Init()
        {
            this.startDate         = DateTime.Today.AddDays(-0.75f);
            this.lastRandomTime    = this.startDate;
            this.endDate           = DateTime.MinValue;
            this.playerIndexToData = new Dictionary<int, UITemplateRacingPlayerData>();
            this.yourIndex         = new Random().Next(0, UITemplateEventRacingDataController.TotalRacingPlayer);
        }

        public Type ControllerType => typeof(UITemplateEventRacingDataController);
    }

    public class UITemplateRacingPlayerData
    {
        public string Name;
        public string CountryCode;
        public string IconAddressable;
        public int    Score;
        public bool   IsClaimItem;
    }
}