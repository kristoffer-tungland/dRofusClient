using dRofusClient.Occurrences;

namespace dRofusClient.Integration.Tests;

public class OccurrenceTests(SetupFixture fixture) : IClassFixture<SetupFixture>
{
    private readonly IdRofusClient _client = fixture.Client;

    [Fact]
    public async Task OccurenceTests()
    {
        var article = await _client.Create

        var occurence = await _client.CreateOccurrenceAsync(articleId: 15936);

        Assert.True(occurence.Id.HasValue,
            "Expected the created occurrence to have an ID assigned.");

        var query = Query.List()
            .Filter(Filter.Eq(Occurence.IdField, occurence.Id.Value));

        var recievedOccurence = await _client.GetOccurrencesAsync(query);

        Assert.True(recievedOccurence.Count == 1,
            "Expected to receive at least one occurrence from the query.");

        try
        {
            await _client.DeleteOccurrenceAsync(occurence.Id.Value);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Failed to delete occurrence with ID {occurence.Id.Value}: {ex.Message}");
        }

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await _client.GetOccurrenceAsync(occurence.Id.Value);
        });
    }
}
