using System.IO;
using System.Text.Json;

namespace dRofusClient.Helpers;

public static class Json
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string Serialize(object obj) => JsonSerializer.Serialize(obj, Options);

    public static async Task<T?> DeserializeAsync<T>(Stream json, CancellationToken cancellationToken) 
        => await JsonSerializer.DeserializeAsync<T>(json, Options, cancellationToken);
}

// Custom naming policy for snake_case
public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}