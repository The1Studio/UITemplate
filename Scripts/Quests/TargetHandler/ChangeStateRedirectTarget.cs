namespace TheOneStudio.UITemplate.Quests.TargetHandler
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;
    using TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Interface;
    using UnityEngine.Scripting;

    [Preserve]
    public class ChangeStateRedirectTarget : BaseRedirectTarget
    {
        [JsonProperty] private string StateName { get; [Preserve] set; }

        public override Type GetTypeHandle { get; } = typeof(Handler);

        private sealed class Handler : BaseHandler<ChangeStateRedirectTarget>
        {
            private readonly IStateMachine stateMachine;

            [Preserve]
            public Handler(IStateMachine stateMachine)
            {
                this.stateMachine = stateMachine;
            }

            public override UniTask Handle()
            {
                var type = ReflectionUtils.GetAllDerivedTypes<IState>().Single(type1 => type1.Name == this.RedirectTarget.StateName);
                this.stateMachine.TransitionTo(type);
                return UniTask.CompletedTask;
            }
        }
    }
}