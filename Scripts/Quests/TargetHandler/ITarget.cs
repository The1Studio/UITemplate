using System;

namespace TheOneStudio.UITemplate.Quests.TargetHandler
{
    public interface ITarget
    {
        public Type GetTypeHandle { get; set; }

        public interface IHandle
        {
            public void Operate();
        }
    }
}