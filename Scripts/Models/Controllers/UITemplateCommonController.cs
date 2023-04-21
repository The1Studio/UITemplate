namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    using TheOneStudio.UITemplate.UITemplate.Models.LocalDatas;

    public class UITemplateCommonController:IUITemplateControllerData
    {
        private readonly UITemplateCommonData uiTemplateCommonData;

        public UITemplateCommonController(UITemplateCommonData uiTemplateCommonData) { this.uiTemplateCommonData = uiTemplateCommonData; }
        public bool IsFirstTimeOpenGame => this.uiTemplateCommonData.IsFirstTimeOpenGame;

        public void ChangeGameIsAlreadyOpened() { this.uiTemplateCommonData.IsFirstTimeOpenGame = false; }
    }
}