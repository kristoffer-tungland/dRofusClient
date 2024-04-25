using dRofusClient.Extensions;
using dRofusClient.Occurrences;

namespace dRofusClient.Tests;

public sealed class dRofusBodyPatchOptionsTest
{
    [Fact]
    internal void BodyPostOptionsTest()
    {
        var occurence = new dRofusOccurence { Id = 65 };
        var bodyOptions = occurence.ToPostOption();
        Assert.Equal("""{}""", bodyOptions.Body);
    }

    [Fact]
    internal void BodyPostOptionsTest_WithAdditionalProperties()
    {
        var occurence = new dRofusOccurence { Id = 65, AdditionalProperties = { { "prop1", "val1" } } };
        var bodyOptions = occurence.ToPostOption();
        Assert.Equal("""{"prop1":"val1"}""", bodyOptions.Body);
    }

    [Fact]
    internal void BodyPatchOptionsTest()
    {
        var occurence = new dRofusOccurence { Id = 65, ArticleId = 1 };
        var bodyOptions = occurence.ToPatchOption();
        Assert.Equal("""{"article_id":1}""", bodyOptions.Body);
    }

    [Fact]
    internal void BodyPatchOptionsTest_WithAdditionalProperties()
    {
        var occurence = new dRofusOccurence { Id = 65, AdditionalProperties = { { "prop1", "val1" } } };
        var bodyOptions = occurence.ToPatchOption();
        Assert.Equal("""{"prop1":"val1"}""", bodyOptions.Body);
    }
}