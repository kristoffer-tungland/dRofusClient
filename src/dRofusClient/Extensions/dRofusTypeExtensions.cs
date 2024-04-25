namespace dRofusClient.Extensions;

public static class dRofusTypeExtensions
{
    public static string ToRequest(this dRofusType type)
    {
        return type.ToString().ToLower();
    }

    public static string CombineToRequest(this dRofusType type, List<string> parts)
    {
        parts.Insert(0, type.ToRequest());
        return string.Join("/", parts);
    }
    public static string CombineToRequest(this dRofusType type, string part2) => type.CombineToRequest([part2]);
    public static string CombineToRequest(this dRofusType type, string part2, string part3) => type.CombineToRequest([part2,part3]);
    public static string CombineToRequest(this dRofusType type, string part2, string part3, string part4) => type.CombineToRequest([part2,part3,part4]);

    public static string CombineToRequest(this dRofusType type, int? id) => type.CombineToRequest([id.ToRequest()]);
    public static string CombineToRequest(this dRofusType type, int? id, string part3) => type.CombineToRequest([id.ToRequest(),part3]);
    public static string CombineToRequest(this dRofusType type, int? id, string part3, string part4) => type.CombineToRequest([id.ToRequest(),part3,part4]);
}