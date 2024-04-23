using dRofusClient.Projects;

namespace dRofusClient.Tests;

public sealed class dRofusFieldsOptionsExtensionsTests
{
    [Fact]
    internal void Select_FromText()
    {
        var options = dRofusOptions.Field();
        options.Select("id");
        var parameters = options.GetParameters();
        Assert.Equal("$select=id", parameters);
    }

    [Fact]
    internal void Select_FromList()
    {
        var options = dRofusOptions.Field();
        options.Select(["id", "name"]);
        var parameters = options.GetParameters();
        Assert.Equal("$select=id,name", parameters);
    }

    [Fact]
    internal void Select_FromType()
    {
        var options = dRofusOptions.Field();
        options.Select(typeof(dRofusProject));
        var parameters = options.GetParameters();
        Assert.Equal("$select=id,constructor,name,planned_gross_area,project_designed_gross_area,project_gross_net_factor,room_level_gross_net_factor", 
            parameters);
    }
}