namespace TheOneStudio.UITemplate.UITemplate.FTUE.Conditions
{
    using Newtonsoft.Json;

    public interface IFtueCondition
    {
        string Id { get; }
        bool   IsPassedCondition(string data);
    }
    
    public abstract class FtueCondition<T> : IFtueCondition
    {
        public abstract string Id { get; }

        public bool IsPassedCondition(string data)
        {
            return this.IsPassedCondition(JsonConvert.DeserializeObject<T>(data));
        }
        
        protected abstract bool IsPassedCondition(T data);
    }
}