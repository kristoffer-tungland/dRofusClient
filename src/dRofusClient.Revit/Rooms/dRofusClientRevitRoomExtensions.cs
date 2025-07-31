using dRofusClient.Revit.Utils;
using dRofusClient.ApiLogs;
using dRofusClient.Options;

namespace dRofusClient.Rooms
{
    /// <summary>
    /// Synchronous wrappers for room related extension methods.
    /// </summary>
    public static class dRofusClientRevitRoomExtensions
    {
        public static List<Room> GetRooms(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomsAsync(query, cancellationToken));
        }

        public static Room CreateRoom(this IdRofusClient client, CreateRoom roomToCreate, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.CreateRoomAsync(roomToCreate, cancellationToken));
        }

        public static Room GetRoom(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomAsync(id, query, cancellationToken));
        }

        public static Room UpdateRoom(this IdRofusClient client, Room room, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.UpdateRoomAsync(room, cancellationToken));
        }

        public static TemplateConnectionStatus GetRoomEquipmentListStatus(this IdRofusClient client, int id, int equipmentListTypeId, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomEquipmentListStatusAsync(id, equipmentListTypeId, cancellationToken));
        }

        public static TemplateConnectionStatus UpdateRoomEquipmentListStatus(this IdRofusClient client, int id, int equipmentListTypeId, TemplateConnectionStatus body, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.UpdateRoomEquipmentListStatusAsync(id, equipmentListTypeId, body, cancellationToken));
        }

        public static List<Files.FileDetails> GetRoomFiles(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomFilesAsync(id, query, cancellationToken));
        }

        public static RoomGroup GetRoomGroup(this IdRofusClient client, int id, int groupTypeId, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomGroupAsync(id, groupTypeId, cancellationToken));
        }

        public static RoomGroupUpdate UpdateRoomGroup(this IdRofusClient client, int id, int groupTypeId, RoomGroupUpdate groupUpdate, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.UpdateRoomGroupAsync(id, groupTypeId, groupUpdate, cancellationToken));
        }

        public static List<Files.Image> GetRoomImages(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomImagesAsync(id, query, cancellationToken));
        }

        public static List<RoomLog> GetRoomLogs(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomLogsAsync(id, query, cancellationToken));
        }

        public static List<RoomLog> GetRoomLogs(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomLogsAsync(query, cancellationToken));
        }

        public static TemplateConnectionStatus GetRoomDataStatus(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomDataStatusAsync(id, cancellationToken));
        }

        public static TemplateConnectionStatus UpdateRoomDataStatus(this IdRofusClient client, int id, TemplateConnectionStatus status, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.UpdateRoomDataStatusAsync(id, status, cancellationToken));
        }

        public static byte[] GetRoomImage(this IdRofusClient client, int roomImageId, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomImageAsync(roomImageId, cancellationToken));
        }

        public static Files.Image GetRoomImageMeta(this IdRofusClient client, int roomImageId, ItemQuery? query = default, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetRoomImageMetaAsync(roomImageId, query, cancellationToken));
        }
    }
}
