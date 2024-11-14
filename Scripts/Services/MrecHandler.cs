namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System.Collections.Generic;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using UnityEngine;

    public class MrecHandler
    {
        private readonly List<string> screenShowMrec = new(); 
        
        public void RegisterScreenCanShowMrec<T>() where T : IScreenPresenter
        {
            var screenName = typeof(T).Name;
            if (this.screenShowMrec.Contains(screenName)) return;
            this.screenShowMrec.Add(screenName);
        }

        public bool CanShowMrec(string screenName) => this.screenShowMrec.Contains(screenName);
    }
}