namespace SmartMeterApp.Models
{
    public class ToastObject
    {
        public ToastObject(string message, ToastType toastType)
        {
            Message = message;
            ToastType = toastType;
        }

        public string Message { get; set; }
        public ToastType ToastType  { get; set; }
    }
}
