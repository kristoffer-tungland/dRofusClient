using dRofusClient.ApiLogs;
using dRofusClient.Occurrences;

namespace dRofusClient.Integration.Tests;

public class OccurrenceTests(OccurenceFixture fixture) : IClassFixture<OccurenceFixture>
{
    private readonly IdRofusClient _client = fixture.Client;

    [Fact]
    public async Task CanGetOccurrences()
    {
        var query = Query.List()
            .Filter(Filter.Eq(Occurence.IdField, fixture.Occurence.GetId()));
        var occurrences = await _client.GetOccurrencesAsync(query);
        Assert.NotNull(occurrences);
        Assert.True(occurrences.Count != 0,
            "Expected to receive at least one occurrence from the query.");

        var occurence = occurrences[0];

        Assert.True(occurence.Id.HasValue,
            "Expected the occurrence to have an ID assigned.");

        Assert.Equal(fixture.Occurence.GetId(), occurence.GetId());
    }

    [Fact]
    public async Task CanGetOccurrence()
    {
        var occurence = await _client.GetOccurrenceAsync(fixture.Occurence.GetId());
        Assert.NotNull(occurence);
        Assert.True(occurence.Id.HasValue,
            "Expected the occurrence to have an ID assigned.");
        Assert.Equal(fixture.Occurence.GetId(), occurence.GetId());
    }

    [Fact]
    public async Task CanCreateOccurrence()
    {
        var createdOccurence = await _client.CreateOccurrenceAsync(CreateOccurence.Of(fixture.Item));
        
        try
        {
            Assert.NotNull(createdOccurence);
            Assert.True(createdOccurence.Id.HasValue,
                "Expected the created occurrence to have an ID assigned.");

            Assert.Equal(fixture.Item.GetId(), createdOccurence.ArticleId);
        }
        finally
        {
            await _client.DeleteOccurrenceAsync(createdOccurence.GetId());
        }
    }

    [Fact]
    public async Task CanEditOccurrence()
    {
        var occurence = await _client.CreateOccurrenceAsync(CreateOccurence.Of(fixture.Item));
        try
        {
            occurence.OccurrenceName = "Updated Occurrence Name " + DateTime.Now.ToString("yyyyMMddHHmmss");
            var updatedOccurence = await _client.UpdateOccurrenceAsync(occurence);
            Assert.NotNull(updatedOccurence);
            Assert.Equal(occurence.GetId(), updatedOccurence.GetId());
            Assert.Equal(occurence.OccurrenceName, updatedOccurence.OccurrenceName);
        }
        finally
        {
            await _client.DeleteOccurrenceAsync(occurence.GetId());
        }
    }

    [Fact]
    public async Task CanDeleteOccurrence()
    {
        var occurence = await _client.CreateOccurrenceAsync(CreateOccurence.Of(fixture.Item));

        try
        {
            await _client.DeleteOccurrenceAsync(occurence.GetId());
        }
        catch (Exception ex)
        {
            Assert.Fail($"Failed to delete occurrence with ID {occurence.GetId()}: {ex.Message}");
        }

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await _client.GetOccurrenceAsync(occurence.GetId());
        });
    }

    [Fact]
    public async Task CanGetLogsForOccurrence()
    {
        var occurence = await _client.CreateOccurrenceAsync(CreateOccurence.Of(fixture.Item));
        try
        {
            var query = Query.List()
                .Filter(Filter.Eq(OccurrenceLog.OccurrenceIdField, occurence.GetId()))
                ;

            var logs = await _client.GetOccurrenceLogsAsync(query);
            Assert.NotNull(logs);
            Assert.NotEmpty(logs);

            var logForOccurrence = logs.Where(log => log.OccurrenceId == occurence.GetId()).ToList();

            Assert.NotEmpty(logForOccurrence);
            Assert.All(logForOccurrence, log =>
            {
                Assert.NotNull(log);
                Assert.Contains("New Occurrence", log.Action);
                Assert.Equal(occurence.GetId(), log.OccurrenceId);
            });
        }
        finally
        {
            await _client.DeleteOccurrenceAsync(occurence.GetId());
        }

    }
}
