namespace dRofusClient.AttributeConfigurations;

public static class dRofusAttributeConfigurationExtensions
{
    public static Task<List<AttributeConfiguration>> GetAttributeConfigurationsAsync(this IdRofusClient client, ListQuery? options = default, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<AttributeConfiguration>(dRofusType.AttributeConfigurations.ToRequest(), options, cancellationToken);
    }

    public static Task<List<AttributeConfiguration>> GetAttributeConfigurationsAsync(this IdRofusClient client, AttributeConfigType attributeConfigType, bool? availableToUsers = null, CancellationToken cancellationToken = default)
    {
        var options = Query.List();
        var configTypeFilter = Filter.Eq(AttributeConfiguration.ConfigTypeField, attributeConfigType.ToRequest());

        if (availableToUsers.HasValue)
        {
            options = options.Filter(
                Filter.And(configTypeFilter, 
                Filter.Eq(AttributeConfiguration.AvailableToUsersField, availableToUsers.Value)));
        }
        else
        {
            options = options.Filter(configTypeFilter);
        }

        return client.GetListAsync<AttributeConfiguration>(dRofusType.AttributeConfigurations.ToRequest(), options, cancellationToken);
    }

    public static async Task<AttributeConfiguration?> GetAttributeConfigurationAsync(this IdRofusClient client, int attributeConfigurationId, CancellationToken cancellationToken = default)
    {
        var options = Query.List()
            .Filter(Filter.Eq("id", attributeConfigurationId));
        var items = await client.GetListAsync<AttributeConfiguration>(dRofusType.AttributeConfigurations.ToRequest(), options, cancellationToken).ConfigureAwait(false);
        return items.FirstOrDefault();
    }
}