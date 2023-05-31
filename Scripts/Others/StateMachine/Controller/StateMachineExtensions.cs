namespace TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Controller
{
    using System;
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;
    using TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Interface;
    using UnityEngine.Assertions;
    using Zenject;

    public static class StateMachineExtensions
    {
        public static void InstallTargetStateMachine<TTarget, TTargetState, TTargetStateMachine>(this DiContainer container)
        where TTarget : ITarget
        where TTargetState : ITargetState<ITarget>
        where TTargetStateMachine : TargetStateMachine<TTarget>
        {
            container.Bind<IState>()
                     .To(convention => convention.AllNonAbstractClasses().DerivingFrom<TTargetState>())
                     .AsTransient()
                     .WhenInjectedInto<TTargetStateMachine>()
                     .Lazy();
        }

        public static void InstallNonTargetStateMachine<TState, TStateMachine>(this DiContainer container)
        where TState : IState
        where TStateMachine : StateMachine
        {
            Assert.IsFalse(typeof(ITargetState<>).IsAssignableFrom(typeof(TState)), $"State {typeof(TState)} must be non-target");
            Assert.IsFalse(typeof(TargetStateMachine<>).IsAssignableFrom(typeof(IStateMachine)), $"State machine {typeof(TStateMachine)} must be non-target");

            container.Bind<IState>()
                     .To(convention => convention.AllNonAbstractClasses().DerivingFrom<TState>())
                     .AsSingle()
                     .WhenInjectedInto<TStateMachine>()
                     .NonLazy();

            container.Bind<TStateMachine>().AsSingle().NonLazy();
        }

        public static void UseStateMachine<TTarget, TTargetStateMachine>(this TTarget target, DiContainer container, Type initState)
        where TTarget : ITarget
        where TTargetStateMachine : TargetStateMachine<TTarget>
        {
            Assert.IsTrue(typeof(ITargetState<TTarget>).IsAssignableFrom(initState), $"Param {initState} must be target state");

            var stateMachine = container.Instantiate<TTargetStateMachine>(new object[] { target });
            stateMachine.TransitionTo(initState);
        }

        public static void UseStateMachine<TTarget, TTargetStateMachine, TInitState>(this TTarget target, DiContainer container)
        where TTarget : ITarget
        where TTargetStateMachine : TargetStateMachine<TTarget>
        where TInitState : ITargetState<TTarget>
        {
            target.UseStateMachine<TTarget, TTargetStateMachine>(container, typeof(TInitState));
        }

        public static void DisposeStateMachine<TTarget>(this TTarget target)
        where TTarget : ITarget
        {
            if (target.StateMachine is null)
            {
                return;
            }

            target.StateMachine.CurrentState?.Exit();
            target.StateMachine = null;
        }
    }
}