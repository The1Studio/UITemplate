namespace TheOneStudio.UITemplate.UITemplate.Scenes.Utils
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class UITemplateOnOffButton : MonoBehaviour
    {
        public Button Button
        {
            get => this.GetComponent<Button>();
            private set { }
        }

        [SerializeField] private GameObject onObjects;
        [SerializeField] private GameObject offObjects;

        public bool isOn;

        public void SetOnOff(bool isOn)
        {
            this.isOn = isOn;
            this.onObjects.SetActive(isOn);
            this.offObjects.SetActive(!isOn);
        }
    }
}