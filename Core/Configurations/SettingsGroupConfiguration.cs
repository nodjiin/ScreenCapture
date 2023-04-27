using System.ComponentModel.DataAnnotations;

namespace Core.Configurations;

public class SettingsGroupConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "A settings group must contain at least a configured setting.")]
    public List<Setting> Settings { get; set; } = new List<Setting>();
}
