// ReSharper disable InconsistentNaming
namespace dRofusClient.PropertyMeta;

public static class dRofusClientPropertyMetaExtensions
{
    public static Task<List<dRofusPropertyMeta>> GetPropertyMetaAsync(this IdRofusClient client, 
        dRofusType dRofusType, 
        dRofusPropertyMetaOptions? options = default, 
        CancellationToken cancellationToken = default)
    {
        return client.OptionsListAsync<dRofusPropertyMeta>(dRofusType.ToRequest(), options, cancellationToken);
    }
}