// ReSharper disable InconsistentNaming
namespace dRofusClient.Projects;

public static class dRofusClientProjectExtensions
{
    public static async Task<dRofusProject> GetProjectAsync(this IdRofusClient client, dRofusFieldsOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await client.SendAsync<dRofusProject>(HttpMethod.Get, dRofusType.Projects, options, cancellationToken);
    }
}