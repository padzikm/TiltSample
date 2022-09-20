using System.ComponentModel.DataAnnotations;

namespace TiltDemoApi.Configuration;

public class ConfigHost
{
    public const string Key = "Host:Self";
    
    public string Url { get; set; }
    public string Endpoint { get; set; }
    public int Level { get; set; }
}