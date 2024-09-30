namespace TheOneStudio.UITemplate.Quests.TargetHandler
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using UnityEngine.Scripting;

    [Preserve]
    public class OpenScreenRedirectTarget : BaseRedirectTarget
    {
        [JsonProperty] private string PresenterName { get; [Preserve] set; }

        public override Type GetTypeHandle => typeof(Handler);

        private sealed class Handler : BaseHandler<OpenScreenRedirectTarget>
        {
            private readonly IScreenManager screenManager;
            private readonly MethodInfo     openScreenMethod;

            [Preserve]
            public Handler(IScreenManager screenManager)
            {
                this.screenManager = screenManager;
                this.openScreenMethod = this.screenManager.GetType()
                    .GetMethods()
                    .Single(method => method.Name == nameof(this.screenManager.OpenScreen) && method.IsGenericMethod && method.GetGenericArguments().Length is 1);
            }

            public override async UniTask Handle()
            {
                var screen = ReflectionUtils.GetAllDerivedTypes<IScreenPresenter>().Single(screen => screen.Name == this.RedirectTarget.PresenterName);
                var task = this.openScreenMethod.MakeGenericMethod(screen)
                    .Invoke(this.screenManager, null);
                await (UniTask)task.GetType().GetMethod("AsUniTask")!.Invoke(task, null);
            }
        }
    }
}