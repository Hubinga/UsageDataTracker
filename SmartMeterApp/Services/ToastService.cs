using SmartMeterApp.Models;
using System.Collections.ObjectModel;

namespace SmartMeterApp.Services
{
    public class ToastService
    {
        public ObservableCollection<ToastObject> ToastObjects { get; set; } = new ObservableCollection<ToastObject>();
		private int secondsToRemoveToast = 5;


        public void AddToast(string message, ToastType toastType)
        {
			ToastObject toast = new ToastObject(message, toastType);
			ToastObjects.Add(toast);

			// Start a timer to remove the toast after 10 seconds
			Timer timer = new Timer(RemoveToast, toast, secondsToRemoveToast * 1000, Timeout.Infinite);
		}

		public void RemoveToast(object state)
		{
			var toast = state as ToastObject;
			if (toast != null)
			{
				ToastObjects.Remove(toast);
			}
		}
    }
}
