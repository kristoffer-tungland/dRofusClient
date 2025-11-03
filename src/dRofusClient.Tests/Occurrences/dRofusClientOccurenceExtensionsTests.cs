using System;
using System.Net;
using System.Net.Http;
using dRofusClient;
using dRofusClient.Occurrences;
using dRofusClient.Options;
using dRofusClient.Extensions;
using dRofusClient.Bases;
using dRofusClient.Models;

namespace dRofusClient.Tests.Occurrences;

// Add a fake client that implements SendAsync to support PatchAsync extension
public class FakeRofusClient : IdRofusClient
{
    public object? LastRequest { get; private set; }
    public RequestBase? LastOptions { get; private set; }
    public CancellationToken LastToken { get; private set; }
    public HttpMethod? LastMethod { get; private set; }
    public HttpRequestMessage? LastHttpRequest => _handler.LastRequest;

    public object? PatchAsyncResult { get; set; }
    public object? ListAsyncResult { get; set; }

    private readonly FakeHandler _handler = new();
    public HttpClient HttpClient { get; }

    public FakeRofusClient()
    {
        HttpClient = new HttpClient(_handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
    }

    private class FakeHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public HttpResponseMessage Response { get; set; } = new(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        };

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(Response);
        }
    }

    public void ClearAuthentication() => throw new NotImplementedException();
    public void Dispose() { }
    public string? GetBaseUrl() => HttpClient.BaseAddress?.ToString();
    public (string database, string projectId) GetDatabaseAndProjectId() => ("db", "pr");
    public Task<bool> IsLoggedIn() => Task.FromResult(true);
    public Task Login(dRofusConnectionArgs args, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Login(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Logout() { }

    public Task<TResult> SendAsync<TResult>(HttpMethod method, string route, RequestBase options, CancellationToken cancellationToken) where TResult : dRofusDto, new()
    {
        LastMethod = method;
        LastRequest = route;
        LastOptions = options;
        LastToken = cancellationToken;
        if (method == HttpMethod.Patch && PatchAsyncResult is TResult r)
            return Task.FromResult(r);
        throw new NotImplementedException();
    }

    public Task<TResult> SendAsync<TResult>(HttpMethod method, dRofusType dRofusType, RequestBase? options = null, CancellationToken cancellationToken = default) where TResult : dRofusDto, new() =>
        SendAsync<TResult>(method, dRofusType.ToRequest(), options ?? new ListQuery(), cancellationToken);

    public Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default) =>
        HttpClient.SendAsync(request, cancellationToken);

    public Task<List<TResult>> SendListAsync<TResult>(HttpMethod method, dRofusType dRofusType, RequestBase? options = null, CancellationToken cancellationToken = default) where TResult : dRofusDto =>
        SendListAsync<TResult>(method, dRofusType.ToRequest(), options, cancellationToken);

    public Task<List<TResult>> SendListAsync<TResult>(HttpMethod method, string route, RequestBase? options = null, CancellationToken cancellationToken = default) where TResult : dRofusDto
    {
        LastMethod = method;
        LastRequest = route;
        LastOptions = options;
        LastToken = cancellationToken;
        if (ListAsyncResult is List<TResult> list)
            return Task.FromResult(list);
        return Task.FromResult(new List<TResult>());
    }

    public void Setup(dRofusConnectionArgs args) => throw new NotImplementedException();
    public void UpdateAuthentication(string authenticationHeader) => throw new NotImplementedException();
    public void UpdateAuthentication(ModernLoginResult modernLoginResult) => throw new NotImplementedException();
}

public class dRofusClientOccurenceExtensionsTests
{
    [Fact]
    public async Task UpdateOccurrenceStatusAsync_CallsPatchAsyncWithCorrectParameters()
    {
        // Arrange
        var fakeClient = new FakeRofusClient();
        int occurrenceId = 123;
        var propertyName = "occurrence_classification_156_classification_entry_id_code";
        var options = new StatusPatchRequest
        {
            PropertyName = propertyName,
            Body = new StatusPatchBody { Code = "CODE123" }
        };
        var expectedRequest = dRofusType.Occurrences.CombineToRequest(occurrenceId, "statuses", options.StatusTypeId.ToString());
        var expectedResult = new StatusPatchBody { Code = "CODE123" };
        fakeClient.PatchAsyncResult = expectedResult;

        // Act
        var result = await fakeClient.UpdateOccurrenceStatusAsync(
            occurrenceId, options, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult.Code, result.Code);
        Assert.Equal(expectedResult.StatusId, result.StatusId);
        Assert.Equal(expectedRequest, fakeClient.LastRequest);
        Assert.Equal(options, fakeClient.LastOptions);
        Assert.Equal(156, options.StatusTypeId);
    }

    [Fact]
    public async Task UpdateOccurrenceStatusAsync_PassesCancellationToken()
    {
        // Arrange
        var fakeClient = new FakeRofusClient();
        int occurrenceId = 456;
        var propertyName = "occurrence_classification_42_classification_entry_id_id";
        var options = new StatusPatchRequest
        {
            PropertyName = propertyName,
            Body = new StatusPatchBody { StatusId = 99 }
        };
        var expectedRequest = dRofusType.Occurrences.CombineToRequest(occurrenceId, "statuses", options.StatusTypeId.ToString());
        var expectedResult = new StatusPatchBody { StatusId = 99 };
        var cancellationToken = new CancellationTokenSource().Token;
        fakeClient.PatchAsyncResult = expectedResult;

        // Act
        var result = await fakeClient.UpdateOccurrenceStatusAsync(
            occurrenceId, options, cancellationToken);

        // Assert
        Assert.Equal(expectedResult.StatusId, result.StatusId);
        Assert.Equal(expectedRequest, fakeClient.LastRequest);
        Assert.Equal(options, fakeClient.LastOptions);
        Assert.Equal(cancellationToken, fakeClient.LastToken);
        Assert.Equal(42, options.StatusTypeId);
    }

    [Fact]
    public async Task CreateOccurrenceAsync_WithRoomWithoutRoomScheduleId_ThrowsArgumentException()
    {
        var fakeClient = new FakeRofusClient();
        var occurenceToCreate = new CreateOccurence
        {
            ArticleId = 1,
            RoomId = 5
        };

        await Assert.ThrowsAsync<ArgumentException>(() => fakeClient.CreateOccurrenceAsync(occurenceToCreate));
    }

    [Fact]
    public async Task UpdateOccurrenceAsync_WithRoomWithoutRoomScheduleId_ThrowsArgumentException()
    {
        var fakeClient = new FakeRofusClient();
        var occurence = new Occurence
        {
            Id = 10,
            RoomId = 3
        };

        await Assert.ThrowsAsync<ArgumentException>(() => fakeClient.UpdateOccurrenceAsync(occurence));
    }
}