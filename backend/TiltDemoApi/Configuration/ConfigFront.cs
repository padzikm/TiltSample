using System.ComponentModel.DataAnnotations;

namespace TiltDemoApi.Configuration;

public class ConfigFront
{
    public const string Key = "Host:Front";
    
    [Required]
    public string Url { get; set; }
    
    [Required]
    public string Endpoint { get; set; }
    
    [Required]
    public int? SomeNumber { get; set; }
}