using System.ComponentModel.DataAnnotations;

namespace CaptureAgent.Configurations;
public class FtpServerConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public string Ip { get; set; } = string.Empty;

    [Range(0, 65535)]
    public int Port { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string UserName { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string CertificatePath { get; set; } = string.Empty;
}

