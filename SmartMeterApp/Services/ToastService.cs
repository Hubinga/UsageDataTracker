using SmartMeterApp.Models;
using System.Collections.ObjectModel;

namespace SmartMeterApp.Services
{
    public class ToastService
    {
        public ObservableCollection<ToastObject> ToastObjects { get; set; } = new ObservableCollection<ToastObject>();

        public void AddToast()
        {
            ToastObjects.Add(new ToastObject(true, $"test{ToastObjects.Count}", ToastType.Info));
        }

        public void RemoveToast(ToastObject toast)
        {
            ToastObjects.Remove(toast);
        }

        public event Action<string, ToastType> OnShow;

        public void ShowToast(string message, ToastType toastType)
        {
            OnShow?.Invoke(message, toastType);
        }
    }
}
