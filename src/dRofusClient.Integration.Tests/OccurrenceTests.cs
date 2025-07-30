using dRofusClient.Occurrences;

namespace dRofusClient.Integration.Tests;

//[Collection(nameof(OccurenceTestCollection))]
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
    public async Task CanCreateAndDeleteOccurrence()
    {
        var occurence = await _client.CreateOccurrenceAsync(CreateOccurence.Of(fixture.Item));

        Assert.True(occurence.Id.HasValue,
            "Expected the created occurrence to have an ID assigned.");

        var query = Query.List()
            .Filter(Filter.Eq(Occurence.IdField, occurence.GetId()));

        var recievedOccurence = await _client.GetOccurrencesAsync(query);

        Assert.True(recievedOccurence.Count == 1,
            "Expected to receive at least one occurrence from the query.");

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
}
