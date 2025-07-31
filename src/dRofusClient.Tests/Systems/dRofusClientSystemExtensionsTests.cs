using System.Net.Http;
using dRofusClient.Systems;
using dRofusClient.Tests.Occurrences;
using dRofusClient.Extensions;
using dRofusClient.ApiLogs;

namespace dRofusClient.Tests.Systems;

public class dRofusClientSystemExtensionsTests
{
    [Fact]
    public async Task GetSystemLogsAsync_CallsSendListAsync()
    {
        var fake = new FakeRofusClient();
        fake.ListAsyncResult = new List<ApiLogs.SystemLog>();
        var query = Query.List();
        await fake.GetSystemLogsAsync(query);
        Assert.Equal("systems/logs", fake.LastRequest);
    }
}
