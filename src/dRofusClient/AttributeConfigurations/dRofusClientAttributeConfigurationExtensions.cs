using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace dRofusClient.AttributeConfigurations;

/// <summary>
/// Extension methods for working with Attribute Configurations in dRofus API
/// </summary>
public static class dRofusClientAttributeConfigurationExtensions
{
    /// <summary>
    /// Get list of Attribute Configurations with the specified options
    /// </summary>
    public static Task<List<dRofusAttributeConfiguration>> GetAttributeConfigurationsAsync(this IdRofusClient client, 
        dRofusListOptions options, 
        CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusAttributeConfiguration>(dRofusType.AttributeConfigurations.ToRequest(), options, cancellationToken);
    }
}