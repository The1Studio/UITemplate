namespace TheOneStudio.UITemplate.Quests.Signals
{
    public class QuestStatusChangedSignal
    {
        public UITemplateQuestController QuestController { get; }

        public QuestStatusChangedSignal(UITemplateQuestController questController)
        {
            this.QuestController = questController;
        }
    }
}