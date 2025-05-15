# dRofusClient

C# library for interacting with dRofus API

## Usage
```csharp
using dRofusClient;
using System;

namespace dRofusClientExample
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new dRofusClient("https://api.drofus.com", "username", "password");
			var projects = client.GetProjects();
			foreach (var project in projects)
			{
				Console.WriteLine(project.Name);
			}
		}
	}
}
```
