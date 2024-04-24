namespace dRofusClient.Extensions;

public static class dRofusDtoExtensions
{
    static readonly JsonSerializerSettings _settings = new() { NullValueHandling = NullValueHandling.Ignore };

    public static dRofusBodyPatchOptions ToPatchOption(this dRofusDto dto)
    {
        var json = JsonConvert.SerializeObject(dto, _settings);
        return new dRofusBodyPatchOptions(json);
    }

    public static dRofusBodyPostOptions ToPostOption(this dRofusDto dto)
    {
        var json = JsonConvert.SerializeObject(dto, _settings);
        return new dRofusBodyPostOptions(json);
    }
}