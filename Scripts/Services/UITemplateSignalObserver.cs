namespace TheOneStudio.UITemplate.UITemplate.Services
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Zenject;

    public class UITemplateSignalObserver
    {
        private Dictionary<Type, bool>                         SignalFiringInfos { get; } = new ();
        private Dictionary<ObserveSignalInfo, HashSet<Action>> PendingActions    { get; } = new ();

        public UITemplateSignalObserver(SignalBus signalBus)
        {
            var signals = GetDeclaredSignalTypes(signalBus);

            foreach (var signal in signals)
            {
                this.SignalFiringInfos[signal] = false;

                void OnSignalFired(object _)
                {
                    this.SignalFiringInfos[signal] = true;
                    signalBus.Unsubscribe(signal, OnSignalFired);
                    this.ResolvePendingAction(signal);
                }

                signalBus.Subscribe(signal, OnSignalFired);
            }
        }

        private static IEnumerable<Type> GetDeclaredSignalTypes(SignalBus signalBus)
        {
            var signalBusType = signalBus.GetType();

            var signalBusField = signalBusType.GetField("_localDeclarationMap", BindingFlags.Instance | BindingFlags.NonPublic);

            if (signalBusField is null)
                throw new Exception("Cannot get _localDeclarationMap field from SignalBus.");

            var localDeclarationMap = (Dictionary<BindingId, SignalDeclaration>)signalBusField.GetValue(signalBus);

            if (localDeclarationMap is null)
                throw new Exception("Cannot get _localDeclarationMap field from SignalBus.");

            foreach (var key in localDeclarationMap.Keys)
            {
                yield return key.Type;
            }
        }

        private void ResolvePendingAction(Type signalType)
        {
            // don't remove ToArray() because it will cause Remove() throw exception
            foreach (var (observeSignalInfo, actions) in this.PendingActions.ToArray())
            {
                // check pending action contains the signal type and all signals are fired
                if (!observeSignalInfo.Signals.Contains(signalType) || !observeSignalInfo.Signals.All(e => this.SignalFiringInfos[e])) continue;

                foreach (var action in actions)
                {
                    action?.Invoke();
                }

                this.PendingActions.Remove(observeSignalInfo);
            }
        }

        public void CallOnSignalFired(Action action, params Type[] signals)
        {
            var observeSignalInfo = new ObserveSignalInfo(signals);

            if (observeSignalInfo.Signals.All(e => this.SignalFiringInfos[e]))
            {
                action?.Invoke();
                return;
            }

            if (!this.PendingActions.TryGetValue(observeSignalInfo, out var actions))
            {
                actions                                = new HashSet<Action>();
                this.PendingActions[observeSignalInfo] = actions;
            }

            actions.Add(action);
        }

        private class ObserveSignalInfo
        {
            public HashSet<Type> Signals { get; }

            public ObserveSignalInfo(IEnumerable<Type> signals)
            {
                this.Signals = new HashSet<Type>(signals);
            }

            public override int GetHashCode()
            {
                return this.Signals.Aggregate(0, HashCode.Combine);
            }

            public override bool Equals(object obj)
            {
                return obj is ObserveSignalInfo other && this.Signals.SetEquals(other.Signals);
            }
        }
    }

}