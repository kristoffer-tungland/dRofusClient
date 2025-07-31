using dRofusClient.Extensions;
using dRofusClient.Occurrences;

namespace dRofusClient.Tests;

public sealed class dRofusDtoExtensionsTests
{
    [Fact]
    internal void ToPostRequestTest()
    {
        var occurence = new Occurence { Id = 65 };
        var bodyOptions = occurence.ToPostRequest();
        Assert.Equal("""{}""", bodyOptions.Body);
        Assert.Null(bodyOptions.StatusFields); // Add check for new type
    }

    [Fact]
    internal void ToPostRequestTest_WithAdditionalProperties()
    {
        var occurence = new Occurence { Id = 65, AdditionalProperties = { { "prop1", "val1" } } };
        var bodyOptions = occurence.ToPostRequest();
        Assert.Equal("""{"prop1":"val1"}""", bodyOptions.Body);
        // StatusFields should still be null unless a status field is present
        Assert.Null(bodyOptions.StatusFields);
    }

    [Fact]
    internal void ToPatchRequestTest()
    {
        var occurence = new Occurence { Id = 65, ArticleId = 1 };
        var bodyOptions = occurence.ToPatchRequest();
        Assert.Equal("""{"article_id":1}""", bodyOptions.Body);
        Assert.Null(bodyOptions.StatusFields);
    }

    [Fact]
    internal void ToPatchRequestTest_WithAdditionalProperties()
    {
        var occurence = new Occurence { Id = 65, AdditionalProperties = { { "prop1", "val1" } } };
        var bodyOptions = occurence.ToPatchRequest();
        Assert.Equal("""{"prop1":"val1"}""", bodyOptions.Body);
        Assert.Null(bodyOptions.StatusFields);
    }
}