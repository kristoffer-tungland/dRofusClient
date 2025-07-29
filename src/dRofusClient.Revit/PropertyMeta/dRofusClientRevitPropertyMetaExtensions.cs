using dRofusClient.PropertyMeta;
using dRofusClient.Options;
using dRofusClient.Revit.Utils;

namespace dRofusClient.Revit.PropertyMeta
{
    public static class dRofusClientRevitPropertyMetaExtensions
    {
        public static List<dRofusPropertyMeta> GetPropertyMeta(this IdRofusClient client,
            dRofusType dRofusType,
            MetadataQuery? options = default,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetPropertyMetaAsync(dRofusType, options, cancellationToken)
            );
        }
    }
}