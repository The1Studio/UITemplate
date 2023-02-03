namespace UITemplate.Scripts.Scenes.Popups
{
    using UnityEngine;
    
    public class UITemplateStarRate
    {
        public GameObject Star1On;
        public GameObject Star2On;
        public GameObject Star3On;
        public GameObject Star1Off;
        public GameObject Star2Off;
        public GameObject Star3Off;

        public void SetStarRate(int rate)
        {
            switch (rate)
            {
                case 1:
                    this.SetOnOffStar(this.Star1On, this.Star1Off, true);
                    this.SetOnOffStar(this.Star2On, this.Star2Off, false);
                    this.SetOnOffStar(this.Star3On, this.Star3Off, false);
                    break;
                case 2:
                    this.SetOnOffStar(this.Star1On, this.Star1Off, true);
                    this.SetOnOffStar(this.Star2On, this.Star2Off, true);
                    this.SetOnOffStar(this.Star3On, this.Star3Off, false);
                    break;
                case 3:
                    this.SetOnOffStar(this.Star1On, this.Star1Off, true);
                    this.SetOnOffStar(this.Star2On, this.Star2Off, true);
                    this.SetOnOffStar(this.Star3On, this.Star3Off, true);
                    break;
                default:
                    this.SetOnOffStar(this.Star1On, this.Star1Off, false);
                    this.SetOnOffStar(this.Star2On, this.Star2Off, false);
                    this.SetOnOffStar(this.Star3On, this.Star3Off, false);
                    break;
            }
        }

        private void SetOnOffStar(GameObject onObj, GameObject offObj, bool isActive)
        {
            onObj.SetActive(isActive);
            offObj.SetActive(!isActive);
        }
    }
}