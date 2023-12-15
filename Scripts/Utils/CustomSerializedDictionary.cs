namespace TheOneStudio.UITemplate.UITemplate.Utils
{
    using System;
    using System.Collections.Generic;
    using DG.Tweening;
    using GameFoundation.Scripts.Utilities.Extension;
    using ServiceImplementation.Configs.CustomTypes;
    using TMPro;
    using UnityEngine.UI;

    [Serializable]
    public class StringToTextDictionary : SerializableDictionary<string, TMP_Text>
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
    public class StringToButtonDictionary : SerializableDictionary<string, Button>
    {
    }
    
    [Serializable]
    public class StringToIntDictionary : SerializableDictionary<string, int>
    {
    }
}