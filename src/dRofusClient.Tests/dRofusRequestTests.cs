using dRofusClient.Occurrences;

namespace dRofusClient.Tests;

public class dRofusRequestTests
{
    [Fact]
    public void GetParametersFieldQuery_Includes_SelectFields()
    {
        var parameters = Query.Field().Select(Occurence.IdField).GetParameters();
        Assert.Equal("$select=id", parameters);
    }

    [Fact]
    public void GetParametersFieldQuery_Includes_SelectMultipleFields()
    {
        var parameters = Query.Field().Select(Occurence.IdField, Occurence.OccurrenceNameField).GetParameters();
        Assert.Equal("$select=id,occurrence_name", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_SelectFields()
    {
        var parameters = Query.List().Select(Occurence.IdField).GetParameters();
        Assert.Equal("$select=id", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_OrderBy()
    {
        var parameters = Query.List().OrderBy(Occurence.IdField).GetParameters();
        Assert.Equal("$orderby=id asc", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_MultipleOrderBy()
    {
        var parameters = Query.List().OrderBy(Occurence.OccurrenceNameField, Occurence.ArticleIdField).GetParameters();
        Assert.Equal("$orderby=occurrence_name,article_id asc", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_OrderByDescending()
    {
        var parameters = Query.List().OrderByDescending(Occurence.IdField).GetParameters();
        Assert.Equal("$orderby=id desc", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_MultipleOrderByDescending()
    {
        var parameters = Query.List().OrderByDescending("name", "architect_no").GetParameters();
        Assert.Equal("$orderby=name,architect_no desc", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_Filter()
    {
        Assert.Equal("$filter=id eq 2", Query.List().Filter(Filter.Eq("id", 2)).GetParameters());
        Assert.Equal("$filter=id ne 2", Query.List().Filter(Filter.Ne("id", 2)).GetParameters());
        Assert.Equal("$filter=id lt 2", Query.List().Filter(Filter.Lt("id", 2)).GetParameters());
        Assert.Equal("$filter=id gt 2", Query.List().Filter(Filter.Gt("id", 2)).GetParameters());
        Assert.Equal("$filter=id le 2", Query.List().Filter(Filter.Le("id", 2)).GetParameters());
        Assert.Equal("$filter=id ge 2", Query.List().Filter(Filter.Ge("id", 2)).GetParameters());
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterDate()
    {
        var parameters = Query.List().Filter(Filter.Gt("created", new DateTime(2019, 1, 1))).GetParameters();
        Assert.Equal("$filter=created gt '2019-1-1'", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterIn()
    {
        var parameters = Query.List().Filter(Filter.In("id", 1, 2, 3, 4)).GetParameters();
        Assert.Equal("$filter=id in (1,2,3,4)", parameters);
    }

    [Fact]
    public void GetParametersParamsIntQuery_Includes_FilterIn()
    {
        var parameters = Query.List().Filter(Filter.In("id", 1, 2, 3, 4)).GetParameters();
        Assert.Equal("$filter=id in (1,2,3,4)", parameters);
    }

    [Fact]
    public void GetParametersParamsStringOptions_Includes_FilterIn()
    {
        var parameters = Query.List().Filter(Filter.In("id","apple", "banana")).GetParameters();
        Assert.Equal("$filter=id in ('apple','banana')", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterInString()
    {
        var parameters = Query.List().Filter(Filter.In("name", ["kitchen", "office"])).GetParameters();
        Assert.Equal("$filter=name in ('kitchen','office')", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterContains()
    {
        var parameters = Query.List().Filter(Filter.Contains("name", "kitch")).GetParameters();
        Assert.Equal("$filter=contains(name,'kitch')", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterInMultiple()
    {
        var parameters = Query.List().Filter(
            Filter.And(
                Filter.In("id", ["1", "2", "3", "4"]),
                Filter.Lt("id", 200)
                )).GetParameters();
        Assert.Equal("$filter=id in ('1','2','3','4') and id lt 200", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterMultiple()
    {
        var parameters = Query.List()
            .Filters(
                Filter.Gt("id", 2),
                Filter.Lt("id", 200)
                ).GetParameters();
        Assert.Equal("$filter=id gt 2 and id lt 200", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterGreaterLessThan()
    {
        var parameters = Query.List().Filter(
            Filter.GtAndLt(2, "id", 200)).GetParameters();
        Assert.Equal("$filter=id gt 2 and id lt 200", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterIsEmpty()
    {
        var parameters = Query.List().Filter(
            Filter.IsEmpty("name")).GetParameters();
        Assert.Equal("$filter=name eq null", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterIsNotNull()
    {
        var parameters = Query.List().Filter(
            Filter.IsNotEmpty("name")).GetParameters();
        Assert.Equal("$filter=name ne null", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterStartsWith()
    {
        var parameters = Query.List().Filter(
            Filter.StartsWith("name", "pre")).GetParameters();
        Assert.Equal("$filter=startswith(name,'pre')", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterEndsWith()
    {
        var parameters = Query.List().Filter(
            Filter.EndsWith("name", "suffix")).GetParameters();
        Assert.Equal("$filter=endswith(name,'suffix')", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterWildcard()
    {
        var parameters = Query.List().Filter(
            Filter.Wildcard("name", "prefix*suffix")).GetParameters();
        Assert.Equal("$filter=startswith(name,'prefix') and endswith(name,'suffix')", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_FilterMultipleWildcards()
    {
        var parameters = Query.List().Filter(
            Filter.Wildcard("name", "prefix*middle*suffix")).GetParameters();
        Assert.Equal("$filter=startswith(name,'prefix') and contains(name,'middle') and endswith(name,'suffix')", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_Top()
    {
        var parameters = Query.List().Top(10000).GetParameters();
        Assert.Equal("$top=10000", parameters);
    }

    [Fact]
    public void GetParametersListQuery_Includes_Skip()
    {
        var parameters = Query.List().Skip(10000).GetParameters();
        Assert.Equal("$skip=10000", parameters);
    }

    [Fact]
    public void GetParametersListQuery()
    {
        var query = Query.List()
            .Select(Occurence.IdField)
            .OrderBy(Occurence.IdField)
            .Filters(
                Filter.Gt(Occurence.IdField, 5),
                Filter.Lt(Occurence.IdField, 200)
                )
            .Top(10000)
            .Skip(10000);

        var parameters = query.GetParameters();
        Assert.Equal("$select=id&$orderby=id asc&$filter=id gt 5 and id lt 200&$top=10000&$skip=10000", parameters);
    }

    [Fact]
    public void dRofusFilter_Bool_EqualsTrue()
    {
        var parameters = Query.List().Filter(
            Filter.Eq("available_to_users", true)).GetParameters();
        Assert.Equal("$filter=available_to_users eq true", parameters);
    }

    [Fact]
    public void dRofusFilter_Bool_EqualsFalse()
    {
        var parameters = Query.List().Filter(
            Filter.Eq("available_to_users", false)).GetParameters();
        Assert.Equal("$filter=available_to_users eq false", parameters);
    }

    [Fact]
    public void dRofusFilter_Bool_InTrue()
    {
        var parameters = Query.List().Filter(
            Filter.In("available_to_users", true)).GetParameters();
        Assert.Equal("$filter=available_to_users in (true)", parameters);
    }

    [Fact]
    public void dRofusFilter_Bool_InFalse()
    {
        var parameters = Query.List().Filter(
            Filter.In("available_to_users", false)).GetParameters();
        Assert.Equal("$filter=available_to_users in (false)", parameters);
    }

    [Fact]
    public void dRofusFilter_Bool_InTrueFalse()
    {
        var parameters = Query.List().Filter(
            Filter.In("available_to_users", true, false)).GetParameters();
        Assert.Equal("$filter=available_to_users in (true,false)", parameters);
    }

    [Fact]
    public void dRofusFilter_IsEmpty()
    {
        var parameters = Query.List().Filter(
            Filter.IsEmpty("number")).GetParameters();
        Assert.Equal("$filter=number eq null", parameters);
    }

    [Fact]
    public void dRofusFilter_IsNotEmpty()
    {
        var parameters = Query.List().Filter(
            Filter.IsNotEmpty("number")).GetParameters();
        Assert.Equal("$filter=number ne null", parameters);
    }
}