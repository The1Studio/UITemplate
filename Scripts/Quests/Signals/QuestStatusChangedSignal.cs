namespace TheOneStudio.UITemplate.Quests.Signals
{
    public sealed class QuestStatusChangedSignal
    {
        public UITemplateQuestController QuestController { get; }

        public QuestStatusChangedSignal(UITemplateQuestController questController)
        {
            this.QuestController = questController;
        }
    }
}