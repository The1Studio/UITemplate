namespace TheOneStudio.UITemplate.UITemplate.Services
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MrecHandler
    {
        private readonly List<string> screenShowMrec = new(); 
        
        public void RegisterScreenCanShowMrec(string presenter)
        {
            Debug.LogError(presenter);
            if (this.screenShowMrec.Contains(presenter)) return;
            this.screenShowMrec.Add(presenter);
        }

        public bool CanShowMrec(string screenName) => this.screenShowMrec.Contains(screenName);
    }
}