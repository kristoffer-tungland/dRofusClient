using System.Net.Http;
using System.IO;
using dRofusClient.Files;
using dRofusClient.Items;
using dRofusClient.Extensions;
using dRofusClient.Tests.Occurrences; // reuse FakeRofusClient
using dRofusClient.ApiLogs;
using FileMeta = dRofusClient.Files.File;
using ImageMeta = dRofusClient.Files.Image;

namespace dRofusClient.Tests.Items;

public class dRofusClientItemExtensionsTests
{
    [Fact]
    public async Task GetItemFilesAsync_CallsSendListAsyncWithCorrectRoute()
    {
        var fake = new FakeRofusClient();
        var query = Query.List();
        fake.ListAsyncResult = new List<FileMeta>();
        await fake.GetItemFilesAsync(5, query);
        Assert.Equal(HttpMethod.Get, fake.LastMethod);
        Assert.Equal("items/5/files", fake.LastRequest);
        Assert.Equal(query, fake.LastOptions);
    }

    [Fact]
    public async Task UploadItemFileAsync_SendsHttpRequest()
    {
        var fake = new FakeRofusClient();
        fake.LastHttpRequest?.Dispose();
        var stream = new MemoryStream([1,2,3]);
        await fake.UploadItemFileAsync(4, stream, "file.bin", "desc");
        Assert.NotNull(fake.LastHttpRequest);
        Assert.Equal(HttpMethod.Post, fake.LastHttpRequest!.Method);
        Assert.Equal("http://localhost/api/db/pr/items/4/files", fake.LastHttpRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task AddItemFileLinkAsync_SendsPostRequest()
    {
        var fake = new FakeRofusClient();
        await fake.AddItemFileLinkAsync(7, 9);
        Assert.Equal(HttpMethod.Post, fake.LastHttpRequest!.Method);
        Assert.Equal("http://localhost/api/db/pr/items/7/files/9", fake.LastHttpRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task RemoveItemFileLinkAsync_SendsDeleteRequest()
    {
        var fake = new FakeRofusClient();
        await fake.RemoveItemFileLinkAsync(3, 2);
        Assert.Equal(HttpMethod.Delete, fake.LastHttpRequest!.Method);
        Assert.Equal("http://localhost/api/db/pr/items/3/files/2", fake.LastHttpRequest!.RequestUri!.ToString());
    }

    [Fact]
    public async Task GetItemImagesAsync_CallsSendListAsync()
    {
        var fake = new FakeRofusClient();
        fake.ListAsyncResult = new List<ImageMeta>();
        var query = Query.List();
        await fake.GetItemImagesAsync(11, query);
        Assert.Equal(HttpMethod.Get, fake.LastMethod);
        Assert.Equal("items/11/images", fake.LastRequest);
        Assert.Equal(query, fake.LastOptions);
    }

    [Fact]
    public async Task GetItemLogsAsync_CallsSendListAsync()
    {
        var fake = new FakeRofusClient();
        fake.ListAsyncResult = new List<ApiLogs.ItemLog>();
        var query = Query.List();
        await fake.GetItemLogsAsync(query);
        Assert.Equal("items/logs", fake.LastRequest);
    }
}
