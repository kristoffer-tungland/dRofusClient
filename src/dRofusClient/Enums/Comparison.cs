using System.ComponentModel;

namespace dRofusClient.Enums;

public enum Comparison
{
    /// <summary>
    /// Equality comparison.
    /// </summary>
    [Description("Equals")]
    Eq, // Equals

    /// <summary>
    /// Not equal comparison.
    /// </summary>
    [Description("Not equal")]
    Ne, // Not equal

    /// <summary>
    /// Less than comparison.
    /// </summary>
    [Description("Less than")]
    Lt, // Less than

    /// <summary>
    /// Greater than comparison.
    /// </summary>
    [Description("Greater than")]
    Gt, // Greater than

    /// <summary>
    /// Less than or equal comparison.
    /// </summary>
    [Description("Less than or equal")]
    Le, // Less than or equal

    /// <summary>
    /// Greater than or equal comparison.
    /// </summary>
    [Description("Greater than or equal")]
    Ge, // Greater than or equal

    /// <summary>
    /// Membership check.
    /// </summary>
    [Description("Is a member of")]
    In, // Is a member of

    /// <summary>
    /// String contains a sub-string comparison.
    /// </summary>
    [Description("Contains")]
    Contains,

    /// <summary>
    /// String starts with a specified substring comparison.
    /// </summary>
    [Description("Starts with")]
    StartsWith,

    /// <summary>
    /// String ends with a specified substring comparison.
    /// </summary>
    [Description("Ends with")]
    EndsWith
}
