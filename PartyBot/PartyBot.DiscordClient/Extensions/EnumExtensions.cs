using System.ComponentModel;
using System.Reflection;

namespace PartyBot.DiscordClient.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum @enum)
            where TEnum : struct, Enum
        {
            var type = typeof(TEnum);
            var enumName = type.GetEnumName(@enum);

            if (enumName == null)
            {
                return string.Empty;
            }

            var member = type.GetMember(enumName)[0];

            if (member.GetCustomAttribute<DescriptionAttribute>() is not DescriptionAttribute attribute)
            {
                return string.Empty;
            }

            return attribute.Description;
        }
    }
}
