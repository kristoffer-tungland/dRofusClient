using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using dRofusClient.Items;

namespace dRofusClient.Rooms;

/// <summary>
/// Data transfer object for Room
/// </summary>
public record dRofusRoom : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Data transfer object for Room Log
/// </summary>
public record dRofusRoomLog : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Data transfer object for Template Connection Status
/// </summary>
public record dRofusTemplateConnectionStatus : dRofusDto
{
    public int Id { get; init; }
    public string? Status { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Data transfer object for Room Group
/// </summary>
public record dRofusRoomGroup : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Data transfer object for Room Group Update
/// </summary>
public record dRofusRoomGroupUpdate : dRofusDto
{
    public int? GroupId { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Extension methods for working with Rooms in dRofus API
/// </summary>
public static class dRofusClientRoomExtensions
{
    /// <summary>
    /// Get list of Rooms with the specified options
    /// </summary>
    public static Task<List<dRofusRoom>> GetRoomsAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusRoom>(dRofusType.Rooms.ToRequest(), options, cancellationToken);
    }

    /// <summary>
    /// Create new Room
    /// </summary>
    public static Task<dRofusRoom> CreateRoomAsync(this IdRofusClient client, dRofusRoom room, CancellationToken cancellationToken = default)
    {
        return client.PostAsync<dRofusRoom>(dRofusType.Rooms.ToRequest(), room.ToPostOption(), cancellationToken);
    }

    /// <summary>
    /// Get specified Room by ID
    /// </summary>
    public static Task<dRofusRoom> GetRoomAsync(this IdRofusClient client, int id, dRofusFieldsOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusRoom>(dRofusType.Rooms.CombineToRequest(id), options, cancellationToken);
    }

    /// <summary>
    /// Update an existing Room
    /// </summary>
    public static Task<dRofusRoom> UpdateRoomAsync(this IdRofusClient client, dRofusRoom room, CancellationToken cancellationToken = default)
    {
        return client.PatchAsync<dRofusRoom>(dRofusType.Rooms.CombineToRequest(room.Id), room.ToPatchOption(), cancellationToken);
    }

    /// <summary>
    /// Get files meta for specified Room
    /// </summary>
    public static Task<List<Items.dRofusFile>> GetRoomFilesAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<Items.dRofusFile>(dRofusType.Rooms.CombineToRequest(id, "files"), options, cancellationToken);
    }

    /// <summary>
    /// Get list of image metas for a specified Room
    /// </summary>
    public static Task<List<dRofusImage>> GetRoomImagesAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusImage>(dRofusType.Rooms.CombineToRequest(id, "images"), options, cancellationToken);
    }

    /// <summary>
    /// Get log entries for specified Room
    /// </summary>
    public static Task<dRofusRoomLog> GetRoomLogsAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusRoomLog>(dRofusType.Rooms.CombineToRequest(id, "logs"), options, cancellationToken);
    }

    /// <summary>
    /// Gets the RDS (Room Data Sheet) status for the given room id
    /// </summary>
    public static Task<dRofusTemplateConnectionStatus> GetRoomDataStatusAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusTemplateConnectionStatus>(dRofusType.Rooms.CombineToRequest(id, "roomdatastatus"), null, cancellationToken);
    }

    /// <summary>
    /// Update the RDS (Room Data Sheet) status for the given room id
    /// </summary>
    public static Task<dRofusTemplateConnectionStatus> UpdateRoomDataStatusAsync(this IdRofusClient client, int id, dRofusTemplateConnectionStatus status, CancellationToken cancellationToken = default)
    {
        return client.PatchAsync<dRofusTemplateConnectionStatus>(dRofusType.Rooms.CombineToRequest(id, "roomdatastatus"), status.ToPatchOption(), cancellationToken);
    }

    /// <summary>
    /// Get RoomGroup for specified Room and Room Group Type
    /// </summary>
    public static Task<dRofusRoomGroup> GetRoomGroupAsync(this IdRofusClient client, int roomId, int groupTypeId, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusRoomGroup>(dRofusType.Rooms.CombineToRequest(roomId, "groups", groupTypeId.ToString()), null, cancellationToken);
    }

    /// <summary>
    /// Update the Room Group for a specific Room and Room Group Type
    /// </summary>
    public static Task<dRofusRoomGroupUpdate> UpdateRoomGroupAsync(this IdRofusClient client, int roomId, int groupTypeId, dRofusRoomGroupUpdate update, CancellationToken cancellationToken = default)
    {
        return client.PatchAsync<dRofusRoomGroupUpdate>(dRofusType.Rooms.CombineToRequest(roomId, "groups", groupTypeId.ToString()), update.ToPatchOption(), cancellationToken);
    }

    /// <summary>
    /// Gets the equipment list status for the given room id and room equipment list type id
    /// </summary>
    public static Task<dRofusTemplateConnectionStatus> GetEquipmentListStatusAsync(this IdRofusClient client, int roomId, int equipmentListTypeId, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusTemplateConnectionStatus>(dRofusType.Rooms.CombineToRequest(roomId, "equipmentliststatus", equipmentListTypeId.ToString()), null, cancellationToken);
    }

    /// <summary>
    /// Update equipment list status for the given room and equipment list type
    /// </summary>
    public static Task<dRofusTemplateConnectionStatus> UpdateEquipmentListStatusAsync(this IdRofusClient client, int roomId, int equipmentListTypeId, dRofusTemplateConnectionStatus status, CancellationToken cancellationToken = default)
    {
        return client.PatchAsync<dRofusTemplateConnectionStatus>(dRofusType.Rooms.CombineToRequest(roomId, "equipmentliststatus", equipmentListTypeId.ToString()), status.ToPatchOption(), cancellationToken);
    }

    /// <summary>
    /// Get log entries for all rooms
    /// </summary>
    public static Task<List<dRofusRoomLog>> GetRoomsLogsAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusRoomLog>(dRofusType.Rooms.CombineToRequest("logs"), options, cancellationToken);
    }
}