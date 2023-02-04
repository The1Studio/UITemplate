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
                this.StarOnList[i].SetActive(isActive);
                this.StarOffList[i].SetActive(!isActive);
            }
        }
    }
}