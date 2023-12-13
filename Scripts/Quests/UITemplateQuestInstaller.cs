namespace TheOneStudio.UITemplate.Quests
{
    using UnityEngine;
    using Zenject;

    public class UITemplateQuestInstaller : Installer<UITemplateQuestInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<UITemplateQuestManager>().AsSingle();

            if (Object.FindObjectOfType<UITemplateQuestNotificationService>() is { } notificationService)
            {
                this.Container.BindInterfacesAndSelfTo<UITemplateQuestNotificationService>()
                    .FromInstance(notificationService)
                    .AsSingle()
                    .NonLazy();
            }
        }
    }
}