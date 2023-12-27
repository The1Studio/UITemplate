namespace TheOneStudio.UITemplate.HighScore
{
    using TheOneStudio.UITemplate.HighScore.Signals;
    using Zenject;

    public class UITemplateHighScoreInstaller : Installer<UITemplateHighScoreInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.DeclareSignal<NewHighScoreSignal>();
        }
    }
}