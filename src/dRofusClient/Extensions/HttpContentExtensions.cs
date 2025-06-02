using System.IO;

namespace dRofusClient.Extensions;

internal static class HttpContentExtensions
{
    internal static async Task<T?> ReadFromJsonAsync<T>(this HttpContent content, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();
        await content.CopyToAsync(stream);
        stream.Position = 0;
        return await Json.DeserializeAsync<T>(stream, cancellationToken);
    }
}