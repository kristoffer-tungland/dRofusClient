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

`dRofusClient.Mcp` exposes Items (articles), Occurrences and Rooms to local AI agents over
stdio. It does not require Revit and does not open an HTTP listener.

### Requirements

- A dRofus account with API access and permission to the intended database/project.
  Ask your administrator for the HTTPS API origin, database ID and project ID.
- An MCP host supporting local **stdio** servers, such as Claude Desktop,
  VS Code with GitHub Copilot, or OpenCode. Install and sign in to the host first.
  Your organization's AI/data-sharing policy must permit sending project data
  to the selected model provider.
- A supported 64-bit Windows, macOS or glibc-based Linux system matching an asset
  below, with outbound HTTPS access to your dRofus API.
- **Release binaries are self-contained .NET 8 applications: no separate .NET
  installation is needed.** Native OS libraries are still required. Building
  from source needs the .NET SDK; running a framework-dependent publish needs the
  .NET 8 runtime. Neither Node.js nor Python is required by this server.

### Installation (Windows, macOS and Linux)

Open [GitHub Releases](https://github.com/kristoffer-tungland/dRofusClient/releases)
and select a release that actually lists the MCP assets. **The existing `v1.0.15`
release has no MCP binaries.** The packaging described here is for releases
produced by the new pipeline; it does not mean those assets are already live.
Until a release contains them, use [Build from source](#build-from-source).

Set `VERSION` to the selected release's version **without its leading `v`**;
the download commands use tag `v${VERSION}`. Do not use `1.0.15` for MCP downloads.
The available asset names will be:

| OS / architecture | Asset |
| --- | --- |
| Windows x64 | `dRofusClient.Mcp-VERSION-win-x64.zip` |
| macOS Intel | `dRofusClient.Mcp-VERSION-osx-x64.tar.gz` |
| macOS Apple Silicon | `dRofusClient.Mcp-VERSION-osx-arm64.tar.gz` |
| Linux x64 (glibc) | `dRofusClient.Mcp-VERSION-linux-x64.tar.gz` |
| Linux ARM64 (glibc) | `dRofusClient.Mcp-VERSION-linux-arm64.tar.gz` |

Download the matching archive and `SHA256SUMS` from the **same release**. Verify
the checksum before extracting; stop on any mismatch. A matching checksum checks
integrity, not independent publisher identity: download only from the trusted
repository. GitHub's automatic “Source code” archives are not runnable binaries.

**Extract ALL files together**, not just the executable. Keep the DLLs,
`.deps.json`, `.runtimeconfig.json`, runtime/native libraries and XML documentation
(including the core library's generated `dRofusClient.VERSION.xml`)
in the installed directory. The Swagger schema is embedded in the application;
no separate Swagger download is needed. Install each version in its own directory
and update the host's command path when upgrading.

**For maintainers:** [onPushMaster.yml](.github/workflows/onPushMaster.yml) runs on
pushes to `master`, `v*` tags, or manual `workflow_dispatch`. A single job runs the
existing `paulhatch/semantic-version` calculation and shares its version with both
NuGet and MCP packaging; there is no version input or manual version bump.
Manual dispatch has a boolean `publish_release` input (default `false`): setting
it explicitly to `true` permits a GitHub release from the selected ref, including
a feature branch. Without that opt-in, manual feature-branch runs build only.
NuGet pushes remain restricted to `master` and `v*` tags; a manual feature-branch
release does not push NuGet packages. Every path uses the shared semantic version,
not a version input. The release stays draft until all five native-runner test
jobs and archive upload/download SHA256 checks pass. Existing public versions
cannot be overwritten. Dispatch may remain unavailable until the default branch
contains `workflow_dispatch`; merge the workflow changes into `master` first to
enable it.

#### Windows (PowerShell, x64)

Run in PowerShell, enter the version you selected, and keep the printed executable
path for your host configuration:

```powershell
$ErrorActionPreference = "Stop"
$VERSION = Read-Host "Release version without v (must contain MCP assets)"
$Asset = "dRofusClient.Mcp-$VERSION-win-x64.zip"
$Release = "https://github.com/kristoffer-tungland/dRofusClient/releases/download/v$VERSION"
$DownloadDir = Join-Path $HOME "Downloads\drofus-mcp-$VERSION"
$InstallDir = Join-Path $env:LOCALAPPDATA "drofus-mcp\$VERSION"
New-Item -ItemType Directory -Force -Path $DownloadDir | Out-Null
Invoke-WebRequest "$Release/$Asset" -OutFile (Join-Path $DownloadDir $Asset)
Invoke-WebRequest "$Release/SHA256SUMS" -OutFile (Join-Path $DownloadDir "SHA256SUMS")
$Entries = @(Get-Content (Join-Path $DownloadDir "SHA256SUMS") | Where-Object {
    ($_ -split '\s+')[-1].TrimStart('*') -eq $Asset
})
if ($Entries.Count -ne 1) { throw "Missing or ambiguous checksum entry" }
$Expected = ($Entries[0] -split '\s+')[0]
$Actual = (Get-FileHash (Join-Path $DownloadDir $Asset) -Algorithm SHA256).Hash
if ($Actual -ne $Expected) { throw "Checksum mismatch; do not install" }
Expand-Archive -Path (Join-Path $DownloadDir $Asset) -DestinationPath $InstallDir
Join-Path $InstallDir "dRofusClient.Mcp.exe"
```

If the installation directory already exists, choose a fresh directory instead
of mixing old and new files. There is no native Windows ARM64 asset in this list.

#### macOS (Terminal)

Use **About This Mac** to check the chip: Apple Silicon uses `osx-arm64`; Intel
uses `osx-x64`. `uname -m` prints `arm64` or `x86_64`, but a Terminal running under
Rosetta can report `x86_64` on Apple Silicon. Set `RID` for your actual chip:

```sh
printf 'Release version without v (must contain MCP assets): '
read -r VERSION
RID=osx-arm64 # Change to osx-x64 for Intel
ASSET="dRofusClient.Mcp-$VERSION-$RID.tar.gz"
RELEASE="https://github.com/kristoffer-tungland/dRofusClient/releases/download/v$VERSION"
DOWNLOAD_DIR="$HOME/Downloads/drofus-mcp-$VERSION"
INSTALL_DIR="$HOME/Applications/drofus-mcp/$VERSION"
mkdir -p "$DOWNLOAD_DIR" &&
cd "$DOWNLOAD_DIR" &&
curl -fLO "$RELEASE/$ASSET" &&
curl -fLO "$RELEASE/SHA256SUMS" &&
grep -F "  $ASSET" SHA256SUMS | shasum -a 256 -c - &&
mkdir -p "$(dirname "$INSTALL_DIR")" &&
mkdir "$INSTALL_DIR" &&
tar -xzf "$ASSET" -C "$INSTALL_DIR" &&
chmod u+x "$INSTALL_DIR/dRofusClient.Mcp" &&
printf 'Installed command: %s/dRofusClient.Mcp\n' "$INSTALL_DIR"
```

If Gatekeeper blocks this trusted download, first verify its source and checksum.
After the blocked launch, use **System Settings → Privacy & Security → Open
Anyway** for this application, then confirm the prompt. See
[Apple's guidance](https://support.apple.com/en-us/102445). Do not globally disable
Gatekeeper or remove quarantine recursively from unrelated files. If macOS reports
malware or your organization's policy blocks the application, stop and contact
your administrator rather than bypassing the warning.

#### Linux (Terminal, glibc)

Use `linux-x64` for `uname -m` = `x86_64`, or `linux-arm64` for `aarch64`/`arm64`.
These assets target **glibc, not Alpine/musl**. Use a supported glibc distribution
(or a glibc-based container) instead of trying to run them directly on Alpine.
You need `curl`, `tar` and `sha256sum`:

```sh
printf 'Release version without v (must contain MCP assets): '
read -r VERSION
RID=linux-x64 # Change to linux-arm64 for ARM64
ASSET="dRofusClient.Mcp-$VERSION-$RID.tar.gz"
RELEASE="https://github.com/kristoffer-tungland/dRofusClient/releases/download/v$VERSION"
DOWNLOAD_DIR="$HOME/Downloads/drofus-mcp-$VERSION"
INSTALL_DIR="$HOME/.local/share/drofus-mcp/$VERSION"
mkdir -p "$DOWNLOAD_DIR" &&
cd "$DOWNLOAD_DIR" &&
curl -fLO "$RELEASE/$ASSET" &&
curl -fLO "$RELEASE/SHA256SUMS" &&
grep -F "  $ASSET" SHA256SUMS | sha256sum -c - &&
mkdir -p "$(dirname "$INSTALL_DIR")" &&
mkdir "$INSTALL_DIR" &&
tar -xzf "$ASSET" -C "$INSTALL_DIR" &&
chmod u+x "$INSTALL_DIR/dRofusClient.Mcp" &&
printf 'Installed command: %s/dRofusClient.Mcp\n' "$INSTALL_DIR"
```

Self-contained does not mean statically linked. The OS still needs the .NET 8
native dependencies, including glibc, ICU, OpenSSL, zlib, libgcc and libstdc++;
some distributions also require Kerberos libraries. Install the packages for
your distribution/version using Microsoft's
[.NET Linux prerequisites](https://learn.microsoft.com/en-us/dotnet/core/install/linux).
Do not work around missing libraries by disabling TLS checks or globalization.

### Quick Start

1. Install/extract the matching release above, or publish from source below.
2. Choose **one** host configuration below. Replace every `YOUR_*`/`VERSION`
   placeholder with your own details. Use the absolute executable path printed
   by installation, not the archive path or a path inside Downloads.
3. Keep `DROFUS_ENABLE_WRITES=false`. Start/restart the server through the host,
   review its trust prompt and enable only the tools you need.
4. Ask: **“Using drofus, list up to five rooms in the configured project. Show
   their IDs and names; do not change anything.”** Approve the read tool if asked.
   This verifies the connection and project permissions without modifying data.
5. Only consider writes after reviewing [Write approval and limitations](#write-approval-and-limitations)
   and testing with a dedicated test project.

**The host launches the server and communicates over stdio.** There is no web
page, port, interactive login or startup banner. Running the executable manually
with valid configuration normally waits silently for MCP input; use Ctrl+C to
stop it, then let the host start its own process.

### Host configuration

Merge the example into the indicated configuration file without removing your
other servers/settings. These hosts use **different JSON formats**. Paths in JSON
are literal: do not assume `~`, `$HOME` or `%LOCALAPPDATA%` expands in `command`.
Use the resolved absolute path, doubling backslashes in Windows JSON.
Example installed paths (replace username and version):

- Windows: `C:\\Users\\YOUR_USER\\AppData\\Local\\drofus-mcp\\VERSION\\dRofusClient.Mcp.exe`
- macOS: `/Users/YOUR_USER/Applications/drofus-mcp/VERSION/dRofusClient.Mcp`
- Linux: `/home/YOUR_USER/.local/share/drofus-mcp/VERSION/dRofusClient.Mcp`

#### Claude Desktop

Open **Settings → Developer → Edit Config**. This opens
`claude_desktop_config.json`; on Windows it is under `%APPDATA%\Claude`, and on
macOS under your home directory's `Library/Application Support/Claude`.
Use the path for your installation in this example:

```json
{
  "mcpServers": {
    "drofus": {
      "command": "/Users/YOUR_USER/Applications/drofus-mcp/VERSION/dRofusClient.Mcp",
      "args": [],
      "env": {
        "DROFUS_BASE_URL": "https://api-no.drofus.com",
        "DROFUS_DATABASE": "YOUR_DATABASE",
        "DROFUS_PROJECT_ID": "YOUR_PROJECT_ID",
        "DROFUS_USERNAME": "YOUR_USERNAME",
        "DROFUS_PASSWORD": "YOUR_PASSWORD",
        "DROFUS_ENABLE_WRITES": "false"
      }
    }
  }
}
```

The password above is a placeholder, **not a secret-store reference**. Replacing
it stores your password in plaintext in the config file. Never commit or share
that file, paste it into chat, or include it in screenshots; restrict access to
your OS account and protect backups. Prefer injecting credentials through a
trusted launcher/secret manager's process environment (omit the corresponding
`env` keys), or on Windows use [Windows Credential Manager](#windows-credential-manager):
remove `DROFUS_PASSWORD` and add `"DROFUS_USE_WINDOWS_CREDENTIALS": "true"`.
GUI apps do not necessarily inherit a terminal's environment.

Fully quit and reopen Claude Desktop, then inspect the `drofus` connector/tools.
The official [local server guide](https://modelcontextprotocol.io/docs/develop/connect-local-servers)
describes this `mcpServers` format; UI names can vary by host version.

#### VS Code with GitHub Copilot

Use **MCP: Open User Configuration** for a personal profile, or create
`.vscode/mcp.json` in your workspace. This example deliberately uses VS Code's
`servers`/`inputs` format so the password is prompted for and securely stored by
VS Code, rather than written into the JSON:

```json
{
  "inputs": [
    {
      "type": "promptString",
      "id": "drofus-password",
      "description": "dRofus password",
      "password": true
    }
  ],
  "servers": {
    "drofus": {
      "type": "stdio",
      "command": "C:\\Users\\YOUR_USER\\AppData\\Local\\drofus-mcp\\VERSION\\dRofusClient.Mcp.exe",
      "args": [],
      "env": {
        "DROFUS_BASE_URL": "https://api-no.drofus.com",
        "DROFUS_DATABASE": "YOUR_DATABASE",
        "DROFUS_PROJECT_ID": "YOUR_PROJECT_ID",
        "DROFUS_USERNAME": "YOUR_USERNAME",
        "DROFUS_PASSWORD": "${input:drofus-password}",
        "DROFUS_ENABLE_WRITES": "false"
      }
    }
  }
}
```

On macOS/Linux replace `command` with the appropriate absolute path above. Use
**MCP: List Servers → drofus → Start**, review trust, and enter your password.
Open Copilot Chat in an agent mode supporting MCP and select the `drofus` tools
in **Configure Tools**. Keep tool approval prompts enabled; do not enable global
auto-approval to make setup work.

**Compatibility:** current VS Code docs also describe portable `.mcp.json` and
Copilot `mcp-config.json` files using `mcpServers`. Those formats do **not** support
VS Code `${input:...}` variables. Copilot harness sessions do not receive servers
requiring interactive input from `.vscode/mcp.json`. Use a VS Code session that
supports the input-variable configuration above; for a harness session, follow
the official portable configuration guidance with securely supplied environment
credentials instead. Do not paste a password into a shared portable config.
See [MCP server setup](https://code.visualstudio.com/docs/agent-customization/mcp-servers)
and the [configuration reference](https://code.visualstudio.com/docs/agents/reference/mcp-configuration).

#### OpenCode

Merge this into `opencode.json` in your project root, or your personal
`opencode.json` in your home directory's `.config/opencode` directory. OpenCode
uses `mcp`, `type: "local"`, a **command array**, and `environment` (not `env`):

```json
{
  "$schema": "https://opencode.ai/config.json",
  "mcp": {
    "drofus": {
      "type": "local",
      "command": ["/home/YOUR_USER/.local/share/drofus-mcp/VERSION/dRofusClient.Mcp"],
      "enabled": true,
      "environment": {
        "DROFUS_BASE_URL": "https://api-no.drofus.com",
        "DROFUS_DATABASE": "YOUR_DATABASE",
        "DROFUS_PROJECT_ID": "YOUR_PROJECT_ID",
        "DROFUS_USERNAME": "YOUR_USERNAME",
        "DROFUS_PASSWORD": "{env:DROFUS_PASSWORD}",
        "DROFUS_ENABLE_WRITES": "false"
      }
    }
  }
}
```

Change the command for Windows/macOS. Provide `DROFUS_PASSWORD` in the environment
that launches OpenCode, preferably through your secret manager. For a one-session
Bash launch on macOS/Linux, this prompts without echoing the password or recording
it in shell history:

```bash
read -r -s -p 'dRofus password: ' DROFUS_PASSWORD
printf '\n'
export DROFUS_PASSWORD
opencode
unset DROFUS_PASSWORD
```

Run that block in **Bash**, not macOS's default zsh. Environment variables are
not an encrypted vault; limit access to your account and avoid dumping the
environment into logs. OpenCode's documented `{env:NAME}` substitution produces
an empty string when a variable is absent, so an unset password will fail startup
unless the Windows fallback is configured. On Windows you can instead remove the
password entry and enable the credential-store option described below.

Restart OpenCode after changes, run `opencode mcp list` to inspect server status,
then ask the read-only Quick Start question. Review your host's tool permissions;
do not grant blanket approval. See the official
[MCP configuration](https://opencode.ai/docs/mcp-servers/) and
[environment substitution](https://opencode.ai/docs/config/#env-vars) docs.

**All hosts:** enabling writes is not sufficient to execute them. A host must
support **interactive form elicitation** and show the approval form to the human.
Hosts without it can read and preview, but cannot execute writes even with
`DROFUS_ENABLE_WRITES=true`. Ordinary tool-call approval is not a replacement.

### Build from source

If binaries are not yet available, install the .NET 8 SDK (or a compatible later
SDK) and publish the server from this repository (replace the absolute paths).
This explicitly creates a **framework-dependent** publish; the machine running
it needs the .NET 8 runtime, even when a newer SDK was used to build:

```sh
dotnet publish /absolute/path/to/dRofusClient/src/dRofusClient.Mcp/dRofusClient.Mcp.csproj \
  --configuration Release --self-contained false --output /absolute/path/to/drofus-mcp
```

Keep the entire output directory. Configure your MCP host to launch the published
DLL, not `dotnet run`, so build output cannot interfere with the stdio protocol.
For example, using Claude's format (replace `/absolute/path/to/dotnet` with the
installed executable's full path, obtainable with `command -v dotnet` on Unix or
`(Get-Command dotnet).Source` in PowerShell):

```json
{
  "mcpServers": {
    "drofus": {
      "command": "/absolute/path/to/dotnet",
      "args": ["/absolute/path/to/drofus-mcp/dRofusClient.Mcp.dll"]
    }
  }
}
```

Add the same environment configuration shown above. For VS Code use `servers`
and `env`; for OpenCode use `"command": ["/absolute/path/to/dotnet",
"/absolute/path/to/drofus-mcp/dRofusClient.Mcp.dll"]` and `environment`.

### Connection configuration

Supply these environment variables to the server process through your host's
secure environment/secret configuration:

| Variable | Purpose |
| --- | --- |
| `DROFUS_BASE_URL` | Canonical HTTPS API origin, e.g. `https://api-no.drofus.com`; no path, query or embedded credentials |
| `DROFUS_DATABASE` | Database identifier |
| `DROFUS_PROJECT_ID` | Project identifier |
| `DROFUS_USERNAME` | dRofus account with the required project permissions |
| `DROFUS_PASSWORD` | Account password; takes precedence over Windows Credential Manager when nonblank |
| `DROFUS_USE_WINDOWS_CREDENTIALS` | Optional `true` or `false` (default `false`); if enabled and `DROFUS_PASSWORD` is absent/blank, read the saved Windows password |
| `DROFUS_ENABLE_WRITES` | Optional `true` or `false`; defaults to `false` |

Use a least-privileged account. Do not put credentials in prompts, tool arguments,
checked-in MCP configuration or command-line arguments. Database/project IDs
accept letters, digits, underscores and hyphens. Each server process is scoped to
one project; tools cannot change the destination or authentication.

#### Windows Credential Manager

On Windows, set `DROFUS_USE_WINDOWS_CREDENTIALS=true` and omit `DROFUS_PASSWORD`
to reuse a saved password. Continue to supply `DROFUS_BASE_URL`, `DROFUS_DATABASE`,
`DROFUS_PROJECT_ID` and `DROFUS_USERNAME`; only the password comes from the store.
A nonblank environment password always wins, so existing configurations continue
to work on Windows, Linux and macOS without accessing the credential store.

Use the existing dRofus Windows login's **Remember me** option, or open
**Control Panel → Credential Manager → Windows Credentials → Add a generic
credential**. The target must match the existing library convention:
`drofus://<username>@<server>`, with the configured username and password.
The server is the API origin without its scheme or trailing slash, except
`https://api-no.drofus.com`, which maps to `db2.nosyko.no`.
For example, user `alice` on the Nordic server uses
`drofus://alice@db2.nosyko.no`; on the EU server it uses
`drofus://alice@api-eu.drofus.com`.

Run the MCP host under the Windows account that owns the saved credential.
The server reads that single target at startup; it never enumerates, creates,
updates or deletes credentials. Missing/empty credentials, store failures, or
attempting the fallback on a non-Windows machine fail startup without exposing
passwords, credential targets or raw store errors. There is no interactive
password prompt and agents cannot enable or access the store through tools.

### Troubleshooting

| Symptom | What to check |
| --- | --- |
| Download returns 404 / no asset | Select a release whose Assets list includes MCP archives; `v1.0.15` does not. Match its version/tag exactly or build from source. |
| Checksum mismatch | Stop; delete the suspect download and fetch the archive and `SHA256SUMS` again from the same trusted release. Do not ignore the failure. |
| Executable missing / `ENOENT` | Extract the complete archive and use its absolute executable path. Windows uses `.exe`; macOS/Linux do not. Check JSON escaping and remove unresolved placeholders. |
| Permission denied / blocked launch | macOS/Linux: apply `chmod u+x` to the installed executable and check whether the filesystem is mounted `noexec`. macOS: follow the per-app Gatekeeper steps above only for a trusted download. |
| Bad CPU type / exec format error | Download the architecture matching your OS/CPU; Linux and macOS archives are not interchangeable. Check Rosetta on Apple Silicon. |
| Missing ICU, SSL or other native libraries | Install your distribution's .NET 8 native prerequisites. Linux assets require glibc and do not run directly on Alpine/musl. |
| “Install .NET” / missing framework | A release archive should be self-contained: re-extract all files and launch its executable. A source framework-dependent publish requires the .NET 8 runtime. |
| Manual launch appears to hang | Normal for stdio after successful startup: it is waiting for a host handshake. Stop it with Ctrl+C and start through the MCP host; there is no HTTP endpoint. |
| Server exits / credentials absent | Check the host's environment, API origin and database/project identifiers. GUI apps might not inherit shell variables. An unset OpenCode `{env:...}` becomes empty. |
| Authentication/authorization error | Verify the regional API origin, account, project access and password privately with your administrator. Windows fallback requires the same OS account and exact saved credential target. Never paste credentials into chat. |
| Tools absent in VS Code | Check the selected config format, restart the server, inspect **MCP: List Servers → drofus → Show Output**, and enable its tools. Input-variable configurations are not forwarded to Copilot harness sessions; see the compatibility note above. |
| Claude fails to connect | Fully quit/reopen it and inspect `mcp.log` / `mcp-server-drofus.log` under your home directory's `Library/Logs/Claude` (macOS) or `%APPDATA%\Claude\logs` (Windows). |
| OpenCode fails to connect | Check `opencode mcp list`, the `mcp`/`environment` keys and the launching process's environment. This is a local server, not an HTTP/OAuth connection. |
| Write declined or unavailable | Check preview mode, write opt-in, API permissions and human form approval. Hosts without form elicitation cannot execute writes. Do not bypass approvals or automatically retry writes. |

Diagnostics go to stderr, not stdout. Review logs locally and redact any
project/personal data before sharing. Never add shell banners or logging to stdout
in a launcher, because that stream carries the MCP protocol.

### Tools

| Tool | Purpose |
| --- | --- |
| `search_items`, `get_item` | Search/read Items; selected fields use API identifiers |
| `search_occurrences`, `get_occurrence` | Search/read Occurrences, including filters on `article_id` and `room_id` |
| `search_rooms`, `get_room` | Find Rooms and read selected identity/requirement fields |
| `get_room_occurrences` | Paginated occurrences constrained to `roomId`, optionally `equipmentListTypeId`, with assignment and quantity fields always included |
| `get_item_history`, `get_occurrence_history`, `get_room_history` | Per-entity API change log, including time, user, action, field, old/new values and notes |
| `get_field_metadata` | Project field labels, identifiers, types, units and known read-only restrictions; entity is `items`, `occurrences` or `rooms` |
| `list_attribute_configurations` | Paginated configuration summaries including type, applicability, user availability and default status |
| `get_attribute_configuration` | Configuration context and paginated mappings, preserving identifiers, labels and raw directions |
| `find_attribute_mappings` | Exact ID/label lookup within a selected configuration, with explicit ambiguity and missing-data results |
| `resolve_property` | Endpoint-aware resolution of API names, verified built-in aliases/synonyms and custom labels; returns provenance and never confirms fuzzy suggestions |
| `search_custom_properties` | List/search live custom and dynamic properties by API ID, label or group, with optional exact `propertyGroup` filtering |
| `resolve_custom_property` | Resolve an exact ID, label or `group: label` to an API ID, reporting ambiguous or missing matches instead of guessing |
| `create_item` | Create an Item with required `level_id` (existing item group) and `name` |
| `create_room` | Create a Room with required `name` and supported optional creation fields |
| `update_item`, `update_occurrence`, `update_room` | Sparse field updates; occurrence statuses are separate ordered steps |

### Built-in-first property resolution

When a user names a property, agents should first call `resolve_property` with
`entity` and `property`, rather than perform a keyword search of custom fields.
Resolution supports `items`, `occurrences`, `rooms` and `systems`. Room fields can
be read with `get_room` and verified writable fields changed with `update_room`
under the normal write safeguards. Resolution does not enable System read/write tools.

Resolution order:

1. Exact API field name (case-insensitive).
2. Verified built-in CLR property names, human-readable CLR names, OpenAPI titles,
   and live metadata display names.
3. Built-in alternative labels from OpenAPI descriptions and XML summaries.
4. Exact custom-property labels or grouped titles.
5. Last-resort edit-distance suggestions.
6. Ask the user to clarify unresolved, ambiguous or suggested matches.

Mappings are generated from each endpoint's bundled OpenAPI schema, actual
`JsonPropertyName` attributes on client models, generated client XML documentation,
and current project metadata. There is no hand-maintained synonym dictionary,
translation guess, or fallback that constructs an API ID from an arbitrary name.
The resolver retains full labels and removes explicit `Group: ` prefixes to
support unqualified display names. Model names are also split at word boundaries.
Documentation and metadata are data, not instructions.

Results include `status`, `stage`, `resolvedId`, total `matchCount`, and paginated
`candidates` containing field contracts, `builtIn`, and aliases with their
`source`. Only a unique verified match returns `resolvedId`. Built-in aliases
take precedence over custom labels; an exact custom API ID still wins at step 1.
Conflicting high-confidence built-in aliases or documented synonyms return
`ambiguous` instead of selecting the first result. Grouped titles or exact API
IDs can disambiguate. Pagination never conceals the total ambiguity.

Fuzzy results return `suggestions` with no resolved ID, even for one candidate.
They are not permission to read or write a guessed field: obtain clarification
and resolve the confirmed choice first. `not_found` also requires clarification.
The resolver accepts page sizes of 1–100 (default 25); use `nextOffset` while
`hasMore` is true.

The core build generates XML documentation alongside its assembly. Preserve that
XML file when deploying the published server; XML-only labels are unavailable
if it is removed. Schema and model mappings still work without XML, but no
missing documentation is guessed. Live metadata failures are reported rather
than silently using an incomplete project catalog.

Verified model examples: Room `RoomNumber` maps to `architect_no`, while System
`SystemComponentId` maps to `base_occurrence_id`. These come directly from
`JsonPropertyName` attributes, not snake-case guesses. Item's two documented
“Serial Number” labels are ambiguous without their group or API ID.

### Custom-property discovery

Use the custom-only tools after verified built-in resolution fails, or to browse
known custom properties explicitly. They do not replace `resolve_property` and
must not override its built-in matches or ambiguities.

Both custom-property tools accept `entity` (`items`, `occurrences` or `rooms`), `limit`
(1–100, default 25) and `offset`. Discovery uses live project metadata and excludes
standard fields defined in the bundled schema or client DTOs; dynamic status fields
are included. Use `get_field_metadata` for standard fields. Results retain the
property group, grouped title, original `dataType`, normalized `type`, unit and
conservative `readOnly` flag.

Call `search_custom_properties` with no `search` to list properties, or supply a
case-insensitive substring of the ID or grouped title. To resolve a label, call
`resolve_custom_property` with `property`, optionally narrowed by an exact
`propertyGroup`. Matching is case-insensitive: exact IDs take precedence over exact
labels/titles. Resolution returns `resolved`, `ambiguous` or `not_found`, a
`resolvedId` only for a unique match, the full `matchCount`, and paginated
`candidates`. Pagination never turns an ambiguous result into a unique match.
Use the returned API ID verbatim in `fields`, filters or `changes`; never construct
IDs from display labels. Unknown writability remains blocked, and occurrence
status changes still go through `statuses`, not ordinary field patches.

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

### Room requirements and assigned equipment

For questions such as “How many sockets does this room need?” or “Should it have
water outlets?”, the tool descriptions and server instructions guide agents to:

1. Find the Room with `search_rooms`, using verified identity fields such as
   `architect_no` (room number), and clarify multiple matches rather than guessing.
2. Resolve requirement labels using `resolve_property` with `entity="rooms"`.
   Inspect `get_field_metadata` and, after built-in resolution fails, custom-property
   discovery. There are no assumed universal socket/water-outlet field IDs.
3. Read the resolved IDs through `get_room(fields=...)`. Its default selection
   contains identity fields only. Missing or null requirements mean **unknown**,
   not zero or “not required”.
4. Use `get_room_occurrences(roomId=...)` and follow every `nextOffset` while
   `hasMore` is true. The tool verifies the Room exists and always enforces its
   `room_id` filter; additional filters can only narrow the list. An existing
   Room with no assignments returns an empty page.
5. Inspect related Items using each occurrence's `article_id` to identify equipment.
   Compare relevant `quantity` values, not occurrence row counts, with the Room's
   requirements. Null quantities remain unknown. Keep `equipment_list_type_id`
   schedules separate; optionally filter to a known schedule with
   `equipmentListTypeId`. Do not double-count alternative schedules or declare
   completeness from filtered or partial results.
6. Report requirement values, assigned quantities, IDs and any gaps/unknowns
   separately. `get_room_history` can help investigate changes.

Room assignment in dRofus does **not** prove physical/BIM placement, geometric
location or code compliance; checking whether everything is modeled in the room
requires external model evidence. To change room requirements, use `update_room`
with verified writable property IDs; to create a room, use `create_room`.
Both share `DROFUS_ENABLE_WRITES` and interactive approval with Item/Occurrence
writes. Updating requirements does not modify assigned occurrences. Occurrence reassignment still
requires the normal write preview/approval and `equipment_list_type_id`.

### Revit parameter mappings across MCP servers

The three attribute configuration tools are read-only and work without
`DROFUS_ENABLE_WRITES`. Agent-facing read/write descriptions and server instructions
proactively direct agents to these tools when comparing dRofus with Revit:

1. Use `list_attribute_configurations`, optionally with ANDed filters on API
   fields such as `config_type` or `name`. Configuration type values include
   `room`, `space`, `article` and `revit-occurrence`—not plural MCP entity names.
   Check `applicable_to`, `available_to_users` and `is_default`, and follow all
   pages. A default does not prove the configuration is active in the Revit model;
   clarify competing configurations before selecting one.
2. Pass its positive ID to `get_attribute_configuration`, or search with
   `find_attribute_mappings(configurationId, property, side)`. `side` is `drofus`,
   `external`, or `either` (default). Lookup prefers exact case-sensitive IDs,
   then exact case-insensitive labels. It does not fuzzy-match or translate IDs.
3. Read the original `drofus_attribute_id`/`drofus_attribute_label`,
   `external_attribute_id`/`external_attribute_label`, `direction`, `configuration`
   and mapping `id`. IDs are opaque: do not assume they are directly usable API
   field names, Revit parameter names, GUIDs or built-in parameter codes.
   Verify the dRofus side against endpoint-specific `resolve_property` and
   `get_field_metadata`, and the Revit side through that MCP server's parameter
   metadata, including type/instance scope and units. Unverified identifiers
   require clarification, not a guessed conversion.
4. Respect the documented direction:
   - `Key`: identifies corresponding objects; it is not a transfer direction.
   - `ToExternalApplication`: dRofus → the configured external application (Revit
     when this is a Revit configuration).
   - `ToDrofus`: external application → dRofus.
   Missing, null or unfamiliar directions remain unchanged and mean unknown.
   They are never defaulted to `Key` or assumed bidirectional.

List pagination applies to configurations. Read/lookup `limit` and `offset`
apply to mapping elements within one configuration (default 25, maximum 100).
The upstream API returns the selected configuration's entire elements array;
mapping lookup and pagination occur locally. Results retain configuration context.
Lookup returns `matched`, `ambiguous`, `not_found`, or `unavailable`, and a
`matchCount` calculated before pagination. Multiple mappings remain ambiguous
even if only one fits on a page. `unavailable` with a null count means elements
were missing/null; an empty array is a known empty set. Read results use
`available` for supplied elements. Follow `nextOffset` while `hasMore` is true.

A mapping describes **intended synchronization**, not proof that values are
synchronized or that objects are matched correctly. It neither grants write
permission nor bypasses either MCP server's approval requirements. Configuration
editing and automatic cross-server synchronization are not provided.

### Write approval and limitations

All five write tools default to `preview=true`, which does not modify dRofus.
For execution, set `preview=false`; the operator must also enable writes in the
server environment using `DROFUS_ENABLE_WRITES=true` (the default is `false`).
There is no separate Room write flag. The server then requests **interactive form elicitation**
from the MCP host, showing the project, operation, target, proposed values and
current selected values for updates. Approval expires after two minutes.
Decline, cancel, missing/false approval, or a host without form elicitation
prevents the write. Use a trusted host that presents the form to the human user,
not one that lets the model automatically approve elicitation requests.

- Item creation accepts only `level_id`, `name`, `bim_id`, `bip`, `note`,
  `parent_id`, `price_reference`, `serial_no` (maximum 10 characters), and
  `to_be_drawn`. Creation requires an already known item group ID.
- Room creation requires a non-empty `name` (maximum 500 characters). Optional
  fields from the API creation schema are `architect_no`, `description`,
  `designed_area`, `drawing_name`, `drawing_no`, `note`, `programmed_area`,
  `room_function_id` (positive when supplied), and `user_room_no`.
  Additional verified writable requirements can be set with `update_room`
  afterward, using a separate preview/approval. Room deletion and room-specific
  status/group/template operations are not exposed.
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
  and no automatic write retry; retrying item or room creation can create duplicates.
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
