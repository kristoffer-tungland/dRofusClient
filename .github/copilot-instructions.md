---
applyTo: '**/dRofusClient*Extensions.cs'
---
# Extension Methods Sync Pattern Guidance

**dRofusClient*Extensions Sync Method Policy**

- For every new public extension method added to any `dRofusClient*Extensions` class (e.g., `dRofusClientOccurenceExtensions`), a synchronous wrapper method **must** be added to the corresponding `dRofusClient.Revit` project (e.g., `dRofusClientRevitOccurenceExtensions`).
- The sync wrapper should use `AsyncUtil.RunSync` to call the async method and provide the same parameters and return type (converted to sync where appropriate).
- The sync wrapper **must** include XML documentation that matches the async method's documentation, with wording adapted for synchronous context.
- This ensures that all dRofusClient extension APIs are available in both async and sync forms for Revit integration and scripting scenarios.
- When adding a new extension method, always check for and implement the corresponding sync wrapper in the Revit project.