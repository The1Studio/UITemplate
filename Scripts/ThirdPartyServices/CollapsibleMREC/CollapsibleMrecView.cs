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
            this.BgTransform.gameObject.SetActive(false);
        }
    }
}