namespace TheOneStudio.UITemplate.UITemplate.Models.Controllers
{
    public class UITemplateUserDataController
    {
        private readonly UITemplateUserData userData;

        public UITemplateUserDataController(UITemplateUserData userData) { this.userData = userData; }

        public void SetIsFirstOpenGame(bool isFirstOpenGame) { this.userData.IsFirstOpenGame = isFirstOpenGame; }

        public bool IsFirstOpenGame => this.userData.IsFirstOpenGame;
    }
}