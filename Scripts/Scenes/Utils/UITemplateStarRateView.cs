namespace UITemplate.Scripts.Scenes.Popups
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class UITemplateStarRateView : MonoBehaviour
    {
        [SerializeField] private List<GameObject> StarOnList;
        [SerializeField] private List<GameObject> StarOffList;

        public void SetStarRate(int rate)
        {
            for (int i = 0; i < this.StarOnList.Count; i++)
            {
                bool isActive = i < rate;
                this.SetOnOffStar(this.StarOnList[i], this.StarOffList[i], isActive);
            }
        }

        private void SetOnOffStar(GameObject onObj, GameObject offObj, bool isActive)
        {
            onObj.SetActive(isActive);
            offObj.SetActive(!isActive);
        }
    }
}