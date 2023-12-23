namespace TheOneStudio.UITemplate.Quests.UI
{
    using GameFoundation.Scripts.Utilities.Extension;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class UITemplateQuestPopupTabButton : MonoBehaviour
    {
        [field: SerializeField] public string Tab { get; private set; }

        [SerializeField] private GameObject[] activeObjects;
        [SerializeField] private GameObject[] inactiveObjects;

        public UITemplateQuestPopupPresenter Parent { get; set; }

        private void Awake()
        {
            this.GetComponent<Button>().onClick.AddListener(this.OnClick);
        }

        private void OnClick()
        {
            this.Parent.Tab = this.Tab;
        }

        public void SetActive(bool active)
        {
            this.activeObjects.ForEach(obj => obj.SetActive(active));
            this.inactiveObjects.ForEach(obj => obj.SetActive(!active));
        }
    }
}