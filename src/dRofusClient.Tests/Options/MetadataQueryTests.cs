namespace dRofusClient.Tests.Options;

public sealed class MetadataQueryTests
{
    [Fact]
    internal void MetadataQuery_GetParameters_DepthTest()
    {
        var metadataQuery = Query.Metadata(4);
        var parameters = metadataQuery.GetParameters();
        Assert.Equal("depth=4", parameters);
    }
}