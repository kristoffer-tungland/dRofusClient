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
    public object? LastOptions { get; private set; }
    public CancellationToken LastToken { get; private set; }
    public object? PatchAsyncResult { get; set; }

    public HttpClient HttpClient => throw new NotImplementedException();

    public void ClearAuthentication()
    {
        throw new NotImplementedException();
    }

    public string? GetBaseUrl()
    {
        throw new NotImplementedException();
    }

    public (string database, string projectId) GetDatabaseAndProjectId()
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsLoggedIn()
    {
        throw new NotImplementedException();
    }

    public Task Login(dRofusConnectionArgs args, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Login(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Logout()
    {
        throw new NotImplementedException();
    }

    // PatchAsync extension calls this method
    public Task<TResult> SendAsync<TResult>(HttpMethod method, string route, dRofusOptionsBase options, CancellationToken cancellationToken) where TResult : dRofusDto, new()
    {
        // Only handle PATCH for this test
        if (method == HttpMethod.Patch)
        {
            LastRequest = route;
            LastOptions = options;
            LastToken = cancellationToken;
            return Task.FromResult((TResult)PatchAsyncResult!);
        }
        throw new NotImplementedException();
    }

    public Task<TResult> SendAsync<TResult>(HttpMethod method, dRofusType dRofusType, dRofusOptionsBase? options = null, CancellationToken cancellationToken = default) where TResult : dRofusDto, new()
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TResult>> SendListAsync<TResult>(HttpMethod method, dRofusType dRofusType, dRofusOptionsBase? options = null, CancellationToken cancellationToken = default) where TResult : dRofusDto
    {
        throw new NotImplementedException();
    }

    public Task<List<TResult>> SendListAsync<TResult>(HttpMethod method, string route, dRofusOptionsBase? options = null, CancellationToken cancellationToken = default) where TResult : dRofusDto
    {
        throw new NotImplementedException();
    }

    public void Setup(dRofusConnectionArgs args)
    {
        throw new NotImplementedException();
    }

    public void UpdateAuthentication(string authenticationHeader)
    {
        throw new NotImplementedException();
    }

    public void UpdateAuthentication(ModernLoginResult modernLoginResult)
    {
        throw new NotImplementedException();
    }

    // Implement other interface members as needed (throw NotImplementedException if not used)
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
        var options = new dRofusStatusPatchOptions
        {
            PropertyName = propertyName,
            Body = new dRofusStatusPatchBody { Code = "CODE123" }
        };
        var expectedRequest = dRofusType.Occurrences.CombineToRequest(occurrenceId, "statuses", options.StatusTypeId.ToString());
        var expectedResult = new dRofusStatusPatchBody { Code = "CODE123" };
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
        var options = new dRofusStatusPatchOptions
        {
            PropertyName = propertyName,
            Body = new dRofusStatusPatchBody { StatusId = 99 }
        };
        var expectedRequest = dRofusType.Occurrences.CombineToRequest(occurrenceId, "statuses", options.StatusTypeId.ToString());
        var expectedResult = new dRofusStatusPatchBody { StatusId = 99 };
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
}