using dRofusClient.Extensions;
using dRofusClient.Occurrences;

namespace dRofusClient.Tests.Extensions;

public sealed class dRofusPropertyToFieldExtensionsTests
{
    [Fact]
    internal void ToLowerUnderscore()
    {
        Assert.Equal("id", nameof(Occurence.Id).ToSnakeCase());
    }

    [Fact]
    internal void ToLowerUnderscore_ConvertsCamelCase_ToUnderscore()
    {
        Assert.Equal("addition_order_quantity", nameof(Occurence.AdditionOrderQuantity).ToSnakeCase());
    }

    [Fact]
    internal void ToLowerUnderscore_DoesNotConvertNotCamelCase_ToUnderscore()
    {
        Assert.Equal("not_camel_case", "not_camel_case".ToSnakeCase());
    }
}