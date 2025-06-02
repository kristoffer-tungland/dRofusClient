# dRofusClient

C# library for interacting with dRofus API

## Supported Frameworks

- .NET 8
- .NET Standard 2.0

## Features

- Simple, strongly-typed API client for dRofus
- Project listing and basic data retrieval
- Designed for extensibility

## Installation

Install via NuGet:dotnet add package dRofusClientOr via the NuGet Package Manager:Install-Package dRofusClient
## Usage

### Basic usage with `dRofusClientFactory`using dRofusClient;
using dRofusClient.Factory;
using System;

namespace dRofusClientExample
{
	class Program
	{
		static void Main(string[] args)
		{
			var factory = new dRofusClientFactory("https://api.drofus.com", "username", "password");
			var client = factory.CreateClient();
			var projects = client.GetProjects();
			foreach (var project in projects)
			{
				Console.WriteLine(project.Name);
			}
		}
	}
}
## Contributing

Contributions are welcome! Please open issues or submit pull requests.

## License

[MIT](LICENSE)
