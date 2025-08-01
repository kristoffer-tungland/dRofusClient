# Extension Methods Sync Pattern Guidance

**dRofusClient*Extensions Sync Method Policy**

- For every new public extension method added to any `dRofusClient*Extensions` class (e.g., `dRofusClientOccurenceExtensions`), a synchronous wrapper method **must** be added to the corresponding `dRofusClient.Revit` project (e.g., `dRofusClientRevitOccurenceExtensions`).
- The sync wrapper should use `AsyncUtil.RunSync` to call the async method and provide the same parameters and return type (converted to sync where appropriate).
- The sync wrapper **must** include XML documentation that matches the async method's documentation, with wording adapted for synchronous context.
- This ensures that all dRofusClient extension APIs are available in both async and sync forms for Revit integration and scripting scenarios.
- When adding a new extension method, always check for and implement the corresponding sync wrapper in the Revit project.

---

# dRofus*Dto Class Implementation Guidance

**dRofus*Dto XML Documentation and Read-Only Policy**

- Any class that derives from a base class matching `dRofus*Dto` (e.g., `dRofusIdDto`, `dRofusDto`) **must** have XML documentation for the class and all its properties.
- This requirement applies equally to record types.
- The XML documentation should be based on the property and class descriptions found in the OpenAPI schema (swagger.json). Use the OpenAPI description for each property and the class summary.
- Properties that are marked as `readOnly` in the OpenAPI schema **must** use `init` accessors instead of `set`.
- Properties that are **not** marked as `readOnly` should expose a standard `set` accessor so that values can be modified after construction.
- Classes with read-only properties **must** implement a `ClearReadOnlyFields` method, following the pattern in `Occurence.ClearReadOnlyFields`, which returns a copy of the object with all read-only properties set to null.
- This ensures consistency with the API contract and supports scenarios where read-only fields need to be cleared before updates.
