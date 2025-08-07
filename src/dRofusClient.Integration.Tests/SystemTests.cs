using dRofusClient.Systems;

namespace dRofusClient.Integration.Tests;

public class SystemTests(SystemFixture fixture) : IClassFixture<SystemFixture>
{
    private readonly IdRofusClient _client = fixture.Client;
    [Fact]
    public async Task CanGetSystems()
    {
        var systemComponentId = fixture.System.SystemComponentId!.Value;

        var query = Query.List()
            .Filter(Filter.Eq(SystemInstance.SystemComponentIdField, systemComponentId))
            .Select(SystemInstance.IdField, SystemInstance.NumberField, SystemInstance.NameField);

        var systems = await _client.GetSystemsAsync(query);

        Assert.NotEmpty(systems);
        Assert.Contains(systems, system => system.Id == fixture.System.Id);
    }
    [Fact]
    public async Task CanGetSystem()
    {
        var system = await _client.GetSystemAsync(fixture.System.GetId());
        Assert.NotNull(system);
        Assert.Equal(fixture.System.Id, system.Id);
        Assert.Equal(fixture.System.Number, system.Number);
        Assert.Equal(fixture.System.Name, system.Name);
    }

    [Fact]
    public async Task CanUpdateSystem()
    {
        
        var systemInstance = new SystemInstance
        {
            Id = fixture.System.Id
        };

        systemInstance.SerialNumber = GetRandomNumber();
        systemInstance.Description = "Do not delete this system is used in tests " + DateTime.Now.ToString("yyyyMMddHHmmss");

        var system = await _client.UpdateSystemAsync(systemInstance);
        Assert.NotNull(system);
        Assert.Equal(fixture.System.Id, system.Id);
        Assert.Equal(systemInstance.Description, system.Description);
    }

    private string GetRandomNumber()
    {
        var random = new Random();
        return random.Next(1000, 9999).ToString();

    }

    [Fact]
    public async Task CanGetComponentsInASystem()
    {
        var query = Query.List().Select("id");

        var system = await _client.GetSystemComponentsAsync(fixture.System.GetId(), query);
        Assert.NotEmpty(system);
        Assert.All(system, component => Assert.NotEqual(0, component.Id));
    }

    [Fact]
    public async Task CanGetSystemLogs()
    {
        var logs = await _client.GetSystemLogsAsync(fixture.System.GetId(), Query.List());
        Assert.NotEmpty(logs);
    }

    [Fact]
    public async Task CanGetLogsForAllSystems()
    {
        var query = Query.List()
            .Filter(Filter.Eq(ApiLogs.SystemLog.SystemIdField, fixture.System.GetId()));

        var logs = await _client.GetSystemLogsAsync(query);
        Assert.NotEmpty(logs);
        Assert.All(logs, log => Assert.Equal(fixture.System.Id, log.SystemId));
    }
}
