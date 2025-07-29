using dRofusClient.Extensions;
using dRofusClient.Occurrences;

namespace dRofusClient.Tests;

public sealed class OccurencePostAndPatchBodyTests
{
    [Fact]
    internal void BodyPostRequestTest()
    {
        var occurence = new Occurence { Id = 65 };
        var bodyOptions = occurence.ToPostRequest();
        Assert.Equal("""{}""", bodyOptions.Body);
    }

    [Fact]
    internal void BodyPostOptionsTest_WithAdditionalProperties()
    {
        var occurence = new Occurence { Id = 65, AdditionalProperties = { { "prop1", "val1" } } };
        var bodyOptions = occurence.ToPostRequest();
        Assert.Equal("""{"prop1":"val1"}""", bodyOptions.Body);
    }

    [Fact]
    internal void BodyPatchRequestTest()
    {
        var occurence = new Occurence { Id = 65, ArticleId = 1 };
        var bodyOptions = occurence.ToPatchRequest();
        Assert.Equal("""{"article_id":1}""", bodyOptions.Body);
    }

    [Fact]
    internal void BodyPatchRequestTest_WithAdditionalProperties()
    {
        var occurence = new Occurence { Id = 65, AdditionalProperties = { { "prop1", "val1" } } };
        var bodyOptions = occurence.ToPatchRequest();
        Assert.Equal("""{"prop1":"val1"}""", bodyOptions.Body);
    }
}