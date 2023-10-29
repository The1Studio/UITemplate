namespace TheOneStudio.UITemplate.UITemplate.Leaderboard.Models
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;

    
    public class UITemplateLeaderBoardData : ILocalData, IUITemplateLocalData
    {
        [OdinSerialize] public Dictionary<int, Dictionary<int, Dictionary<int, int>>> YearToMonthToDayToHighestScore { get; set; } = new();
        [OdinSerialize] public Dictionary<int, int>                                   WeekIndexToHighestScore        { get; set; } = new();
        [OdinSerialize] public int                                                    AllTimeHighestScore            { get; set; }


        public void Init() { }

        public Type ControllerType => typeof(UITemplateLeaderBoardDataController);
    }
}