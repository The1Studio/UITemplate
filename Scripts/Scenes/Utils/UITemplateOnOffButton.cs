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

        private void Awake() { this.Button = this.GetComponent<Button>(); }
        

        public void SetOnOff(bool isActive)
        {
            this.onObjects.SetActive(isActive);
            this.offObjects.SetActive(!isActive);
        }
    }
}