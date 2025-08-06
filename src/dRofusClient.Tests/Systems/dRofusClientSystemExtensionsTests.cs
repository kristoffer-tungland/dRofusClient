using System.Net.Http;
using dRofusClient.Systems;
using SystemModel = dRofusClient.Systems.System;
using SC = dRofusClient.SystemComponents;
using dRofusClient.Tests.Occurrences;
using dRofusClient.Extensions;
using dRofusClient.ApiLogs;
using Files = dRofusClient.Files;

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

    [Fact]
    public async Task GetSystemsAsync_CallsSendListAsync()
    {
        var fake = new FakeRofusClient();
        fake.ListAsyncResult = new List<SystemModel>();
        var query = Query.List();
        await fake.GetSystemsAsync(query);
        Assert.Equal("systems", fake.LastRequest);
    }

    [Fact]
    public async Task GetSystemComponentsAsync_CallsSendListAsyncWithCorrectRoute()
    {
        var fake = new FakeRofusClient();
        fake.ListAsyncResult = new List<SC.Component>();
        var query = Query.List();
        await fake.GetSystemComponentsAsync(3, query);
        Assert.Equal("systems/3/components", fake.LastRequest);
    }

    [Fact]
    public async Task GetSystemFilesAsync_UsesCorrectRoute()
    {
        var fake = new FakeRofusClient();
        fake.ListAsyncResult = new List<Files.FileDetails>();
        var query = Query.List();
        await fake.GetSystemFilesAsync(7, query);
        Assert.Equal("systems/7/files", fake.LastRequest);
    }
}
