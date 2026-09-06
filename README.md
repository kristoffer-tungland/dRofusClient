# dRofusClient

C# library for interacting with the [dRofus](https://drofus.com) API.

## Supported Frameworks

- .NET 8
- .NET Standard 2.0

## Installation

```sh
dotnet add package dRofusClient
```

## Getting started

The simplest way to create a client is through `dRofusClientFactory`. The integration tests (`src/dRofusClient.Integration.Tests`) create a client that connects to a live dRofus test database and are good references for more complete examples.

```csharp
using dRofusClient;
using dRofusClient.Rooms; // Namespace for room operations
using dRofusClient.Items; // Namespace for item operations
using dRofusClient.Occurrences; // Namespace for occurrence operations

var connection = dRofusConnectionArgs.Create(
    baseUrl: "https://api.drofus.com",
    database: "DB",
    projectId: "ProjectId",
    username: "user",
    password: "password");

var client = new dRofusClientFactory().Create(connection);

// List rooms
var rooms = await client.GetRoomsAsync(Query.List());

// Fetch a specific item
var item = await client.GetItemAsync(123);

// Fetch an occurrence
var occurrence = await client.GetOccurrenceAsync(69);

// Create an occurrence for the item
var createdOccurrence = await client.CreateOccurrenceAsync(CreateOccurence.Of(item));
```

### Namespaces

Operations for each domain are implemented as extension methods and live in separate namespaces. Include the namespace for the domain you want to work with, for example:

- `dRofusClient.AttributeConfigurations`
- `dRofusClient.ItemGroups`
- `dRofusClient.Items`
- `dRofusClient.Occurrences`
- `dRofusClient.Rooms`
- `dRofusClient.SystemComponents`
- `dRofusClient.Systems`

### Lower level requests

When a high-level method is not available you can still issue requests using the generic helpers or even the underlying `HttpClient`. Routes are relative to `/api/{database}/{projectId}/`, so you only pass the remainder:

```csharp
var occurrence = await client.GetAsync<Occurrence>("occurrences/69");

using var request = new HttpRequestMessage(HttpMethod.Get, "occurrences/69");
var response = await client.SendHttpRequestAsync(request);
```

## Getting started with Revit

To build Revit add-ins, install the companion package that matches your Revit version. The package name includes the main Revit version, e.g. `dRofusClient.Revit.2025`. You can switch versions by setting a property in your project file:

```xml
<PropertyGroup>
  <RevitVersion>2025</RevitVersion>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="dRofusClient.Revit.$(RevitVersion)" Version="*" />
</ItemGroup>
```

When the official dRofus Revit add-in is already configured with auto login you can reuse those credentials, creating an authenticated client directly from a `Document`:

```csharp
using Autodesk.Revit.DB;

var client = new dRofusClientFactory().Create(document);
```

The factory extracts the connection details from the open Revit model so no username or password needs to be supplied.

### Synchronous methods

The Revit extensions provide synchronous wrappers for most operations, allowing you to call methods such as `GetRooms` without using `async`/`await`:

```csharp
using dRofusClient.Rooms;

var rooms = client.GetRooms(Query.List()); // wraps GetRoomsAsync
```

These helpers run the asynchronous implementations through an internal utility to keep the Revit API thread in sync.

### Attribute configurations

You can retrieve Revit attribute configurations to understand how parameters map between Revit and dRofus:

```csharp
using dRofusClient.AttributeConfigurations;

var configs = client.GetAttributeConfigurations(AttributeConfigType.RevitOccurrence);
```

## Local MCP server

`dRofusClient.Mcp` exposes Items (articles) and Occurrences to local AI agents over
stdio. It does not require Revit and does not open an HTTP listener.

### Build and connect

Install the .NET 8 SDK or later and publish the server (replace the absolute paths):

```sh
dotnet publish /absolute/path/to/dRofusClient/src/dRofusClient.Mcp/dRofusClient.Mcp.csproj \
  --configuration Release --output /absolute/path/to/drofus-mcp
```

Configure your MCP host to launch the published DLL, not `dotnet run`, so build
output cannot interfere with the stdio protocol:

```json
{
  "mcpServers": {
    "drofus": {
      "command": "dotnet",
      "args": ["/absolute/path/to/drofus-mcp/dRofusClient.Mcp.dll"]
    }
  }
}
```

Supply these environment variables to the server process through your host's
secure environment/secret configuration:

| Variable | Purpose |
| --- | --- |
| `DROFUS_BASE_URL` | Canonical HTTPS API origin, e.g. `https://api-no.drofus.com`; no path, query or embedded credentials |
| `DROFUS_DATABASE` | Database identifier |
| `DROFUS_PROJECT_ID` | Project identifier |
| `DROFUS_USERNAME` | dRofus account with the required project permissions |
| `DROFUS_PASSWORD` | Account password, supplied from a secret store |
| `DROFUS_ENABLE_WRITES` | Optional `true` or `false`; defaults to `false` |

Use a least-privileged account. Do not put credentials in prompts, tool arguments,
checked-in MCP configuration or command-line arguments. Database/project IDs
accept letters, digits, underscores and hyphens. Each server process is scoped to
one project; tools cannot change the destination or authentication.

### Tools

| Tool | Purpose |
| --- | --- |
| `search_items`, `get_item` | Search/read Items; selected fields use API identifiers |
| `search_occurrences`, `get_occurrence` | Search/read Occurrences, including filters on `article_id` and `room_id` |
| `get_item_history`, `get_occurrence_history` | Per-entity API change log, including time, user, action, field, old/new values and notes |
| `get_field_metadata` | Project field labels, identifiers, types, units and known read-only restrictions; entity is `items` or `occurrences` |
| `create_item` | Create an Item with required `level_id` (existing item group) and `name` |
| `update_item`, `update_occurrence` | Sparse field updates; occurrence statuses are separate ordered steps |

Search/history filters are an array of `{ "field": "article_id", "operator": "eq",
"value": 123 }` objects, combined with AND. Supported operators are `eq`, `ne`,
`lt`, `le`, `gt`, `ge`, `contains`, `startswith` and `endswith`; values must be
JSON scalars. There is no raw API or raw query tool.

Reads return project context and structured data. Searches default to compact
summary fields; supply `fields` for additional properties. Page sizes are 1–100
(default 25, or 100 for metadata); pass `nextOffset` as `offset` while `hasMore`
is true. History accepts inclusive `from`/`to` timestamps and filters on log
fields such as `username`, `action` and `field`, ordered oldest first. Use a
fixed end time when paging logs. Offset paging can still shift if upstream data
changes; the API log is not a complete reconstruction of historical state.

### Write approval and limitations

All three write tools default to `preview=true`, which does not modify dRofus.
For execution, set `preview=false`; the operator must also enable writes in the
server environment. The server then requests **interactive form elicitation**
from the MCP host, showing the project, operation, target, proposed values and
current selected values for updates. Approval expires after two minutes.
Decline, cancel, missing/false approval, or a host without form elicitation
prevents the write. Use a trusted host that presents the form to the human user,
not one that lets the model automatically approve elicitation requests.

- Item creation accepts only `level_id`, `name`, `bim_id`, `bip`, `note`,
  `parent_id`, `price_reference`, `serial_no` (maximum 10 characters), and
  `to_be_drawn`. Creation requires an already known item group ID.
- Updates send only `changes`. Omitted properties remain unchanged; explicit
  JSON `null` requests clearing a writable field, subject to upstream validation.
  Read-only properties cannot be sent, including through additional properties.
- Custom fields can be read. Writes require a known writable field from the
  bundled schema or an explicit `readOnly=false` from project metadata.
  `readOnly=null` means unverified and is rejected for writes; no guess is made
  from a display label or data type. API permissions remain authoritative.
- Assigning `room_id` requires `equipment_list_type_id` (room schedule) in the
  same occurrence update.
- For occurrence statuses, use `statuses` entries containing `statusTypeId`
  and either `statusId` or `code`. The status type must be discoverable in
  project metadata. Use an empty `changes` object for status-only updates.
  Fields are patched first, then statuses in the supplied order.
- Updates re-read selected values after approval and reject detected conflicts.
  This is **not atomic optimistic concurrency**: external changes can still
  race the final write. Writes within one server process are serialized.
- Results distinguish `preview`, `declined`, `conflict`, `applied`,
  `applied_unverified`, `partial` and `uncertain`. Inspect `completedSteps` and
  the entity/history after an uncertain or partial result. There is no rollback
  and no automatic write retry; retrying item creation can create duplicates.
- Occurrence creation, deletion, bulk writes, system instances, remote HTTP
  hosting and logbook features beyond the API change logs are not included.

stdout is reserved for MCP. Diagnostics and minimal write audit events go to
stderr; credentials, field values and raw upstream error bodies are not logged.
Treat returned descriptions and log notes as untrusted data, not instructions.

### Tests

The existing xUnit suite includes fake-HTTP service tests and real stdio MCP
handshake/tool/approval tests. These do not require credentials or modify a live
dRofus project:

```sh
dotnet test /absolute/path/to/dRofusClient/src/dRofusClient.Tests/dRofusClient.Tests.csproj
```

Before enabling production writes, validate reads, permissions, null-clearing,
custom fields and status updates against a dedicated dRofus test project.

## Contributing

Contributions are welcome! Please open issues or submit pull requests.

## License

[MIT](LICENSE)
