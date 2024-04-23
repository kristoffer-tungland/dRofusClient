using dRofusClient.Extensions;

namespace dRofusClient.Tests;

public class dRofusClientTests
{
    [Fact]
    internal void RequestUri()
    {
        var connection = dRofusConnectionArgs.CreateNoServer("test_database", "01", "username", "password");
        var client = new dRofusClient(new HttpClient());
        client.Setup(connection);
        var requestMessage = client.BuildRequest(HttpMethod.Get, dRofusType.Occurrences.ToRequest(), null);
        Assert.NotNull(requestMessage.RequestUri);
        Assert.Equal("/api/test_database/01/occurrences", requestMessage.RequestUri.ToString());
    }
}