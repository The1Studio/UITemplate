namespace TheOneStudio.UITemplate.UITemplate.Scenes.BottomBarNavigator
{
    using System.Collections.Generic;
    using System.Linq;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public class UITemplateBottomNavigatorBarView : MonoBehaviour
    {
        #region inject

        private SignalBus      signalBus;

        #endregion

        public Transform buttonParent;

        private List<BottomBarNavigatorTabButtonView> Buttons;
        private int                                   CurrentActiveIndex { get; set; } = -1;

        [Inject]
        public void Constructor(SignalBus signalBus)
        {
            this.signalBus     = signalBus;

            this.Init();
        }

        private void Init() { this.signalBus.Subscribe<OnRemoveAdsSucceedSignal>(this.OnRemoveAdsHandler); }

        protected virtual void OnRemoveAdsHandler() { }

        private void Awake()
        {
            this.Buttons = this.buttonParent.GetComponentsInChildren<BottomBarNavigatorTabButtonView>().ToList();

            for (var index = 0; index < this.Buttons.Count; index++)
            {
                var bottomBarNavigatorTabButtonView = this.Buttons[index];
                bottomBarNavigatorTabButtonView.Init();
                var index1 = index;
                bottomBarNavigatorTabButtonView.Button.onClick.AddListener(() => this.OnClickBottomBarButton(index1));
            }
            
            this.OnClickBottomBarButton(this.DefaultActiveIndex);
        }

        private void OnClickBottomBarButton(int index)
        {
            Debug.Log($"On Click: {index}");
            if (this.CurrentActiveIndex == index) return;
            this.CurrentActiveIndex = index;
            
            this.OnCLickButton(index);
            var bottomBarNavigatorTabButtonView = this.Buttons[index];
            bottomBarNavigatorTabButtonView.SetActive(true);
            foreach (var otherBottomBarNavigatorTabButtonView in this.Buttons)
            {
                if (otherBottomBarNavigatorTabButtonView == bottomBarNavigatorTabButtonView) continue;

                otherBottomBarNavigatorTabButtonView.SetActive(false);
            }
        }

        protected virtual void OnCLickButton(int index) { }

        protected virtual int DefaultActiveIndex => 0;
    }
}