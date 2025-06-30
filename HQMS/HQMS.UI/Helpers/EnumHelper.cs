using System.ComponentModel;
using System.Reflection;

namespace HQMS.UI.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription<TEnum>(TEnum value) where TEnum : System.Enum
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return value.ToString();

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        public static string GetName<TEnum>(TEnum value) where TEnum : System.Enum
        {
            return value.ToString();
        }
    }
}
