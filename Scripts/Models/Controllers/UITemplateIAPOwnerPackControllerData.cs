namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;
    using UnityEngine.Scripting;

    public class UITemplateIAPOwnerPackControllerData : IUITemplateControllerData
    {
        private readonly UITemplateIAPOwnerPackData uiTemplateIAPOwnerPackData;

        [Preserve]
        public UITemplateIAPOwnerPackControllerData(UITemplateIAPOwnerPackData uiTemplateIAPOwnerPackData) { this.uiTemplateIAPOwnerPackData = uiTemplateIAPOwnerPackData; }

        public void AddPack(string packId)
        {
            if (this.uiTemplateIAPOwnerPackData.OwnedPacks.Contains(packId)) return;
            this.uiTemplateIAPOwnerPackData.OwnedPacks.Add(packId);
        }

        public bool IsOwnerPack(string packId) => this.uiTemplateIAPOwnerPackData.OwnedPacks.Contains(packId);
    }
}