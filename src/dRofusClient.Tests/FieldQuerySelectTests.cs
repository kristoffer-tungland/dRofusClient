using dRofusClient.Projects;

namespace dRofusClient.Tests;

public sealed class FieldQuerySelectTests
{
    [Fact]
    internal void Select_FromText()
    {
        var fieldQuery = Query.Field();
        fieldQuery.Select("id");
        var parameters = fieldQuery.GetParameters();
        Assert.Equal("$select=id", parameters);
    }

    [Fact]
    internal void Select_FromList()
    {
        var fieldQuery = Query.Field();
        fieldQuery.Select(["id", "name"]);
        var parameters = fieldQuery.GetParameters();
        Assert.Equal("$select=id,name", parameters);
    }

    [Fact]
    internal void Select_FromType()
    {
        var fieldQuery = Query.Field();
        fieldQuery.Select(typeof(Project));
        var parameters = fieldQuery.GetParameters();
        Assert.Equal("$select=constructor,name,planned_gross_area,project_designed_gross_area,project_gross_net_factor,room_level_gross_net_factor,id", 
            parameters);
    }
}