using System.Text.Json;
using dRofusClient.Extensions;
using dRofusClient.Options;

namespace dRofusClient.Tests.Extensions;

public class JsonPropertyStatusExtensionsTests
{
    [Theory]
    [InlineData("occurrence_classification_156_classification_entry_id_code", "\"CODE123\"", 156, "CODE123", null)]
    [InlineData("occurrence_classification_156_classification_entry_id_id", "42", 156, null, 42)]
    [InlineData("ce156_id_or_parents", "\"SOME_CODE\"", 156, "SOME_CODE", null)]
    public void GetStatusTypeId_And_GetStatusPatchBody_Works_As_Expected(
        string propertyName, string jsonValue, int expectedStatusTypeId, string expectedCode, int? expectedStatusId)
    {
        // Arrange
        using var doc = JsonDocument.Parse($"{{ \"{propertyName}\": {jsonValue} }}");
        var property = doc.RootElement.EnumerateObject().First();

        // Act
        int statusTypeId = dRofusStatusPatchOptions.GetStatusTypeId(propertyName);
        var body = property.GetStatusPatchBody();

        // Assert
        Assert.Equal(expectedStatusTypeId, statusTypeId);
        if (expectedCode != null)
        {
            Assert.NotNull(body);
            Assert.Equal(expectedCode, body!.Code);
            Assert.Null(body.StatusId);
        }
        else if (expectedStatusId.HasValue)
        {
            Assert.NotNull(body);
            Assert.Equal(expectedStatusId, body!.StatusId);
            Assert.Null(body.Code);
        }
        else
        {
            Assert.Null(body);
        }

        // Additional: dRofusStatusPatchOptions compatibility check
        var options = new dRofusStatusPatchOptions
        {
            PropertyName = propertyName,
            Body = body!
        };
        Assert.Equal(expectedStatusTypeId, options.StatusTypeId);
        Assert.Equal(propertyName, options.PropertyName);
    }

    [Fact]
    public void ToStatusPatchOption_Throws_On_Invalid_Property()
    {
        // Use a property/value that will cause GetStatusPatchBody to return null (e.g. boolean value)
        using var doc = JsonDocument.Parse("{\"invalid_property\": true}");
        var property = doc.RootElement.EnumerateObject().First();

        Assert.Throws<ArgumentException>(() => property.ToStatusPatchOption());
    }
}