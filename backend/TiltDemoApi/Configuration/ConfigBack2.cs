using System.ComponentModel.DataAnnotations;

namespace TiltDemoApi.Configuration;

public class ConfigBack2
{
    public const string Key = "Host:Back2";
    
    [Required]
    public string Url { get; set; }
    public string Endpoint { get; set; }
    [Required]
    public int Timeout { get; set; }
}