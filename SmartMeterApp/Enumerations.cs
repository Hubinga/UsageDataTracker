using System.ComponentModel.DataAnnotations;

namespace SmartMeterApp
{
    public enum UserActions
    {
        Login,
        Register
    }

    public enum ToastType
    {
        [Display(Name = "Information")]
        Info,
        [Display(Name = "Warning")]
        Warning,
        [Display(Name = "Error")]
        Error
    }
}
