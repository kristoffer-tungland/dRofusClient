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

    [Fact]
    public async Task GetOccurrenceSystemsAsync_PassesIncludeSubsToRequest()
    {
        var fake = new FakeRofusClient();
        var query = new IsMemberOfSystemsQuery
        {
            IncludeSubs = true
        };

        fake.ListAsyncResult = new List<dRofusSystem.SystemInstance>();

        await fake.GetOccurrenceSystemsAsync(34, query);

        Assert.Same(query, fake.LastOptions);
        Assert.Equal("includeSubs=true", fake.LastOptions!.GetParameters());
    }
}
