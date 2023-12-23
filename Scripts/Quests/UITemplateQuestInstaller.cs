namespace TheOneStudio.UITemplate.Quests
{
    using TheOneStudio.UITemplate.Quests.Rewards;
    using TheOneStudio.UITemplate.Quests.Signals;
    using UnityEngine;
    using Zenject;

    public class UITemplateQuestInstaller : Installer<UITemplateQuestInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IReward.IHandler>()
                .To(convention => convention.AllNonAbstractClasses().DerivingFrom<IReward.IHandler>())
                .AsSingle()
                .WhenInjectedInto<UITemplateQuestController>();

            this.Container.BindInterfacesAndSelfTo<UITemplateQuestManager>().AsSingle();

            if (Object.FindObjectOfType<UITemplateQuestNotificationService>() is { } notificationService)
            {
                this.Container.BindInterfacesAndSelfTo<UITemplateQuestNotificationService>()
                    .FromInstance(notificationService)
                    .AsSingle();
            }

            this.Container.DeclareSignal<QuestStatusChangedSignal>();
        }
    }
}