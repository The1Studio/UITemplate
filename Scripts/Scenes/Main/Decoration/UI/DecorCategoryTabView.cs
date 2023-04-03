namespace TheOneStudio.UITemplate.UITemplate.Scenes.Main.Decoration.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class DecorCategoryTabView : MonoBehaviour
    {
        [SerializeField] private Button activeButton, deActiveButton;

        public Action OnButtonClick { get; set; }

        private RectTransform rectTransform;

        private void Awake()
        {
            this.activeButton.onClick.AddListener(() => this.OnButtonClick?.Invoke());
            this.deActiveButton.onClick.AddListener(() => this.OnButtonClick?.Invoke());
        }

        public void SetActive(bool isActive)
        {
            this.activeButton.gameObject.SetActive(isActive);
            this.deActiveButton.gameObject.SetActive(!isActive);
        }

        public void SetPosition(Vector2 position)
        {
            if (this.rectTransform == null) this.rectTransform = this.GetComponent<RectTransform>();
            this.rectTransform.anchoredPosition = position;
        }
    }
}