namespace TheOneStudio.UITemplate.UITemplate.FTUE.Conditions
{
    using Newtonsoft.Json;

    public interface IFtueCondition
    {
        string Id { get; }
        bool   IsPassedCondition(string     data);
        bool   CanTrigger(string            data);
        string GetTooltipText(string        data);
        string GetShortConditionText(string data);
    }

    public abstract class FtueCondition<T> : IFtueCondition
    {
        public abstract string Id { get; }

        public bool IsPassedCondition(string data) => this.IsPassedCondition(JsonConvert.DeserializeObject<T>(data));

        public bool CanTrigger(string data) => this.CanTrigger(JsonConvert.DeserializeObject<T>(data));

        public string GetTooltipText(string data) => this.GetTooltipText(JsonConvert.DeserializeObject<T>(data));

        public string GetShortConditionText(string data) => this.GetShortConditionText(JsonConvert.DeserializeObject<T>(data));

        protected abstract string GetTooltipText(T data);

        protected abstract bool IsPassedCondition(T data);

        protected virtual bool CanTrigger(T data) => this.IsPassedCondition(data);

        protected abstract string GetShortConditionText(T data);
    }
}