using dRofusClient.Extensions;
using dRofusClient.Occurrences;

namespace dRofusClient.Tests;

public sealed class dRofusBodyPatchOptionsTest
{
    [Fact]
    internal void BodyOptionsTest()
    {
        var occurence = new dRofusOccurence { Id = 65 };
        var bodyOptions = occurence.ToPostOption();
        Assert.Equal("""{"id":65}""", bodyOptions.Body);
    }

    [Fact]
    internal void BodyOptionsTest_WithAdditionalProperties()
    {
        var occurence = new dRofusOccurence { Id = 65, AdditionalProperties = {{"prop1", "val1"}}};
        var bodyOptions = occurence.ToPostOption();
        Assert.Equal("""{"id":65,"prop1":"val1"}""", bodyOptions.Body);
    }
}