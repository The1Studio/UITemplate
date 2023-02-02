namespace UITemplate.Scripts.Scenes.Popups
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UITemplateOnOffButton : Button
    {
        public GameObject OnObjects;
        public GameObject OffObjects;

        public void SetOnOff(bool isActive)
        {
            this.OnObjects.SetActive(isActive);
            this.OffObjects.SetActive(!isActive);
        }
    }
}