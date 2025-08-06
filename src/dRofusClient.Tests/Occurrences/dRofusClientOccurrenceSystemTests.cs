using dRofusClient.Occurrences;
using dRofusSystem = dRofusClient.Systems;
using dRofusClient.Options;
using dRofusClient.Extensions;

namespace dRofusClient.Tests.Occurrences;

public class dRofusClientOccurrenceSystemTests
{
    [Fact]
    public async Task GetOccurrenceSystemsAsync_CallsSendListAsync()
    {
        var fake = new FakeRofusClient();
        var query = new IsMemberOfSystemsQuery();
        fake.ListAsyncResult = new List<dRofusSystem.SystemInstance>();
        await fake.GetOccurrenceSystemsAsync(12, query);
        Assert.Equal("occurrences/12/is-member-of-systems", fake.LastRequest);
    }
}
