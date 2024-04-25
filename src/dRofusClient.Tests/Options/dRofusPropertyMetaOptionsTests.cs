using dRofusClient.Options;

namespace dRofusClient.Tests.Options;

public sealed class dRofusPropertyMetaOptionsTests
{
    [Fact]
    internal void MetaPostOptionsTest()
    {
        var options = new dRofusPropertyMetaOptions(4);
        var parameters = options.GetParameters();
        Assert.Equal("depth=4", parameters);
    }

}