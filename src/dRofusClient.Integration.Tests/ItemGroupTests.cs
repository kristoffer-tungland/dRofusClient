using dRofusClient.ItemGroups;

namespace dRofusClient.Integration.Tests;

public class ItemGroupTests(ItemGroupFixture fixture) : IClassFixture<ItemGroupFixture>
{
    private readonly IdRofusClient _client = fixture.Client;

    [Fact]
    public async Task CanGetItemGroups()
    {
        var itemGroups = await _client.GetItemGroupsAsync(Query.List());
        Assert.NotEmpty(itemGroups);
        Assert.Contains(itemGroups, group => group.Id == fixture.ItemGroup.Id);
    }

    [Fact]
    public async Task CanGetItemGroup()
    {
        var itemGroup = await _client.GetItemGroupAsync(fixture.ItemGroup.GetId());

        Assert.NotNull(itemGroup);
        Assert.Equal(fixture.ItemGroup.Id, itemGroup.Id);
        Assert.Equal(fixture.ItemGroup.Number, itemGroup.Number);
        Assert.Equal(fixture.ItemGroup.Name, itemGroup.Name);
    }

    [Fact]
    public async Task CanCreateItemGroup()
    {
        var createdItemGroup = new CreateItemGroup
        {
            Number = "TestGroup" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            Name = "Test Item Group " + DateTime.Now.ToString("yyyyMMddHHmmss"),
            Parent = fixture.ItemGroup.Id,
        };

        var itemGroup = await _client.CreateItemGroupAsync(createdItemGroup);

        Assert.NotNull(itemGroup);
        Assert.NotEqual(0, itemGroup.Id);
        Assert.Equal(createdItemGroup.Number, itemGroup.Number);
        Assert.Equal(createdItemGroup.Name, itemGroup.Name);
    }

    [Fact]
    public async Task CanEditItemGroup()
    {
        var createdItemGroup = new CreateItemGroup
        {
            Number = "TestEditGroup" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            Name = "Test Item Group " + DateTime.Now.ToString("yyyyMMddHHmmss"),
            Parent = fixture.ItemGroup.Id,
        };

        var itemGroup = await _client.CreateItemGroupAsync(createdItemGroup);

        itemGroup.Name = "Updated Item Group Name " + DateTime.Now.ToString("yyyyMMddHHmmss");

        var updatedResult = await _client.UpdateItemGroupAsync(itemGroup);
        Assert.NotNull(updatedResult);
        Assert.Equal(itemGroup.Name, updatedResult.Name);
    }

    [Fact]
    public async Task GetItemGroups_WhenSelectingOnlySomeFields_ReturnsExpectedFields()
    {
        var query = Query.List()
            .Select(ItemGroup.IdField, ItemGroup.NumberField, ItemGroup.NameField)
            .Filter(Filter.Eq(ItemGroup.IdField, fixture.ItemGroup.GetId()));
        var itemGroups = await _client.GetItemGroupsAsync(query);
        Assert.Single(itemGroups);
        var itemGroup = itemGroups[0];
        Assert.NotNull(itemGroup);
        Assert.Equal(fixture.ItemGroup.Id, itemGroup.Id);
        Assert.Equal(fixture.ItemGroup.Number, itemGroup.Number);
        Assert.Equal(fixture.ItemGroup.Name, itemGroup.Name);
    }

    [Fact]
    public async Task GetItemGroup_WhenSelectingOnlySomeFields_ReturnsExpectedFields()
    {
        var query = Query.Field()
            .Select(ItemGroup.IdField, ItemGroup.NumberField, ItemGroup.NameField);

        var itemGroup = await _client.GetItemGroupAsync(fixture.ItemGroup.GetId());

        Assert.NotNull(itemGroup);
        Assert.Equal(fixture.ItemGroup.Id, itemGroup.Id);
        Assert.Equal(fixture.ItemGroup.Number, itemGroup.Number);
        Assert.Equal(fixture.ItemGroup.Name, itemGroup.Name);
        Assert.Null(itemGroup.Description);
    }

}
