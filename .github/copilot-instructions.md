---
applyTo: '**/*ViewModel.cs'
---
For view models we use a fluent MVVM framework to implement the MVVM pattern. 
The view model should inherit from `ViewModelBase`.

Here’s a concise summary of how to use the MVVMFluent library for view models:

---

**MVVMFluent Usage Summary**

- **Base Class:**  
  All view models should inherit from `ViewModelBase` (or `ValidationViewModelBase` for validation support).

- **Fluent Property Setters:**  
  Use `Get<T>()` and `Set(value)` for simple properties.  
  For advanced scenarios, use `When(value)` to fluently chain actions like `.Changing()`, `.Changed()`, `.Notify()`, `.Validate()`, and `.Set()`.

- **Commands:**  
  Define commands using `FluentCommand` or `FluentCommand<T>`.  
  Use `Do()` to specify the action, and `If()` or `IfValid()` for conditional execution.  
  For async operations, use `AsyncFluentCommand` or `AsyncFluentCommand<T>` with support for cancellation, progress, and error handling.

- **Validation:**  
  Add validation to property setters with `.Validate()` or `.Required()`.  
  Use `IfValid(nameof(Property))` on commands to ensure they only execute when input is valid.

- **Property Change Notification:**  
  Use `.Notify(nameof(OtherProperty))` to trigger change notifications for dependent properties.

- **Default Values:**  
  Use `Get(defaultValue)` to provide a default value for a property.

- **Disposal:**  
  Call `Dispose()` on the view model to clean up resources when it is no longer needed.

- **Window/Dialog Management:**  
  Implement `IClosableViewModel` for close logic and `IResultViewModel<TResult>` for dialogs that return results.  
  Use `RequestCloseView` to request window closure.

**Example:**
```csharp
public class MyViewModel : ViewModelBase
{
    public string Name
    {
        get => Get<string>();
        set => When(value)
                .Changing(v => /* before change */)
                .Changed(v => /* after change */)
                .Notify(SaveCommand)
                .Set();
    }

    public FluentCommand SaveCommand => Do(Save).If(() => !string.IsNullOrEmpty(Name));
    private void Save() { /* save logic */ }
}
```

MVVMFluent streamlines MVVM development by reducing boilerplate and enabling fluent, readable property and command definitions.

---