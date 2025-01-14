namespace TheOneStudio.UITemplate.UITemplate.Scripts.ThirdPartyServices.CollapsibleMREC
{
    using UnityEngine;
    using UnityEngine.UI;

    public class CollapsibleMrecView : MonoBehaviour
    {
        [field: SerializeField] public RectTransform BgTransform;
        [field: SerializeField] public Button        BtnClose;
        
        private void Awake()
        {
            this.BgTransform.sizeDelta = new Vector2(0, 250f * (Screen.dpi / 160f) + 200f); // Calculate size
            this.BgTransform.gameObject.SetActive(false);
        }
    }
}