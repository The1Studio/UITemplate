namespace TheOneStudio.UITemplate.Quests.TargetHandler
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using GameFoundation.Scripts.Utilities.Extension;
    using Newtonsoft.Json;
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;
    using TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Interface;

    public class ChangeStateRedirectTarget : BaseRedirectTarget
    {
        [JsonProperty] private string StateName     { get; set; }
        public override        Type   GetTypeHandle { get; } = typeof(Handler);

        sealed class Handler : BaseHandler<ChangeStateRedirectTarget>
        {
            private readonly IStateMachine stateMachine;

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