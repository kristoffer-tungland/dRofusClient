using dRofusClient.Occurrences;

namespace dRofusClient.Tests;

public class dRofusOptionsTests
{
    [Fact]
    public void GetParametersFieldOptions_Includes_SelectFields()
    {
        var parameters = dRofusOptions.Field().Select(nameof(dRofusOccurence.Id)).GetParameters();
        Assert.Equal("$select=id", parameters);
    }

    [Fact]
    public void GetParametersFieldOptions_Includes_SelectMultipleFields()
    {
        var parameters = dRofusOptions.Field().Select(["id", "name"]).GetParameters();
        Assert.Equal("$select=id,name", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_SelectFields()
    {
        var parameters = dRofusOptions.List().Select(nameof(dRofusOccurence.Id)).GetParameters();
        Assert.Equal("$select=id", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_OrderBy()
    {
        var parameters = dRofusOptions.List().OrderBy(nameof(dRofusOccurence.Id)).GetParameters();
        Assert.Equal("$orderby=id asc", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_MultipleOrderBy()
    {
        var parameters = dRofusOptions.List().OrderBy(["name", "architect_no"]).GetParameters();
        Assert.Equal("$orderby=name,architect_no asc", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_OrderByDescending()
    {
        var parameters = dRofusOptions.List().OrderByDescending(nameof(dRofusOccurence.Id)).GetParameters();
        Assert.Equal("$orderby=id desc", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_MultipleOrderByDescending()
    {
        var parameters = dRofusOptions.List().OrderByDescending(["name", "architect_no"]).GetParameters();
        Assert.Equal("$orderby=name,architect_no desc", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_Filter()
    {
        Assert.Equal("$filter=id eq 2", dRofusOptions.List().Filter(dRofusFilter.Eq("id", 2)).GetParameters());
        Assert.Equal("$filter=id ne 2", dRofusOptions.List().Filter(dRofusFilter.Ne("id", 2)).GetParameters());
        Assert.Equal("$filter=id lt 2", dRofusOptions.List().Filter(dRofusFilter.Lt("id", 2)).GetParameters());
        Assert.Equal("$filter=id gt 2", dRofusOptions.List().Filter(dRofusFilter.Gt("id", 2)).GetParameters());
        Assert.Equal("$filter=id le 2", dRofusOptions.List().Filter(dRofusFilter.Le("id", 2)).GetParameters());
        Assert.Equal("$filter=id ge 2", dRofusOptions.List().Filter(dRofusFilter.Ge("id", 2)).GetParameters());
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterDate()
    {
        var parameters = dRofusOptions.List().Filter(dRofusFilter.Gt("created", new DateTime(2019, 1, 1))).GetParameters();
        Assert.Equal("$filter=created gt '2019-1-1'", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterIn()
    {
        var parameters = dRofusOptions.List().Filter(dRofusFilter.In("id", [1, 2, 3, 4])).GetParameters();
        Assert.Equal("$filter=id in (1,2,3,4)", parameters);
    }

    [Fact]
    public void GetParametersParamsIntOptions_Includes_FilterIn()
    {
        var parameters = dRofusOptions.List().Filter(dRofusFilter.In("id", 1, 2, 3, 4)).GetParameters();
        Assert.Equal("$filter=id in (1,2,3,4)", parameters);
    }

    [Fact]
    public void GetParametersParamsStringOptions_Includes_FilterIn()
    {
        var parameters = dRofusOptions.List().Filter(dRofusFilter.In("id","apple", "banana")).GetParameters();
        Assert.Equal("$filter=id in ('apple','banana')", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterInString()
    {
        var parameters = dRofusOptions.List().Filter(dRofusFilter.In("name", ["kitchen", "office"])).GetParameters();
        Assert.Equal("$filter=name in ('kitchen','office')", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterContains()
    {
        var parameters = dRofusOptions.List().Filter(dRofusFilter.Contains("name", "kitch")).GetParameters();
        Assert.Equal("$filter=contains(name,'kitch')", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterInMultiple()
    {
        var parameters = dRofusOptions.List().Filter(
            dRofusFilter.And(
                dRofusFilter.In("id", ["1", "2", "3", "4"]),
                dRofusFilter.Lt("id", 200)
                )).GetParameters();
        Assert.Equal("$filter=id in ('1','2','3','4') and id lt 200", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterMultiple()
    {
        var parameters = dRofusOptions.List().Filter(
            dRofusFilter.And(
                dRofusFilter.Gt("id", 2),
                dRofusFilter.Lt("id", 200)
                )).GetParameters();
        Assert.Equal("$filter=id gt 2 and id lt 200", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterGreaterLessThan()
    {
        var parameters = dRofusOptions.List().Filter(
            dRofusFilter.GtAndLt(2, "id", 200)).GetParameters();
        Assert.Equal("$filter=id gt 2 and id lt 200", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterIsEmpty()
    {
        var parameters = dRofusOptions.List().Filter(
            dRofusFilter.IsEmpty("name")).GetParameters();
        Assert.Equal("$filter=name eq null", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterIsNotNull()
    {
        var parameters = dRofusOptions.List().Filter(
            dRofusFilter.IsNotEmpty("name")).GetParameters();
        Assert.Equal("$filter=name ne null", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterStartsWith()
    {
        var parameters = dRofusOptions.List().Filter(
            dRofusFilter.StartsWith("name", "pre")).GetParameters();
        Assert.Equal("$filter=startswith(name,'pre')", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterEndsWith()
    {
        var parameters = dRofusOptions.List().Filter(
            dRofusFilter.EndsWith("name", "suffix")).GetParameters();
        Assert.Equal("$filter=endswith(name,'suffix')", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterWildcard()
    {
        var parameters = dRofusOptions.List().Filter(
            dRofusFilter.Wildcard("name", "prefix*suffix")).GetParameters();
        Assert.Equal("$filter=startswith(name,'prefix') and endswith(name,'suffix')", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_FilterMultipleWildcards()
    {
        var parameters = dRofusOptions.List().Filter(
            dRofusFilter.Wildcard("name", "prefix*middle*suffix")).GetParameters();
        Assert.Equal("$filter=startswith(name,'prefix') and contains(name,'middle') and endswith(name,'suffix')", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_Top()
    {
        var parameters = dRofusOptions.List().Top(10000).GetParameters();
        Assert.Equal("$top=10000", parameters);
    }

    [Fact]
    public void GetParametersListOptions_Includes_Skip()
    {
        var parameters = dRofusOptions.List().Skip(10000).GetParameters();
        Assert.Equal("$skip=10000", parameters);
    }

    [Fact]
    public void GetParametersListOptions()
    {
        // ReSharper disable once SuggestVarOrType_SimpleTypes
        var options = dRofusOptions.List()
            .Select(nameof(dRofusOccurence.Id))
            .OrderBy(nameof(dRofusOccurence.Id))
            .Filter(
                dRofusFilter.And(
                    dRofusFilter.Gt("id", 5),
                    dRofusFilter.Lt("id", 200)
                ))
            .Top(10000)
            .Skip(10000);

        var parameters = options.GetParameters();
        Assert.Equal("$select=id&$orderby=id asc&$filter=id gt 5 and id lt 200&$top=10000&$skip=10000", parameters);
    }
}