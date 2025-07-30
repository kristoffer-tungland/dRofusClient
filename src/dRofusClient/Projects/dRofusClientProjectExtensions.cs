// ReSharper disable InconsistentNaming
namespace dRofusClient.Projects;

public static class dRofusClientProjectExtensions
{
    public static async Task<Project> GetProjectAsync(this IdRofusClient client, ItemQuery? options = null, CancellationToken cancellationToken = default)
    {
        return await client.SendAsync<Project>(HttpMethod.Get, dRofusType.Projects, options, cancellationToken);
    }
}