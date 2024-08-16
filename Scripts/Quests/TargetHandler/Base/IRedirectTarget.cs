namespace TheOneStudio.UITemplate.Quests.TargetHandler
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IRedirectTarget
    {
        public Type GetTypeHandle { get; }

        public interface IHandler
        {
            IRedirectTarget RedirectTarget { set; }
            UniTask         Handle();
        }
    }
}