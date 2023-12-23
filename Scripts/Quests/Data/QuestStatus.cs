namespace TheOneStudio.UITemplate.Quests.Data
{
    using System;

    [Flags]
    public enum QuestStatus
    {
        Started   = 1 << 0,
        Shown     = 1 << 1,
        Completed = 1 << 2,
        Collected = 1 << 3,

        NotCompleted = Started | Shown,
        NotCollected = Started | Shown | Completed,
        All          = Started | Shown | Completed | Collected,
    }
}