using dRofusClient.Extensions;
using dRofusClient.Occurrences;

namespace dRofusClient.Tests;

public sealed class dRofusClientTests
{
    private static dRofusClient CreateClient()
    {
        var connection = dRofusConnectionArgs.CreateNoServer("test_database", "01", "username", "password");
        var client = new dRofusClient(new HttpClient(), new NonePromptHandler());
        client.Setup(connection);
        return client;
    }

    [Fact]
    internal void RequestUri()
    {
        var client = CreateClient();
        var requestMessage = client.BuildRequest(HttpMethod.Get, dRofusType.Occurrences.ToRequest(), null);
        Assert.NotNull(requestMessage.RequestUri);
        Assert.Equal("/api/test_database/01/occurrences", requestMessage.RequestUri.ToString());
    }

    [Fact]
    internal async Task RequestUri_Patch()
    {
        var occurence = new Occurence { Id = 65, AdditionalProperties = { { "prop1", "val1" } } };
        var patchOptions = occurence.ToPatchRequest();

        var client = CreateClient();
        var requestMessage = client.BuildRequest(HttpMethod.Patch, dRofusType.Occurrences.CombineToRequest(occurence.Id), patchOptions);
        
        Assert.NotNull(requestMessage.Content);
        var jsonContent = await requestMessage.Content.ReadAsStringAsync();
        Assert.Equal("""{"prop1":"val1"}""", jsonContent);

        var mediaType = requestMessage.Content.Headers?.ContentType?.MediaType;
        Assert.Equal("application/merge-patch+json", mediaType);
        
        Assert.NotNull(requestMessage.RequestUri);
        Assert.Equal("/api/test_database/01/occurrences/65", requestMessage.RequestUri.ToString());
    }

    [Fact]
    internal async Task RequestUri_Post()
    {
        var occurence = new Occurence { Id = 65, AdditionalProperties = { { "prop1", "val1" } } };
        var patchOptions = occurence.ToPostRequest();

        var client = CreateClient();
        var requestMessage = client.BuildRequest(HttpMethod.Post, dRofusType.Occurrences.CombineToRequest(occurence.Id), patchOptions);
        
        Assert.NotNull(requestMessage.Content);
        var jsonContent = await requestMessage.Content.ReadAsStringAsync();
        Assert.Equal("""{"prop1":"val1"}""", jsonContent);

        var mediaType = requestMessage.Content.Headers?.ContentType?.MediaType;
        Assert.Equal("application/json", mediaType);
        
        Assert.NotNull(requestMessage.RequestUri);
        Assert.Equal("/api/test_database/01/occurrences/65", requestMessage.RequestUri.ToString());
    }

    [Fact]
    internal void BaseUrl_NormalizeServerAddress_WithApiPrefix()
    {
        // Test that "api-no.drofus.com" is normalized to "https://api-no.drofus.com"
        // without adding an extra "https://api-" prefix
        var connection = dRofusConnectionArgs.Create("api-no.drofus.com", "test_database", "01", "username", "password");
        Assert.Equal("https://api-no.drofus.com", connection.BaseUrl);
    }
}