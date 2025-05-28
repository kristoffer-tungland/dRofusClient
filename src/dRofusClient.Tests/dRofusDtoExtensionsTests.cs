using dRofusClient.Extensions;
using dRofusClient.Occurrences;

namespace dRofusClient.Tests;

public sealed class dRofusDtoExtensionsTests
{
    [Fact]
    internal void ToPostOptionTest()
    {
        var occurence = new dRofusOccurence { Id = 65 };
        var bodyOptions = occurence.ToPostOption();
        Assert.Equal("""{}""", bodyOptions.Body);
        Assert.Null(bodyOptions.StatusFields); // Add check for new type
    }

    [Fact]
    internal void ToPostOptionTest_WithAdditionalProperties()
    {
        var occurence = new dRofusOccurence { Id = 65, AdditionalProperties = { { "prop1", "val1" } } };
        var bodyOptions = occurence.ToPostOption();
        Assert.Equal("""{"prop1":"val1"}""", bodyOptions.Body);
        // StatusFields should still be null unless a status field is present
        Assert.Null(bodyOptions.StatusFields);
    }

    [Fact]
    internal void ToPatchOptionTest()
    {
        var occurence = new dRofusOccurence { Id = 65, ArticleId = 1 };
        var bodyOptions = occurence.ToPatchOption();
        Assert.Equal("""{"article_id":1}""", bodyOptions.Body);
        Assert.Null(bodyOptions.StatusFields);
    }

    [Fact]
    internal void ToPatchOptionTest_WithAdditionalProperties()
    {
        var occurence = new dRofusOccurence { Id = 65, AdditionalProperties = { { "prop1", "val1" } } };
        var bodyOptions = occurence.ToPatchOption();
        Assert.Equal("""{"prop1":"val1"}""", bodyOptions.Body);
        Assert.Null(bodyOptions.StatusFields);
    }
}