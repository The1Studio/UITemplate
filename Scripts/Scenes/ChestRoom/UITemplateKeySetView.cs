namespace TheOneStudio.UITemplate.UITemplate.Scenes.ChestRoom
{
    using System;
    using System.Collections.Generic;
    using TheOneStudio.UITemplate.UITemplate.Models.Controllers;
    using TheOneStudio.UITemplate.UITemplate.Signals;
    using UnityEngine;
    using Zenject;

    public class UITemplateKeySetView : MonoBehaviour, IDisposable
    {
        private SignalBus signalBus;
        private string currencyKey;

        public List<GameObject> KeySet;

        public void BindData(SignalBus signalBus, int initValue, string currencyKey = UITemplateInventoryDataController.DefaultChestRoomKeyCurrencyID)
        {
            this.signalBus   = signalBus;
            this.currencyKey = currencyKey;
            this.signalBus.Subscribe<UpdateCurrencySignal>(this.OnCurrencyUpdated);
            this.SetKeyAmount(initValue);
        }

        private void OnCurrencyUpdated(UpdateCurrencySignal obj)
        {
            if (!obj.Id.Equals(this.currencyKey)) return;

            this.SetKeyAmount(obj.FinalValue);
        }

        public void Dispose()
        {
            this.signalBus.Unsubscribe<UpdateCurrencySignal>(this.OnCurrencyUpdated);
        }

        private void SetKeyAmount(int keyAmount)
        {
            foreach (var gameObject in this.KeySet)
            {
                gameObject.SetActive(false);
            }

            for (var i = 0; i < Math.Min(keyAmount, this.KeySet.Count); i++)
            {
                this.KeySet[i].SetActive(true);
            }
        }
    }
}