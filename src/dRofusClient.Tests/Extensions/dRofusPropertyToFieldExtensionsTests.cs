using dRofusClient.Extensions;
using dRofusClient.Occurrences;

namespace dRofusClient.Tests.Extensions;

public sealed class dRofusPropertyToFieldExtensionsTests
{
    [Fact]
    internal void ToLowerUnderscore()
    {
        Assert.Equal("id", nameof(dRofusOccurence.Id).ToLowerUnderscore());
    }

    [Fact]
    internal void ToLowerUnderscore_ConvertsCamelCase_ToUnderscore()
    {
        Assert.Equal("addition_order_quantity", nameof(dRofusOccurence.AdditionOrderQuantity).ToLowerUnderscore());
    }

    [Fact]
    internal void ToLowerUnderscore_DoesNotConvertNotCamelCase_ToUnderscore()
    {
        Assert.Equal("not_camel_case", "not_camel_case".ToLowerUnderscore());
    }
}