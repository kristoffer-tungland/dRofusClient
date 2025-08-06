using dRofusClient.SystemComponents;

namespace dRofusClient.Integration.Tests;

public class SystemComponentTests(SystemComponentFixture fixture) : IClassFixture<SystemComponentFixture>
{
    private readonly IdRofusClient _client = fixture.Client;

    [Fact]
    public async Task CanGetSystemComponents()
    {
        var query = Query.List().Filter(Filter.Eq(SystemComponent.IdField, fixture.SystemComponent.GetId()));

        var components = await _client.GetSystemComponentsAsync(query);
        Assert.NotEmpty(components);
        Assert.Single(components);
        Assert.Equal(fixture.SystemComponent.Id, components[0].Id);
    }
    [Fact]
    public async Task CanGetSystemComponent()
    {
        var component = await _client.GetSystemComponentAsync(fixture.SystemComponent.GetId());
        Assert.NotNull(component);
        Assert.Equal(fixture.SystemComponent.Id, component.Id);
    }

    [Fact]
    public async Task CanGetListOfComponentsInASystem()
    {
        var query = Query.List();
        var components = await _client.GetSystemComponentComponentsAsync(fixture.SystemComponent.GetId(), query);
        Assert.NotEmpty(components);
        Assert.All(components, component => Assert.NotEqual(0, component.Id));
    }
}
