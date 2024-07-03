namespace SmartMeterApp.Models
{
    public class ToastObject
    {
        public ToastObject(bool visible, string message, ToastType toastType)
        {
            Visible = visible;
            Message = message;
            ToastType = toastType;
        }

        public bool Visible { get; set; }
        public string Message { get; set; }
        public ToastType ToastType  { get; set; }

        public void SetProperties(bool visible, string message, ToastType toastType)
        {
            Visible = visible;
            Message = message;
            ToastType = toastType;
        }
    }
}
