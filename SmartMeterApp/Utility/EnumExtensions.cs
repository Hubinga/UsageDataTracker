using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SmartMeterApp.Utility
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? value.ToString();
        }
    }
}
