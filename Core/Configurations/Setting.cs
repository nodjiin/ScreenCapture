using System.ComponentModel.DataAnnotations;

namespace Core.Configurations;

public class Setting
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Any setting must have a well defined default value.")]
    public string DefaultValue { get; set; } = string.Empty;
    public string[]? PossibleValues { get; set; }
    public SettingType Type { get; set; }
}

public enum SettingType
{
    /// <summary>
    /// Define a setting with "open value", e.g. where the user can insert a value of his choice.
    /// </summary>
    Open,

    /// <summary>
    /// Define a setting with a fixed amount of possible values, e.g. where the user has to choose a value from within PossibleValues
    /// </summary>
    Fixed,

    /// <summary>
    /// Define a setting with only 2 possible values: true or false. 
    /// </summary>
    Boolean
}