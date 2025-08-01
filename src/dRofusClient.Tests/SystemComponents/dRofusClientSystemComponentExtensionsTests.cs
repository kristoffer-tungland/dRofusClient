using dRofusClient.SystemComponents;
using dRofusClient.Tests.Occurrences;

namespace dRofusClient.Tests.SystemComponents;

public class dRofusClientSystemComponentExtensionsTests
{
    [Fact]
    public async Task GetSystemComponentsAsync_CallsSendListAsync()
    {
        var fake = new FakeRofusClient();
        fake.ListAsyncResult = new List<SystemComponent>();
        var query = Query.List();
        await fake.GetSystemComponentsAsync(query);
        Assert.Equal("systemcomponents", fake.LastRequest);
    }

    [Fact]
    public async Task GetSystemComponentComponentsAsync_UsesCorrectRoute()
    {
        var fake = new FakeRofusClient();
        fake.ListAsyncResult = new List<Component>();
        var query = Query.List();
        await fake.GetSystemComponentComponentsAsync(4, query);
        Assert.Equal("systemcomponents/4/components", fake.LastRequest);
    }
}
