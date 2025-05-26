using dRofusClient.Windows;
using dRofusClient.PropertyMeta;
using dRofusClient.Projects;
using dRofusClient;

namespace dRofusClientDemo;

public class RofusClientDemo
{
    public IdRofusClient CreateClient(string BaseUrl, string Database, string ProjectId, string AuthenticationHeader)
    {
        var args = new dRofusConnectionArgs(BaseUrl, Database, ProjectId, AuthenticationHeader);
        return new dRofusClientFactory().Create(args);
    }

    public IdRofusClient CreateBasicAuthenticationClient()
    {
        var args = dRofusConnectionArgs.CreateNoServer("database", projectId: "01", "username", "password");
        return new dRofusClientFactory().Create(args);
    }

    public IdRofusClient CreateClientWithWindowsCredentials(string database)
    {
        return new dRofusClientFactory().Create(dRofusServers.GetNoServer(), database);
    }

    public async Task<dRofusProject> GetProject(IdRofusClient client)
    {
         return await client.GetProjectAsync();
    }

    public async Task<List<dRofusPropertyMeta>> GetProjectsPropertyMeta(IdRofusClient client)
    {
        return await client.GetPropertyMetaAsync(dRofusType.Projects);
    }

    public async Task<dRofusProject> GetProjectWithIdAndName(IdRofusClient client)
    {
        var options = dRofusOptions.Field()
            .Select(["id", "name"]);

        return await client.GetProjectAsync(options);
    }

    public async Task<string?> GetCustomPropertyForProject(IdRofusClient client, string customPropertyName)
    {
        var propertyMetadata = await client.GetPropertyMetaAsync(dRofusType.Projects);
        var customProperty = propertyMetadata.First(p => p.Name == customPropertyName);
        
        var options = dRofusOptions.Field()
            .Select(customProperty.Id);

        var project = await client.GetProjectAsync(options);
        return project.GetPropertyAsString(customProperty.Id);
    }

}
