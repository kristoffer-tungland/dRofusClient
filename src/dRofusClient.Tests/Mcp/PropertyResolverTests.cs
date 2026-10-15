using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using dRofusClient.Items;
using dRofusClient.Mcp;
using dRofusClient.Occurrences;
using dRofusClient.PropertyMeta;
using dRofusClient.Rooms;
using dRofusClient.Systems;

namespace dRofusClient.Tests.Mcp;

public sealed class PropertyResolverTests
{
    private readonly PropertyResolver _resolver = new(new FieldCatalog());

    [Theory]
    [InlineData("Item", typeof(Item))]
    [InlineData("Occurrence", typeof(Occurence))]
    [InlineData("Room", typeof(Room))]
    [InlineData("System", typeof(SystemInstance))]
    public void EveryModelPropertyResolvesByItsVerifiedApiName(string schema, Type model)
    {
        foreach (var property in model.GetProperties())
        {
            if (property.GetCustomAttribute<JsonPropertyNameAttribute>() is not { } jsonName)
                continue;
            var result = _resolver.Resolve(schema, [], jsonName.Name, 25, 0);
            Assert.Equal("resolved", result.Status);
            Assert.Equal("api_name", result.Stage);
            Assert.Equal(jsonName.Name, result.ResolvedId);
            Assert.True(Assert.Single(result.Candidates).BuiltIn);
        }
    }

    [Theory]
    [InlineData("Room", "RoomNumber", "architect_no")]
    [InlineData("Room", "Room Number", "architect_no")]
    [InlineData("Room", "General: Room Number", "architect_no")]
    [InlineData("System", "SystemComponentId", "base_occurrence_id")]
    [InlineData("System", "Serial Number", "run_no")]
    [InlineData("Occurrence", "Item ID", "article_id")]
    [InlineData("Item", "Budget price", "price")]
    public void BuiltInLabelsResolveFromActualModelOrSchema(string schema, string label, string id)
    {
        var result = _resolver.Resolve(schema, [], label, 25, 0);
        Assert.Equal("resolved", result.Status);
        Assert.Equal(id, result.ResolvedId);
        Assert.True(Assert.Single(result.Candidates).BuiltIn);
        Assert.Contains(result.Candidates[0].Aliases, a => a.Source.StartsWith("model:") || a.Source.StartsWith("OpenAPI"));
    }

    [Fact]
    public void GeneratedXmlDocumentationIsLoadedWithProvenance()
    {
        Assert.True(File.Exists(Path.ChangeExtension(typeof(Room).Assembly.Location, ".xml")));
        var result = _resolver.Resolve("Room", [], "architect_no", 25, 0);
        Assert.Contains(Assert.Single(result.Candidates).Aliases,
            a => a.Source == "XML:dRofusClient.Rooms.Room.RoomNumber" && a.Name == "General: Room Number");
    }

    [Fact]
    public void XmlOnlyGroupedLabelIsAVerifiedSynonymBeforeCustomLookup()
    {
        var result = _resolver.Resolve("Item", Metadata("""
            [{"id":"custom_responsibility","name":"General: Responsibility","dataType":"string"}]
            """), "General: Responsibility", 25, 0);
        Assert.Equal("responsibility", result.ResolvedId);
        Assert.Equal("built_in_synonym", result.Stage);
        Assert.Contains(Assert.Single(result.Candidates).Aliases,
            a => a.Source == "XML:dRofusClient.Items.Item.Responsibility" && a.IsSynonym);
    }

    [Fact]
    public void DocumentedSynonymConflictingWithDisplayAliasIsNotSilentlyDiscarded()
    {
        var result = _resolver.Resolve("Item", Metadata("""
            [{"id":"name","name":"General: Responsibility","dataType":"string"}]
            """), "General: Responsibility", 25, 0);
        Assert.Equal("ambiguous", result.Status);
        Assert.Null(result.ResolvedId);
        Assert.Equal(2, result.MatchCount);
    }

    [Fact]
    public void BuiltInDisplayNameWinsOverCustomDisplayName()
    {
        var result = _resolver.Resolve("Room", Metadata("""
            [{"id":"custom_room","name":"Room Number","dataType":"string"}]
            """), "Room Number", 25, 0);
        Assert.Equal("architect_no", result.ResolvedId);
        Assert.Equal("built_in_alias", result.Stage);
    }

    [Fact]
    public void ExactCustomApiNamePrecedesBuiltInAlias()
    {
        var result = _resolver.Resolve("Room", Metadata("""
            [{"id":"roomnumber","name":"Custom number","dataType":"string"}]
            """), "roomnumber", 25, 0);
        Assert.Equal("roomnumber", result.ResolvedId);
        Assert.Equal("api_name", result.Stage);
        Assert.False(Assert.Single(result.Candidates).BuiltIn);
    }

    [Fact]
    public void LiveBuiltInAliasDoesNotReplaceBundledAliasesOrWeakenReadOnly()
    {
        var metadata = Metadata("""
            [{"id":"architect_no","name":"Local room label","propertyGroup":"Project","dataType":"string"},
             {"id":"id","name":"Local ID","dataType":"integer","readOnly":false}]
            """);
        Assert.Equal("architect_no", _resolver.Resolve("Room", metadata, "Project: Local room label", 25, 0).ResolvedId);
        Assert.Equal("architect_no", _resolver.Resolve("Room", metadata, "Room Number", 25, 0).ResolvedId);
        Assert.True(Assert.Single(_resolver.Resolve("Room", metadata, "Local ID", 25, 0).Candidates).Field.ReadOnly);
    }

    [Fact]
    public void DuplicateBuiltInDisplayNamesAreAmbiguousBeforeCustomOrFuzzy()
    {
        // Both fields have "Serial Number" as a title in the actual Item OpenAPI schema.
        var result = _resolver.Resolve("Item", Metadata("""
            [{"id":"custom_serial","name":"Serial Number","dataType":"string"}]
            """), "Serial Number", 1, 0);
        Assert.Equal("ambiguous", result.Status);
        Assert.Null(result.ResolvedId);
        Assert.Equal(2, result.MatchCount);
        Assert.Single(result.Candidates);
        Assert.Equal("run_no", result.Candidates[0].Field.Id);
        var next = _resolver.Resolve("Item", [], "Serial Number", 1, 1);
        Assert.Equal("ambiguous", next.Status);
        Assert.Equal("serial_no", Assert.Single(next.Candidates).Field.Id);
    }

    [Fact]
    public void GroupedBuiltInLabelDisambiguates()
    {
        Assert.Equal("run_no", _resolver.Resolve("Item", [], "Classification: Serial Number", 25, 0).ResolvedId);
        Assert.Equal("serial_no", _resolver.Resolve("Item", [], "General: Serial Number", 25, 0).ResolvedId);
    }

    [Fact]
    public void MetadataConflictsWithModelAliasRequireClarification()
    {
        var result = _resolver.Resolve("Room", Metadata("""
            [{"id":"drawing_no","name":"Room Number","dataType":"string"}]
            """), "Room Number", 25, 0);
        Assert.Equal("ambiguous", result.Status);
        Assert.Null(result.ResolvedId);
        Assert.Equal(2, result.MatchCount);
    }

    [Fact]
    public void CustomExactLookupPrecedesFuzzyBuiltInAndPreservesAmbiguity()
    {
        var metadata = Metadata("""
            [{"id":"custom_one","name":"Room Numbre","propertyGroup":"A","dataType":"string"},
             {"id":"custom_two","name":"Room Numbre","propertyGroup":"B","dataType":"string"}]
            """);
        var result = _resolver.Resolve("Room", metadata, "Room Numbre", 25, 0);
        Assert.Equal("custom", result.Stage);
        Assert.Equal("ambiguous", result.Status);
        Assert.Null(result.ResolvedId);
        Assert.Equal("custom_two", _resolver.Resolve("Room", metadata, "B: Room Numbre", 25, 0).ResolvedId);
    }

    [Fact]
    public void FuzzyMatchNeverConfirmsEvenSingleCandidate()
    {
        var result = _resolver.Resolve("Room", [], "architect_n", 25, 0);
        Assert.Equal("suggestions", result.Status);
        Assert.Equal("fuzzy", result.Stage);
        Assert.Null(result.ResolvedId);
        Assert.Equal("architect_no", Assert.Single(result.Candidates).Field.Id);
    }

    [Fact]
    public void EndpointAliasesDoNotLeakAndNoUndocumentedSynonymsAreInvented()
    {
        Assert.Equal("not_found", _resolver.Resolve("Item", [], "architect_no", 25, 0).Status);
        Assert.Equal("not_found", _resolver.Resolve("System", [], "Room Number", 25, 0).Status);
        Assert.Equal("not_found", _resolver.Resolve("Item", [], "purchase expenditure", 25, 0).Status);
    }

    private static List<dRofusPropertyMeta> Metadata(string json) =>
        JsonSerializer.Deserialize<List<dRofusPropertyMeta>>(json)!;
}
