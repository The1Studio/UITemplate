namespace TheOneStudio.UITemplate.Quests.Signals
{
    public sealed class QuestStatusChangedSignal
    {
        public UITemplateQuestController QuestController { get; }

        internal QuestStatusChangedSignal(UITemplateQuestController questController)
        {
            this.QuestController = questController;
        }
    }
}