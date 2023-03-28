namespace TheOneStudio.UITemplate.UITemplate.Models
{
    using System;
    using GameFoundation.Scripts.Interfaces;

    public class UITemplateUserJackpotData: ILocalData
    {
        public int CurrentJackpotSpin   { get; set; } = 0;
        public int RemainingJackpotSpin { get; set; } = 100;
        
        public DateTime JackpotDate { get; set; } = DateTime.Now;
        
        public void Init()
        {
            
        }
    }
}