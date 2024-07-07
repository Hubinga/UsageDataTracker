namespace SmartMeterApp.Utility
{
    public interface IEventAggregator
    {
        event Action<int> ActionRequired;

        void TriggerAction(int value);
    }

    public class EventAggregator : IEventAggregator
    {
        public event Action<int> ActionRequired;

        public void TriggerAction(int value)
        {
            ActionRequired?.Invoke(value);
        }
    }
}
