#if THEONE_DAILY_QUEUE_REWARD
namespace TheOneStudio.UITemplate.UITemplate.Models.LocalDatas
{
    using System;
    using System.Collections.Generic;
    using GameFoundation.Scripts.Interfaces;
    using Sirenix.Serialization;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using UnityEngine.Scripting;

    [Preserve]
    public class UITemplateDailyQueueOfferData : ILocalData, IUITemplateLocalData
    {
        [OdinSerialize] internal DateTime                         LastOfferDate          { get; set; } = DateTime.MinValue;
        [OdinSerialize] internal Dictionary<string, RewardStatus> OfferToStatusDuringDay { get; set; } = new();
        [OdinSerialize] internal DateTime                         FirstTimeOpen          { get; set; } = DateTime.Now;

        public void Init() { }

        public Type ControllerType => typeof(UITemplateDailyQueueOfferDataController);
    }
}
#endif