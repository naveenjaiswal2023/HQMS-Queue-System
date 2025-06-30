using System.ComponentModel;
using System.Reflection;

namespace HQMS.API.Shared.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription<TEnum>(TEnum value) where TEnum : Enum
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null)
                return value.ToString();

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        public static string GetName<TEnum>(TEnum value) where TEnum : Enum
        {
            return value.ToString();
        }
    }
}
