namespace TheOneStudio.UITemplate.Quests.UI
{
    using System;
    using GameFoundation.Scripts.Utilities.Extension;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class UITemplateQuestPopupTabButton : MonoBehaviour
    {
        [field: SerializeField] public string Tab { get; private set; }

        [SerializeField] private GameObject[] activeObjects;
        [SerializeField] private GameObject[] inactiveObjects;

        private void Awake()
        {
            this.GetComponent<Button>().onClick.AddListener(this.SetActive);
        }

        private Action onClick;

        public void SetOnClick(Action<string> action, UITemplateQuestPopupTabButton[] tabButtons)
        {
            this.onClick = () =>
            {
                action(this.Tab);
                tabButtons.ForEach(tabButton =>
                {
                    tabButton.activeObjects.ForEach(obj => obj.SetActive(tabButton == this));
                    tabButton.inactiveObjects.ForEach(obj => obj.SetActive(tabButton != this));
                });
            };
        }

        public void SetActive() => this.onClick();
    }
}