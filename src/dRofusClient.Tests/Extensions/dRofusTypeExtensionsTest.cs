using dRofusClient.Extensions;

namespace dRofusClient.Tests.Extensions;

public class dRofusTypeExtensionsTest
{
    [Fact]
    public void ToRequest_ShouldReturnLowercaseString()
    {
        // Arrange
        var type = dRofusType.Occurrences;

        // Act
        var result = type.ToRequest();

        // Assert
        Assert.Equal("occurrences", result);
    }

    [Fact]
    public void CombineToRequest_ShouldCombineTypeAndParts()
    {
        // Arrange
        var type = dRofusType.Occurrences;
        var parts = new List<string> { "part1", "part2", "part3" };

        // Act
        var result = type.CombineToRequest(parts);

        // Assert
        Assert.Equal("occurrences/part1/part2/part3", result);
    }

    [Fact]
    public void CombineToRequest_ShouldCombineTypeAndPart2()
    {
        // Arrange
        var type = dRofusType.Occurrences;
        var part2 = "part2";

        // Act
        var result = type.CombineToRequest(part2);

        // Assert
        Assert.Equal("occurrences/part2", result);
    }

    [Fact]
    public void CombineToRequest_ShouldCombineTypeAndPart2AndPart3()
    {
        // Arrange
        var type = dRofusType.Occurrences;
        var part2 = "part2";
        var part3 = "part3";

        // Act
        var result = type.CombineToRequest(part2, part3);

        // Assert
        Assert.Equal("occurrences/part2/part3", result);
    }

    [Fact]
    public void CombineToRequest_ShouldCombineTypeAndPart2AndPart3AndPart4()
    {
        // Arrange
        var type = dRofusType.Occurrences;
        var part2 = "part2";
        var part3 = "part3";
        var part4 = "part4";

        // Act
        var result = type.CombineToRequest(part2, part3, part4);

        // Assert
        Assert.Equal("occurrences/part2/part3/part4", result);
    }

    [Fact]
    public void CombineToRequest_ShouldCombineTypeAndId()
    {
        // Arrange
        var type = dRofusType.Occurrences;
        var id = 123;

        // Act
        var result = type.CombineToRequest(id);

        // Assert
        Assert.Equal("occurrences/123", result);
    }

    [Fact]
    public void CombineToRequest_ShouldCombineTypeAndIdAndPart3()
    {
        // Arrange
        var type = dRofusType.Occurrences;
        var id = 123;
        var part3 = "part3";

        // Act
        var result = type.CombineToRequest(id, part3);

        // Assert
        Assert.Equal("occurrences/123/part3", result);
    }

    [Fact]
    public void CombineToRequest_ShouldCombineTypeAndIdAndPart3AndPart4()
    {
        // Arrange
        var type = dRofusType.Occurrences;
        var id = 123;
        var part3 = "part3";
        var part4 = "part4";

        // Act
        var result = type.CombineToRequest(id, part3, part4);

        // Assert
        Assert.Equal("occurrences/123/part3/part4", result);
    }
}
