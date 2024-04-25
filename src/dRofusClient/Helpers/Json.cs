using Newtonsoft.Json.Serialization;

namespace dRofusClient.Helpers;

public static class Json
{
    public static readonly JsonSerializerSettings Settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy(),
        }
    };

    public static string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, Settings);
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }
}