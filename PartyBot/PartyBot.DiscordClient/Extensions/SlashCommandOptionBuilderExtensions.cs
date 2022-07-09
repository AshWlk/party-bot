using Discord;
using System.ComponentModel;
using System.Reflection;

namespace PartyBot.DiscordClient.Extensions
{
    public static class SlashCommandOptionBuilderExtensions
    {
        public static SlashCommandOptionBuilder AddChoicesFromEnum<TEnum>(this SlashCommandOptionBuilder commandOptionBuilder)
            where TEnum : struct, Enum
        {
            var type = typeof(TEnum);
            foreach (var value in type.GetEnumValues())
            {
                var enumName = type.GetEnumName(value) ?? string.Empty;
                var member = type.GetMember(enumName)[0];

                if (member.GetCustomAttribute<DescriptionAttribute>() is not DescriptionAttribute attribute)
                {
                    throw new InvalidOperationException($"All members of the enum {nameof(TEnum)} must be decorated with the {nameof(DescriptionAttribute)}");
                }
                
                commandOptionBuilder.AddChoice(attribute.Description, (int)value);
            }

            return commandOptionBuilder;
        }
    }
}
