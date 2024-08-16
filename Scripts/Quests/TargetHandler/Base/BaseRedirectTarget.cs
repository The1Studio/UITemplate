namespace TheOneStudio.UITemplate.Quests.TargetHandler
{
    using System;
    using Cysharp.Threading.Tasks;

    public abstract class BaseRedirectTarget : IRedirectTarget
    {
        public abstract Type GetTypeHandle { get; }

        protected abstract class BaseHandler<T> : IRedirectTarget.IHandler where T : IRedirectTarget
        {
            protected T RedirectTarget { get; private set; }

            public abstract UniTask Handle();

            IRedirectTarget IRedirectTarget.IHandler.RedirectTarget
            {
                set => this.RedirectTarget = (T)value;
            }
        }
    }
}