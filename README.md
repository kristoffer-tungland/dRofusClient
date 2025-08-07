# dRofusClient

C# library for interacting with the [dRofus](https://drofus.com) API.

## Supported Frameworks

- .NET 8
- .NET Standard 2.0

## Installation

```sh
dotnet add package dRofusClient
```

### Revit packages

To build Revit add-ins, install the companion package:

```sh
dotnet add package dRofusClient.Revit
```

When the official dRofus Revit add-in is already configured with auto login you can reuse those credentials, creating an authenticated client directly from a `Document`:

```csharp
using Autodesk.Revit.DB;

var client = new dRofusClientFactory().Create(document);
```

The factory extracts the connection details from the open Revit model so no username or password needs to be supplied.

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

## Contributing

Contributions are welcome! Please open issues or submit pull requests.

## License

[MIT](LICENSE)
