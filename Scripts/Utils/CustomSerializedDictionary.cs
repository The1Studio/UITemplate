namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using System;
    using System.Collections.Generic;
    using DG.Tweening;
    using TheOne.Extensions;
    using TMPro;
    using UnityEngine.UI;

    [Serializable]
    public class StringToTextDictionary : ServiceImplementation.Configs.CustomTypes.SerializableDictionary<string, TMP_Text>
    {
        private Dictionary<string, int> currencyIdToRewardValue = new();

        public void SetReward(Dictionary<string, int> finalValue, Func<int, string> contentFunc, float duration = 0.5f)
        {
            foreach (var (key, value) in finalValue)
            {
                var starValue = GetCurrentRewardValue(key);
                DOTween.To(() => starValue, x => this[key].text = contentFunc.Invoke(x), value, duration);
                this.currencyIdToRewardValue[key] = value;
            }

            return;

            int GetCurrentRewardValue(string rewardKey)
            {
                return this.currencyIdToRewardValue.GetOrAdd(rewardKey, () => 0);
            }
        }

        public void Reset()
        {
            this.currencyIdToRewardValue.Clear();
        }
    }

    [Serializable]
    public class StringToButtonDictionary : ServiceImplementation.Configs.CustomTypes.SerializableDictionary<string, Button>
    {
    }

    [Serializable]
    public class StringToIntDictionary : ServiceImplementation.Configs.CustomTypes.SerializableDictionary<string, int>
    {
    }
}