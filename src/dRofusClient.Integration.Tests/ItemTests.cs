using dRofusClient.Files;
using dRofusClient.Items;

namespace dRofusClient.Integration.Tests;

public class ItemTests(ItemFixture fixture) : IClassFixture<ItemFixture>
{
    private readonly IdRofusClient _client = fixture.Client;
    
    [Fact]
    public async Task CanGetItems()
    {
        var items = await _client.GetItemsAsync(Query.List().Filter(Filter.Eq(Item.IdField, fixture.Item.GetId())));
        Assert.NotEmpty(items);
        Assert.Contains(items, item => item.Id == fixture.Item.Id);
    }

    [Fact]
    public async Task CanGetItem()
    {
        var item = await _client.GetItemAsync(fixture.Item.GetId());
        Assert.NotNull(item);
        Assert.Equal(fixture.Item.Id, item.Id);
        Assert.Equal(fixture.Item.Name, item.Name);
        Assert.Equal(fixture.Item.ClassificationNumber, item.ClassificationNumber);
    }

    [Fact]
    public async Task CanCreateItem()
    {
        var createdItem = CreateItem.With(fixture.ItemGroup, "Test Item " + DateTime.Now.ToString("yyyyMMddHHmmss"));
        var item = await _client.CreateItemAsync(createdItem);

        try
        {
            Assert.NotNull(item);
            Assert.NotEqual(0, item.Id);
            Assert.Equal(createdItem.Name, item.Name);
            Assert.Equal(createdItem.LevelId, item.LevelId);
        }
        finally
        {
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                // Attempting to update an item with an invalid ID should throw an exception
                await _client.DeleteItemAsync(item.GetId());
            });
        }
    }

    [Fact]
    public async Task CanEditItem()
    {
        var createdItem = CreateItem.With(fixture.ItemGroup, "Test Edit Item " + DateTime.Now.ToString("yyyyMMddHHmmss"));
        var item = await _client.CreateItemAsync(createdItem);

        try
        {
            item.Name = "Updated Item Name " + DateTime.Now.ToString("yyyyMMddHHmmss");
            var updatedResult = await _client.UpdateItemAsync(item);
            Assert.NotNull(updatedResult);
            Assert.Equal(item.Name, updatedResult.Name);
        }
        finally
        {
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                // Attempting to update an item with an invalid ID should throw an exception
                await _client.DeleteItemAsync(item.GetId());
            });
        }
    }

    [Fact]
    public async Task CanDeleteItem()
    {
        var createdItem = CreateItem.With(fixture.ItemGroup, "Test Delete Item " + DateTime.Now.ToString("yyyyMMddHHmmss"));
        var item = await _client.CreateItemAsync(createdItem);
        try
        {
            Assert.NotNull(item);
            Assert.NotEqual(0, item.Id);
            // Delete the item
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                // Attempting to update an item with an invalid ID should throw an exception
                await _client.DeleteItemAsync(item.GetId());
            });
        }
        finally
        {
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                // Attempting to update an item with an invalid ID should throw an exception
                await _client.DeleteItemAsync(item.GetId());
            });
        }
    }

    [Fact]
    public async Task AddFileToItem()
    {
        var createdItem = CreateItem.With(fixture.ItemGroup, "Test Item With File " + DateTime.Now.ToString("yyyyMMddHHmmss"));
        var item = await _client.CreateItemAsync(createdItem);
        try
        {
            Assert.NotNull(item);
            Assert.NotEqual(0, item.Id);
            // Add a file to the item
            var fileContent = new byte[] { 1, 2, 3, 4 }; // Example file content
            var fileName = "TestFile.txt";
            var fileDescription = "This is a test file for the item.";

            var fileStream = new MemoryStream(fileContent);

            var uploadedFile = await _client.UploadItemFileAsync(item.GetId(), fileStream, fileName, fileDescription);
            Assert.NotNull(uploadedFile);

            var fileQuery = Query.List()
                .Filter(Filter.Eq(FileDetails.IdField, uploadedFile.FileId));

            var files = await _client.GetItemFilesAsync(item.GetId(), fileQuery);

            Assert.NotEmpty(files);
            Assert.Single(files);

            var file = files[0];
            Assert.Equal(fileName, file.Name);
            Assert.Equal(fileDescription, file.Description);
            Assert.Equal(fileContent.Length, file.Size);
        }
        finally
        {
            await Assert.ThrowsAsync<NotImplementedException>(async () =>
            {
                // Attempting to update an item with an invalid ID should throw an exception
                await _client.DeleteItemAsync(item.GetId());
            });
        }
    }
}
