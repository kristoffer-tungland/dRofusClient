using System.Text.Json;

namespace dRofusClient.JsonConverters
{
    public class GenericEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        private static readonly TEnum FirstEnumValue = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().First();

        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var enumString = reader.GetString();
                if (Enum.TryParse<TEnum>(enumString, true, out var result))
                {
                    return result;
                }
                return FirstEnumValue;
            }
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out int intValue) && Enum.IsDefined(typeof(TEnum), intValue))
                {
                    return (TEnum)Enum.ToObject(typeof(TEnum), intValue);
                }
                return FirstEnumValue;
            }
            return FirstEnumValue;
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}