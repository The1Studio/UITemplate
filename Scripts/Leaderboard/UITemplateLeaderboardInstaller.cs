namespace TheOneStudio.UITemplate.UITemplate.Leaderboard
{
    using TheOneStudio.UITemplate.UITemplate.Leaderboard.Signals;
    using Zenject;

    public class UITemplateLeaderboardInstaller : Installer<UITemplateLeaderboardInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.DeclareSignal<NewAllTimeHighestScoreSignal>();
            this.Container.DeclareSignal<NewDayHighestScoreSignal>();
            this.Container.DeclareSignal<NewMonthHighestScoreSignal>();
            this.Container.DeclareSignal<NewYearHighestScoreSignal>();
            this.Container.DeclareSignal<NewWeekHighestScoreSignal>();
        }
    }
}