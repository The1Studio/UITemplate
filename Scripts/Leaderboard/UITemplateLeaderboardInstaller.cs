namespace TheOneStudio.UITemplate.UITemplate.Leaderboard
{
    using TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals;
    using Zenject;

    public class UITemplateLeaderboardInstaller : Installer<UITemplateLeaderboardInstaller>
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