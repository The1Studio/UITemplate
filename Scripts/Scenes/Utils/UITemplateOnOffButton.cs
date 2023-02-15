namespace UITemplate.Scripts.Scenes.Popups
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class UITemplateOnOffButton : MonoBehaviour
    {
        public Button Button { get; private set; }
        
        [SerializeField] private GameObject onObjects;
        [SerializeField] private GameObject offObjects;

        private void Awake()
        {
            this.Button = this.GetComponent<Button>();
            this.offObjects.GetComponentInChildren<Button>().onClick.AddListener(() => this.Button.onClick.Invoke());
            this.onObjects.GetComponentInChildren<Button>().onClick.AddListener(() => this.Button.onClick.Invoke());
        }

        public void SetOnOff(bool isOn)
        {
            this.onObjects.SetActive(isOn);
            this.offObjects.SetActive(!isOn);
        }
    }
}