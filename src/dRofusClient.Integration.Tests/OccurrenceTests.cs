using dRofusClient.ApiLogs;
using dRofusClient.Models;
using dRofusClient.Occurrences;
using dRofusClient.Options;

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
    public async Task CanCreateOccurrenceWithCustomField()
    {
        var occurenceToCreate = CreateOccurence.Of(fixture.Item);
        occurenceToCreate.Set("occurrence_data_23_10_01_01", 5.5);

        var createdOccurrence = await _client.CreateOccurrenceAsync(occurenceToCreate);

        try
        {
            Assert.NotNull(createdOccurrence);
            Assert.True(createdOccurrence.Id.HasValue,
                "Expected the created occurrence to have an ID assigned.");

            Assert.Equal(fixture.Item.GetId(), createdOccurrence.ArticleId);
        }
        finally
        {
            await _client.DeleteOccurrenceAsync(createdOccurrence.GetId());
        }
    }    

    [Fact]
    public async Task CanCreateOccurrenceWithCustomStringField()
    {
        var occurenceToCreate = CreateOccurence.Of(fixture.Item);

        occurenceToCreate.Set(23100201, "banan");

        var createdOccurence = await _client.CreateOccurrenceAsync(occurenceToCreate);

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
    public async Task CanCreateOccurrenceWithCustomFieldUsingDBId()
    {
        // Field occurrence_data_23_10_01_01
        var occurenceToCreate = CreateOccurence.Of(fixture.Item);

        occurenceToCreate.Set(23100101, 5.5);

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

    [Fact]
    public async Task CanGetIsMemberOfSystems()
    {
        var occurenceToTest = 658; // Replace with a valid occurrence ID for testing

        var query = new IsMemberOfSystemsQuery
        {
            IncludeSubs = true
        };

        query.Select(Systems.SystemInstance.IdField, Systems.SystemInstance.NameField);

        var systems = await _client.GetOccurrenceSystemsAsync(occurenceToTest, query);
        Assert.NotNull(systems);
        Assert.NotEmpty(systems);
        Assert.All(systems, system =>
        {
            Assert.NotNull(system);
            Assert.True(system.Id > 0, "Expected system ID to be greater than zero.");
        });
    }

    [Fact]
    public async Task CanUseLowerLevelApiForOccurrence()
    {
        var occurence = await _client.GetAsync<Occurence>("occurrences/" + fixture.Occurence.GetId());
        Assert.NotNull(occurence);
        Assert.True(occurence.Id.HasValue,
            "Expected the occurrence to have an ID assigned.");

        Assert.Equal(fixture.Occurence.GetId(), occurence.GetId());
    }

}
