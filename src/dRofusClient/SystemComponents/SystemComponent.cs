namespace dRofusClient.SystemComponents;

/// <summary>
/// Represents a system component.
/// </summary>
public record SystemComponent : Occurrences.Occurence
{
    /// <summary>
    /// Returns a copy of this instance with read-only fields cleared.
    /// </summary>
    public new SystemComponent ClearReadOnlyFields()
        => (SystemComponent)base.ClearReadOnlyFields();
}
