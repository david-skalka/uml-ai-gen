using CommandLine;

namespace TodoApp.Infrastructure;

public class AppOptions
{
    [Option('u', "url", Required = false, HelpText = "Base URL for the API.")]
    public string ApiUrl { get; set; } = "http://localhost:5000";

}
