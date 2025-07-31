using System.Net.Http;
using dRofusClient.Rooms;
using dRofusClient.Tests.Occurrences; // reuse FakeRofusClient
using dRofusClient.Extensions;
using FileMeta = dRofusClient.Files.File;

namespace dRofusClient.Tests.Rooms;

public class dRofusClientRoomExtensionsTests
{
    [Fact]
    public async Task GetRoomsAsync_CallsSendListAsync()
    {
        var fake = new FakeRofusClient();
        var query = Query.List();
        fake.ListAsyncResult = new List<Room>();
        await fake.GetRoomsAsync(query);
        Assert.Equal("rooms", fake.LastRequest);
    }

    [Fact]
    public async Task GetRoomFilesAsync_UsesCorrectRoute()
    {
        var fake = new FakeRofusClient();
        var query = Query.List();
        fake.ListAsyncResult = new List<FileMeta>();
        await fake.GetRoomFilesAsync(5, query);
        Assert.Equal("rooms/5/files", fake.LastRequest);
    }

    [Fact]
    public async Task GetRoomImageAsync_BuildsFullUrl()
    {
        var fake = new FakeRofusClient();
        await fake.GetRoomImageAsync(3);
        Assert.Equal("http://localhost/api/db/pr/rooms/images/3", fake.LastHttpRequest!.RequestUri!.ToString());
        Assert.Equal(HttpMethod.Get, fake.LastHttpRequest!.Method);
    }
}
