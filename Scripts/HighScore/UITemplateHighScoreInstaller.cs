namespace TheOneStudio.UITemplate.HighScore
{
    using TheOneStudio.UITemplate.HighScore.Signals;
    using Zenject;

    public class UITemplateHighScoreInstaller : Installer<UITemplateHighScoreInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.DeclareSignal<NewAllTimeHighScoreSignal>();
            this.Container.DeclareSignal<NewDailyHighScoreSignal>();
            this.Container.DeclareSignal<NewMonthlyHighScoreSignal>();
            this.Container.DeclareSignal<NewYearlyHighScoreSignal>();
            this.Container.DeclareSignal<NewWeeklyHighScoreSignal>();
        }
    }
}