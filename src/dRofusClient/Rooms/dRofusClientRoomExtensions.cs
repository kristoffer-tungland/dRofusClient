using dRofusClient.ApiLogs;

namespace dRofusClient.Rooms;

/// <summary>
/// Extension methods for room related endpoints.
/// </summary>
public static class dRofusClientRoomExtensions
{
    public static Task<List<Room>> GetRoomsAsync(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
        => client.GetListAsync<Room>(dRofusType.Rooms.ToRequest(), query, cancellationToken);

    public static Task<Room> CreateRoomAsync(this IdRofusClient client, CreateRoom roomToCreate, CancellationToken cancellationToken = default)
        => client.PostAsync<Room>(dRofusType.Rooms.ToRequest(), roomToCreate.ToPostRequest(), cancellationToken);

    public static Task<Room> GetRoomAsync(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
        => client.GetAsync<Room>(dRofusType.Rooms.CombineToRequest(id), query, cancellationToken);

    public static async Task<Room> UpdateRoomAsync(this IdRofusClient client, Room room, CancellationToken cancellationToken = default)
    {
        room = room.ClearReadOnlyFields();
        var patchOptions = room.ToPatchRequest();
        Room? result = null;
        if (patchOptions.Body is not null && patchOptions.Body != "{}")
            result = await client.PatchAsync<Room>(dRofusType.Rooms.CombineToRequest(room.Id), patchOptions, cancellationToken).ConfigureAwait(false);
        result ??= room with { Id = room.Id };
        return result;
    }

    [Obsolete("Deleting rooms is not supported in dRofus. Use UpdateRoomAsync with a null value for the room to delete it instead.")]
    public static Task DeleteRoomAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Deleting rooms is not supported in dRofus.");
    }

    public static Task<TemplateConnectionStatus> GetRoomEquipmentListStatusAsync(this IdRofusClient client, int id, int equipmentListTypeId, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest(id, "equipmentliststatus", equipmentListTypeId.ToString());
        return client.GetAsync<TemplateConnectionStatus>(request, null, cancellationToken);
    }

    public static Task<TemplateConnectionStatus> UpdateRoomEquipmentListStatusAsync(this IdRofusClient client, int id, int equipmentListTypeId, TemplateConnectionStatus body, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest(id, "equipmentliststatus", equipmentListTypeId.ToString());
        return client.PatchAsync<TemplateConnectionStatus>(request, body.ToPatchRequest(), cancellationToken);
    }

    public static Task<List<Files.FileDetails>> GetRoomFilesAsync(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest(id, "files");
        return client.GetListAsync<Files.FileDetails>(request, query, cancellationToken);
    }

    public static Task<RoomGroup> GetRoomGroupAsync(this IdRofusClient client, int id, int groupTypeId, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest(id, "groups", groupTypeId.ToString());
        return client.GetAsync<RoomGroup>(request, null, cancellationToken);
    }

    public static Task<RoomGroupUpdate> UpdateRoomGroupAsync(this IdRofusClient client, int id, int groupTypeId, RoomGroupUpdate groupUpdate, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest(id, "groups", groupTypeId.ToString());
        return client.PatchAsync<RoomGroupUpdate>(request, groupUpdate.ToPatchRequest(), cancellationToken);
    }

    public static Task<List<Files.Image>> GetRoomImagesAsync(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest(id, "images");
        return client.GetListAsync<Files.Image>(request, query, cancellationToken);
    }

    public static Task<List<RoomLog>> GetRoomLogsAsync(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest(id, "logs");
        return client.GetListAsync<RoomLog>(request, query, cancellationToken);
    }

    public static Task<List<RoomLog>> GetRoomLogsAsync(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest("logs");
        return client.GetListAsync<RoomLog>(request, query, cancellationToken);
    }

    public static Task<TemplateConnectionStatus> GetRoomDataStatusAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest(id, "roomdatastatus");
        return client.GetAsync<TemplateConnectionStatus>(request, null, cancellationToken);
    }

    public static Task<TemplateConnectionStatus> UpdateRoomDataStatusAsync(this IdRofusClient client, int id, TemplateConnectionStatus status, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest(id, "roomdatastatus");
        return client.PatchAsync<TemplateConnectionStatus>(request, status.ToPatchRequest(), cancellationToken);
    }

    public static async Task<byte[]> GetRoomImageAsync(this IdRofusClient client, int roomImageId, CancellationToken cancellationToken = default)
    {
        var (db, pr) = client.GetDatabaseAndProjectId();
        var url = $"/api/{db}/{pr}/" + dRofusType.Rooms.CombineToRequest("images", roomImageId.ToString());
        return await client.HttpClient.GetByteArrayAsync(url).ConfigureAwait(false);
    }

    public static Task<Files.Image> GetRoomImageMetaAsync(this IdRofusClient client, int roomImageId, ItemQuery? query = default, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Rooms.CombineToRequest("images", roomImageId.ToString(), "meta");
        return client.GetAsync<Files.Image>(request, query, cancellationToken);
    }
}
