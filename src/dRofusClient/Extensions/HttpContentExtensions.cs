namespace dRofusClient.Extensions;

internal static class HttpContentExtensions
{
    internal static async Task<T?> ReadFromJsonAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync();
        return Json.Deserialize<T>(json);
    }
}