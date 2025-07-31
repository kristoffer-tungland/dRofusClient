using dRofusClient.Rooms;

namespace dRofusClient.Integration.Tests;

public class RoomTests(RoomFixture fixture) : IClassFixture<RoomFixture>
{
    private readonly IdRofusClient _client = fixture.Client;
    
    [Fact]
    public async Task CanGetRooms()
    {
        var rooms = await _client.GetRoomsAsync(Query.List());
        Assert.NotEmpty(rooms);
        Assert.Contains(rooms, room => room.Id == fixture.Room.Id);
    }
    
    [Fact]
    public async Task CanGetRoom()
    {
        var room = await _client.GetRoomAsync(fixture.Room.GetId());
        Assert.NotNull(room);
        Assert.Equal(fixture.Room.Id, room.Id);
        Assert.Equal(fixture.Room.Name, room.Name);
    }

    [Fact]
    public async Task CanCreateRoom()
    {
        var createdRoom = new CreateRoom
        {
            Name = "Test Room " + DateTime.Now.ToString("yyyyMMddHHmmss")
        };
        
        var room = await _client.CreateRoomAsync(createdRoom);
        
        try
        {
            Assert.NotNull(room);
            Assert.NotEqual(0, room.Id);
            Assert.Equal(createdRoom.Name, room.Name);
        }
        finally
        {
            await Assert.ThrowsAsync<NotSupportedException>(async () =>
            {
                // Attempting to delete a room should throw an exception as deletion is not implemented.
                await _client.DeleteRoomAsync(room.GetId());
            });
        }
    }

    [Fact]
    public async Task CanEditRoom()
    {
        var createdRoom = new CreateRoom
        {
            Name = "Test Edit Room " + DateTime.Now.ToString("yyyyMMddHHmmss")
        };
        
        var room = await _client.CreateRoomAsync(createdRoom);
        
        try
        {
            room.Name = "Updated Room Name " + DateTime.Now.ToString("yyyyMMddHHmmss");
            var updatedResult = await _client.UpdateRoomAsync(room);
            Assert.NotNull(updatedResult);
            Assert.Equal(room.Name, updatedResult.Name);
        }
        finally
        {
            await Assert.ThrowsAsync<NotSupportedException>(async () =>
            {
                // Attempting to delete a room should throw an exception as deletion is not implemented.
                await _client.DeleteRoomAsync(room.GetId());
            });
        }
    }
}
