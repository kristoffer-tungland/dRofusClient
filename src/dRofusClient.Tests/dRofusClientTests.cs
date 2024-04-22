using dRofusClient.Extensions;

namespace dRofusClient.Tests;

public class dRofusClientTests
{
    [Fact]
    internal void GetBase()
    {
        var connection = dRofusConnectionArgs.CreateNoServer("test_database", "01", "username", "password");
        var client = new dRofusClient();
        client.Setup(connection);
        var requestMessage = client.BuildRequest(HttpMethod.Get, dRofusType.Occurrences.ToRequest(), null);

        Assert.NotNull(requestMessage.RequestUri);
        Assert.Equal("/api/test_database/01/occurrences", requestMessage.RequestUri.ToString());
    }
}