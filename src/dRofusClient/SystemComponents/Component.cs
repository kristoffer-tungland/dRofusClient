namespace dRofusClient.SystemComponents;

/// <summary>
/// Represents a component belonging to a system.
/// </summary>
public record Component : Occurrences.Occurence
{
    /// <summary>
    /// Returns a copy of this instance with read-only fields cleared.
    /// </summary>
    public new Component ClearReadOnlyFields()
        => (Component)base.ClearReadOnlyFields();
}
