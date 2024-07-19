namespace SmartMeterApp.Utility
{
    public interface IEventAggregator
    {
        event Action<int> ActionRequired;

        void TriggerAction(int value);
    }

    /// <summary>
    /// This services is used to notify the Chart to update if user edited a usage data value
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        public event Action<int> ActionRequired;

        public void TriggerAction(int value)
        {
            ActionRequired?.Invoke(value);
        }
    }
}
