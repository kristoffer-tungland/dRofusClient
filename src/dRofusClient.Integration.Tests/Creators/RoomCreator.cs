using dRofusClient.Rooms;

namespace dRofusClient.Integration.Tests.Creators;

internal class RoomCreator(IdRofusClient client) : IAsyncDisposable
{
    public string RoomName => "Test Room";
    public Room? Room { get; private set; }
    public async Task InitializeAsync()
    {
        // Since we cannot delete rooms in dRofus, we can try to get an existing one first.
        var existingRooms = await client.GetRoomsAsync(Query.List().Filter(Filter.Eq(Room.NameField, RoomName)));
        if (existingRooms.Count > 0)
        {
            Room = existingRooms[0];
            return;
        }

        var createRoom = new CreateRoom
        {
            Name = RoomName
        };

        Room = await client.CreateRoomAsync(createRoom);

        Room.RoomNumber = "Room Number";
        Room.Description = "Test Room Description";
        Room.Note = "Test Room Notes";
        Room.UserRoomNumber = "User Room Number";
        Room.NameOnDrawing = "Name on Drawing";
        Room.AdditionalNumber = "Additional Number";
        Room.CeilingHeight = 3000.0;
        Room.DesignedArea = 50.0;
        Room.ProgrammedArea = 45.0;
        Room.Perimeter = 20.0;


        if (Room is null)
        {
            throw new InvalidOperationException("Failed to create room.");
        }
    }
    public ValueTask DisposeAsync()
    {
        // Its not possible to delete rooms in dRofus, so we don't do anything here.
        return ValueTask.CompletedTask;
    }
}

public class RoomFixture : ClientSetupFixture, IAsyncLifetime
{
    private RoomCreator? _roomCreator;
    private bool _isDisposed;
    public Room Room => _roomCreator?.Room ?? throw new InvalidOperationException("Room has not been created or initialized.");
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _roomCreator = new RoomCreator(Client);
        await _roomCreator.InitializeAsync();
    }
    public override async Task DisposeAsync()
    {
        if (_isDisposed)
            return;
        try
        {
            if (_roomCreator is null)
                throw new InvalidOperationException("RoomCreator has not been initialized.");
            await _roomCreator.DisposeAsync();
            _isDisposed = true;
        }
        finally
        {
            await base.DisposeAsync();
        }
    }
}
