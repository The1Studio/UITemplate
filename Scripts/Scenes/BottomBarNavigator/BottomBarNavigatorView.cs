namespace TheOneStudio.UITemplate.UITemplate.Scenes.BottomBarNavigator
{
    using System.Collections.Generic;
    using UnityEngine;
    using Zenject;

    public class BottomBarNavigatorView : MonoBehaviour
    {
        public List<BottomBarNavigatorTabButtonView> Buttons;

        [Inject]
        public void Constructor()
        {
        }
    }
}