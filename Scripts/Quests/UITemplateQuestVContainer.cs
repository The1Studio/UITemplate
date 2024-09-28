#if GDK_VCONTAINER
#nullable enable
namespace TheOneStudio.UITemplate.Quests
{
    using GameFoundation.DI;
    using GameFoundation.Signals;
    using TheOneStudio.UITemplate.Quests.Signals;
    using TheOneStudio.UITemplate.UITemplate.Quests.Signals;
    using VContainer;

    public static class UITemplateQuestVContainer
    {
        public static void RegisterQuestManager(this IContainerBuilder builder)
        {
            #if THEONE_QUEST_SYSTEM
            builder.Register<UITemplateQuestManager>(Lifetime.Singleton).AsInterfacesAndSelf();

            builder.DeclareSignal<QuestStatusChangedSignal>();
            builder.DeclareSignal<ClaimAllQuestSignal>();
            #endif
        }
    }
}
#endif