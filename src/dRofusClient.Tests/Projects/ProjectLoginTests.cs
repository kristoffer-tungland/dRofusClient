using System.Net;
using System.Text;
using dRofusClient.Helpers;
using dRofusClient.Projects;

namespace dRofusClient.Tests.Projects;

public class ProjectLoginTests
{
    [Fact]
    public async Task Login_ShouldAcceptProjectResponse_WhenIdIsQuoted()
    {
        using var http = new HttpClient(new ProjectResponseHandler("{\"id\":\"01\",\"name\":\"Example project\"}"));
        using var client = new global::dRofusClient.dRofusClient(http, new NonePromptHandler());
        var connection = new dRofusConnectionArgs("db2.nosyko.no", "example", "01");

        await client.Login(connection);

        Assert.Equal(("example", "01"), client.GetDatabaseAndProjectId());
    }

    [Theory]
    [InlineData("{\"id\":\"01\"}")]
    [InlineData("{\"id\":1}")]
    public async Task DeserializeProject_ShouldReadNumericProjectIds(string json)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        Project? project = await Json.DeserializeAsync<Project>(stream, CancellationToken.None);

        Assert.NotNull(project);
        Assert.Equal(1, project.Id);
    }

    private sealed class ProjectResponseHandler(string json) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            });
    }
}
