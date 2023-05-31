namespace TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Controller
{

    using System;
    using GameFoundation.Scripts.Utilities.Extension;
    using TheOneStudio.HyperCasual.Others.StateMachine.Interface;
    using TheOneStudio.UITemplate.UITemplate.Others.StateMachine.Interface;
    using UnityEngine.Assertions;
    using Zenject;

    public static class StateMachineExtensions
    {
        public static void InstallTargetStateMachine<TTarget, TTargetState, TTargetStateMachine>(this DiContainer container)
        where TTarget : ITarget
        where TTargetState : ITargetState<TTarget>
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
            Assert.IsFalse(typeof(TState).IsSubclassOfRawGeneric(typeof(ITargetState<>)), $"State {typeof(TState)} must be non-target");
            Assert.IsFalse(typeof(TStateMachine).IsSubclassOfRawGeneric(typeof(TargetStateMachine<>)), $"State machine {typeof(TStateMachine)} must be non-target");

            container.Bind<IState>()
                     .To(convention => convention.AllNonAbstractClasses().DerivingFrom<TState>())
                     .AsSingle()
                     .WhenInjectedInto<TStateMachine>()
                     .NonLazy();

            container.BindInterfacesAndSelfTo<TStateMachine>().AsSingle().NonLazy();
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

        public static void TransitionTo<TState>(this IStateMachine stateMachine)
        where TState : IState
        {
            var isTargetStateMachine = stateMachine.GetType().IsSubclassOfRawGeneric(typeof(TargetStateMachine<>));
            var isTargetState        = typeof(TState).IsSubclassOfRawGeneric(typeof(ITargetState<>));

            Assert.IsTrue(isTargetStateMachine == isTargetState, $"State machine {stateMachine.GetType()} and state {typeof(TState)} must be both target or non-target");

            stateMachine.TransitionTo(typeof(TState));
        }
    }

}