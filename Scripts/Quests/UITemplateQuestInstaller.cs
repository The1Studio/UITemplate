#if GDK_ZENJECT
namespace TheOneStudio.UITemplate.Quests
{
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.Quests.Signals;
    using TheOneStudio.UITemplate.UITemplate.Quests.Signals;
    using UnityEngine;
    using Zenject;

    public class UITemplateQuestInstaller : Installer<UITemplateQuestInstaller>
    {
        public override void InstallBindings()
        {
#if THEONE_QUEST_SYSTEM
            this.Container.BindInterfacesAndSelfTo<UITemplateQuestManager>().AsSingle();

            if (Object.FindObjectOfType<UITemplateQuestNotificationView>() is { } notificationView)
            {
                this.Container.BindInterfacesAndSelfTo<UITemplateQuestNotificationView>()
                    .FromInstance(notificationView)
                    .AsSingle();
            }

            this.Container.DeclareSignal<QuestStatusChangedSignal>();
            this.Container.DeclareSignal<ClaimAllQuestSignal>();
#endif
        }
    }
}
#endif