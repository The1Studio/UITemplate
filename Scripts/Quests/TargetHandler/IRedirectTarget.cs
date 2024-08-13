using System;

namespace TheOneStudio.UITemplate.Quests.TargetHandler
{
    public interface IRedirectTarget
    {
        public Type GetTypeHandle { get; set; }

        public interface IHandle
        {
            public void Operate();
        }
    }
}