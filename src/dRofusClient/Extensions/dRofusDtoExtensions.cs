using Newtonsoft.Json.Linq;

namespace dRofusClient.Extensions;

public static class dRofusDtoExtensions
{
    public static dRofusBodyPatchOptions ToPatchOption(this dRofusDto dto)
    {
        var json = Json.Serialize(dto);
        json = RemoveIdField(json);
        return new dRofusBodyPatchOptions(json);
    }

    public static dRofusBodyPostOptions ToPostOption(this dRofusDto dto)
    {
        var json = Json.Serialize(dto);
        json = RemoveIdField(json);
        return new dRofusBodyPostOptions(json);
    }

    static string RemoveIdField(string json)
    {
        var jObject = JObject.Parse(json);
        jObject.Remove("id");
        return jObject.ToString(Formatting.None);
    }
}