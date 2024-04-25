namespace dRofusClient.Extensions;

public static class IdExtensions
{
    public static string ToRequest(this int? id)
    {
        return id.ToString() ?? throw new NullReferenceException("Id is not supplied");
    }
}