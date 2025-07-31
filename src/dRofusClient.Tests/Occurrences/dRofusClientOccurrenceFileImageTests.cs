using System.Net.Http;
using dRofusClient.Files;
using dRofusClient.Occurrences;
using dRofusClient.Extensions;
using dRofusClient.ApiLogs;
using FileMeta = dRofusClient.Files.FileDetails;
using ImageMeta = dRofusClient.Files.Image;

namespace dRofusClient.Tests.Occurrences;

public class dRofusClientOccurrenceFileImageTests
{
    [Fact]
    public async Task GetOccurrenceFilesAsync_CallsSendListAsync()
    {
        var fake = new FakeRofusClient();
        var query = Query.List();
        fake.ListAsyncResult = new List<FileMeta>();
        await fake.GetOccurrenceFilesAsync(8, query);
        Assert.Equal(HttpMethod.Get, fake.LastMethod);
        Assert.Equal("occurrences/8/files", fake.LastRequest);
        Assert.Equal(query, fake.LastOptions);
    }

    [Fact]
    public async Task GetOccurrenceImagesAsync_CallsSendListAsync()
    {
        var fake = new FakeRofusClient();
        var query = Query.List();
        fake.ListAsyncResult = new List<ImageMeta>();
        await fake.GetOccurrenceImagesAsync(2, query);
        Assert.Equal("occurrences/2/images", fake.LastRequest);
    }

    [Fact]
    public async Task GetOccurrenceLogsAsync_CallsSendListAsync()
    {
        var fake = new FakeRofusClient();
        fake.ListAsyncResult = new List<ApiLogs.OccurrenceLog>();
        var query = Query.List();
        await fake.GetOccurrenceLogsAsync(query);
        Assert.Equal("occurrences/logs", fake.LastRequest);
    }
}
