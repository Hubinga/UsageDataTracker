using SmartMeterApp.Models;
using System.Collections.ObjectModel;

namespace SmartMeterApp.Services
{
	/// <summary>
	/// This Service handles toast messages
	/// </summary>
    public class ToastService
    {
        public ObservableCollection<ToastObject> ToastObjects { get; set; } = new ObservableCollection<ToastObject>();
		private int secondsToRemoveToast = 5;

		/// <summary>
		/// Add a toast message to display: gets removed after 5 seconds
		/// </summary>
		/// <param name="message"></param>
		/// <param name="toastType"></param>
        public void AddToast(string message, ToastType toastType)
        {
			ToastObject toast = new ToastObject(message, toastType);
			ToastObjects.Add(toast);

            Console.WriteLine(message);

            // Start a timer to remove the toast after 5 seconds
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
