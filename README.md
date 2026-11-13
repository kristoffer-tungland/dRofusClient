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

## Contributing

Contributions are welcome! Please open issues or submit pull requests.

### NuGet.org trusted publishing

The master/tag publishing workflow uses [NuGet trusted publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing)
instead of a stored NuGet API key. Before running it:

1. Sign in to nuget.org with the account that will authenticate publishing and create a **Trusted Publishing** policy.
   Select the individual or organization owner of the packages, and enter:
   - Repository owner: `kristoffer-tungland`
   - Repository: `dRofusClient`
   - Workflow file: `onPushMaster.yml` (filename only; preserve this spelling)
   - Environment: leave empty (the publishing job does not use a GitHub environment).
   Restrict the package pattern to `dRofusClient*` and allow publishing new versions.
   Also allow publishing new packages if a package ID being released is not yet registered, such as a new Revit version.
   The publishing set is `dRofusClient`, `dRofusClient.Windows`, `dRofusClient.WindowsCredentials`,
   and `dRofusClient.Revit.2021` through `dRofusClient.Revit.2027`.
2. In the GitHub repository's **Settings > Secrets and variables > Actions > Variables**, add the repository variable
   `NUGET_USER` containing the authenticating account's actual **nuget.org profile username**, not an email address
   or an assumed GitHub username. For an organization-owned policy, use the member account that created the policy.
3. Confirm the policy is active and covers all package IDs and their owners before incorporating the workflow change
   into `master`. A push to `master` or a matching `v*` tag starts publishing automatically.
   After a successful trusted publish, remove the unused `NUGET_API_KEY` repository secret.
   The existing `TOKEN` secret is still used for GitHub Packages and is not replaced by this migration.

`NuGet/login@v1` exchanges the publishing job's GitHub OIDC token for a temporary API key immediately before
pushing to NuGet.org. Only that job has `id-token: write`; build jobs retain read-only repository/package access.
The temporary key expires after one hour. Build configurations, semantic versioning, package destinations,
duplicate handling, and automatic release behavior are otherwise unchanged. A failed NuGet push stops the job
so it cannot create a release after only partially publishing packages.

Rerunning an older failed Actions run uses its original workflow revision, including the expired-key authentication;
it does not pick up this change. Recover the failed version using a run whose revision contains this migration,
and confirm its computed version before publishing. Package artifacts are retained for only one day.
Check which packages already exist on each feed; `--skip-duplicate` skips existing versions but does not replace them.

## License

[MIT](LICENSE)
