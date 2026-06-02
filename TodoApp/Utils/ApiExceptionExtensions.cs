using Newtonsoft.Json.Linq;
using TodoApp.Api;

namespace TodoApp.Utils;

public static class ApiExceptionExtensions
{
    public static IReadOnlyList<string> GetValidationErrors(this ApiException exception)
    {
        var json = JObject.Parse(exception.Response!);
        var errorsToken = json["errors"];

        if (errorsToken is JObject errorsObject)
        {
            return errorsObject.Properties()
                .SelectMany(prop => prop.Value.Select(token => token.ToString()))
                .ToList();
        }

        return [json["title"]!.ToString()];
    }

    public static string FormatValidationErrors(this ApiException exception)
    {
        return string.Join('\n', exception.GetValidationErrors());
    }
}
