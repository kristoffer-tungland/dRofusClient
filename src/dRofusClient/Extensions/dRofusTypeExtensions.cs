namespace dRofusClient.Extensions;

public static class dRofusTypeExtensions
{
    public static string ToRequest(this dRofusType dRofusType)
    {
        return dRofusType.ToString().ToLower();
    }
}