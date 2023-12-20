namespace TheOneStudio.UITemplate.UITemplate.Scenes.BottomBarNavigator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public class BottomNavigatorBarView : MonoBehaviour
    {
        #region inject

        private IScreenManager screenManager;
        private SignalBus      signalBus;

        #endregion

        public Transform buttonParent;
        
        [NonSerialized]
        public  List<BottomBarNavigatorTabButtonView> Buttons;

        [Inject]
        public void Constructor(IScreenManager screenManager, SignalBus signalBus)
        {
            this.screenManager = screenManager;
            this.signalBus     = signalBus;
            
            this.Init();
        }

        private void Init()
        {
            this.signalBus.Subscribe<OnRemoveAdsSucceedSignal>(this.OnRemoveAdsHandler);
        }
        
        protected virtual void OnRemoveAdsHandler()
        {
        }

        private void Awake()
        {
            this.Buttons = this.buttonParent.GetComponentsInChildren<BottomBarNavigatorTabButtonView>().ToList();

            foreach (var bottomBarNavigatorTabButtonView in this.Buttons)
            {
                bottomBarNavigatorTabButtonView.Button.onClick.AddListener(() =>
                {
                    
                });
            }
        }
    }
}