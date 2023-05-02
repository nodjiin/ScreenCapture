using System.ComponentModel.DataAnnotations;

namespace Core.Configurations;

public class SettingsGroupConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    public Dictionary<string, Setting> Settings { get; set; } = new Dictionary<string, Setting>();
}
