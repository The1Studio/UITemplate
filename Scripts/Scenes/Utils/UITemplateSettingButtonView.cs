namespace UITemplate.Scripts.Scenes.Popups
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class UITemplateSettingButtonView : MonoBehaviour
    {
        public Type   PopupType; 
        public Button Button { get; private set; }

        [SerializeField] private GameObject DropdowOption;
        [SerializeField] private bool       IsDropdown;

        private void Awake()
        {
            this.Button = this.GetComponent<Button>(); 
            
        }
    }
}