namespace dRofusClient.AttributeConfigurations;

public static class dRofusAttributeConfigurationExtensions
{
    public static Task<List<dRofusAttributeConfiguration>> GetAttributeConfigurationsAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusAttributeConfiguration>(dRofusType.AttributeConfigurations.ToRequest(), options, cancellationToken);
    }
}