using dRofusClient.Windows;
using dRofusClient.PropertyMeta;
using dRofusClient.Projects;
using dRofusClient;

namespace dRofusClientDemo;

public class RofusClientDemo
{
    public IdRofusClient CreateClient(string BaseUrl, string Database, string ProjectId)
    {
        var args = new dRofusConnectionArgs(BaseUrl, Database, ProjectId);
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

    public async Task<Project> GetProject(IdRofusClient client)
    {
         return await client.GetProjectAsync();
    }

    public async Task<List<dRofusPropertyMeta>> GetProjectsPropertyMeta(IdRofusClient client)
    {
        return await client.GetPropertyMetaAsync(dRofusType.Projects);
    }

    public async Task<Project> GetProjectWithIdAndName(IdRofusClient client)
    {
        var options = Query.Field()
            .Select(["id", "name"]);

        return await client.GetProjectAsync(options);
    }

    public async Task<string?> GetCustomPropertyForProject(IdRofusClient client, string customPropertyName)
    {
        var propertyMetadata = await client.GetPropertyMetaAsync(dRofusType.Projects);
        var customProperty = propertyMetadata.First(p => p.Name == customPropertyName);
        
        var options = Query.Field()
            .Select(customProperty.Id);

        var project = await client.GetProjectAsync(options);
        return project.GetPropertyAsString(customProperty.Id);
    }

}
