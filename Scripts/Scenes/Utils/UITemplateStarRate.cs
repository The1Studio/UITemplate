namespace UITemplate.Scripts.Scenes.Popups
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class UITemplateStarRate : MonoBehaviour
    {
        public GameObject Star1On;
        public GameObject Star2On;
        public GameObject Star3On;
        public GameObject Star1Off;
        public GameObject Star2Off;
        public GameObject Star3Off;
        private readonly Dictionary<int, List<bool>> starStates = new()
        {
            { 1, new List<bool> { true, false, false } },
            { 2, new List<bool> { true, true, false } },
            { 3, new List<bool> { true, true, true } },
            { default, new List<bool> { false, false, false } }
        };

        public void SetStarRate(int rate)
        {
            if (!this.starStates.TryGetValue(rate, out var starStates))
            {
                starStates = this.starStates[default];
            }

            this.SetOnOffStar(this.Star1On, this.Star1Off, starStates[0]);
            this.SetOnOffStar(this.Star2On, this.Star2Off, starStates[1]);
            this.SetOnOffStar(this.Star3On, this.Star3Off, starStates[2]);
        }

        private void SetOnOffStar(GameObject onObj, GameObject offObj, bool isActive)
        {
            onObj.SetActive(isActive);
            offObj.SetActive(!isActive);
        }
    }
}