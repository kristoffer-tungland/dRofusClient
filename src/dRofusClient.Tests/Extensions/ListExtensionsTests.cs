using dRofusClient.Extensions;

namespace dRofusClient.Tests.Extensions;

public class ListExtensionsTests
{
    [Fact]
    public void ToCommaSeparated_ShouldReturnCommaSeparatedString()
    {
        // Arrange
        List<string> list = ["apple", "banana", "cherry"];

        // Act
        var result = list.ToCommaSeparated();

        // Assert
        Assert.Equal("apple,banana,cherry", result);
    }

    [Fact]
    public void ToCommaSeparated_ShouldReturnEmptyString_WhenListIsEmpty()
    {
        // Arrange
        List<string> list = [];

        // Act
        var result = list.ToCommaSeparated();

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void AddIfNotNull_ShouldAddItem_WhenItemIsNotNull()
    {
        // Arrange
        List<int?> list = [];
        int? item = 10;

        // Act
        list.AddIfNotNull(item);

        // Assert
        Assert.Single(list);
        Assert.Equal(10, list[0]);
    }

    [Fact]
    public void AddIfNotNull_ShouldNotAddItem_WhenItemIsNull()
    {
        // Arrange
        List<int?> list = [];
        int? item = null;

        // Act
        list.AddIfNotNull(item);

        // Assert
        Assert.Empty(list);
    }
}