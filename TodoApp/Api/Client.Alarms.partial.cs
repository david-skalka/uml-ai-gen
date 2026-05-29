using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoApp.Api;

public partial interface IClient
{
    Task<ICollection<Alarm>> AlarmsGetAllAsync();

    Task<ICollection<Alarm>> AlarmsGetAllAsync(CancellationToken cancellationToken);

    Task<Alarm> AlarmsCreateAsync(CreateAlarmRequest body);

    Task<Alarm> AlarmsCreateAsync(CreateAlarmRequest body, CancellationToken cancellationToken);

    Task<Alarm> AlarmsGetByIdAsync(int id);

    Task<Alarm> AlarmsGetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Alarm> AlarmsUpdateAsync(int id, UpdateAlarmRequest body);

    Task<Alarm> AlarmsUpdateAsync(int id, UpdateAlarmRequest body, CancellationToken cancellationToken);

    Task AlarmsDeleteAsync(int id);

    Task AlarmsDeleteAsync(int id, CancellationToken cancellationToken);
}

public partial class Client
{
    public Task<ICollection<Alarm>> AlarmsGetAllAsync() =>
        AlarmsGetAllAsync(CancellationToken.None);

    public async Task<ICollection<Alarm>> AlarmsGetAllAsync(CancellationToken cancellationToken)
    {
        using var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, "api/alarms");
        request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

        var urlBuilder = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(_baseUrl)) urlBuilder.Append(_baseUrl);
        urlBuilder.Append("api/alarms");

        PrepareRequest(_httpClient, request, urlBuilder);
        request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.RelativeOrAbsolute);
        PrepareRequest(_httpClient, request, urlBuilder.ToString());

        var response = await _httpClient.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var headers = ToHeaders(response);
        ProcessResponse(_httpClient, response);

        if ((int)response.StatusCode == 200)
        {
            var objectResponse = await ReadObjectResponseAsync<ICollection<Alarm>>(response, headers, cancellationToken);
            if (objectResponse.Object == null)
                throw new ApiException("Response was null which was not expected.", (int)response.StatusCode, objectResponse.Text, headers, null);

            return objectResponse.Object;
        }

        throw await CreateUnexpectedResponseAsync(response, headers, cancellationToken);
    }

    public Task<Alarm> AlarmsCreateAsync(CreateAlarmRequest body) =>
        AlarmsCreateAsync(body, CancellationToken.None);

    public async Task<Alarm> AlarmsCreateAsync(CreateAlarmRequest body, CancellationToken cancellationToken)
    {
        using var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "api/alarms");
        var json = JsonConvert.SerializeObject(body, JsonSerializerSettings);
        var content = new System.Net.Http.StringContent(json);
        content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
        request.Content = content;
        request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

        var urlBuilder = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(_baseUrl)) urlBuilder.Append(_baseUrl);
        urlBuilder.Append("api/alarms");

        PrepareRequest(_httpClient, request, urlBuilder);
        request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.RelativeOrAbsolute);
        PrepareRequest(_httpClient, request, urlBuilder.ToString());

        var response = await _httpClient.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var headers = ToHeaders(response);
        ProcessResponse(_httpClient, response);

        if ((int)response.StatusCode == 200)
        {
            var objectResponse = await ReadObjectResponseAsync<Alarm>(response, headers, cancellationToken);
            if (objectResponse.Object == null)
                throw new ApiException("Response was null which was not expected.", (int)response.StatusCode, objectResponse.Text, headers, null);

            return objectResponse.Object;
        }

        throw await CreateUnexpectedResponseAsync(response, headers, cancellationToken);
    }

    public Task<Alarm> AlarmsGetByIdAsync(int id) =>
        AlarmsGetByIdAsync(id, CancellationToken.None);

    public async Task<Alarm> AlarmsGetByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, "api/alarms");
        request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

        var urlBuilder = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(_baseUrl)) urlBuilder.Append(_baseUrl);
        urlBuilder.Append("api/alarms/");
        urlBuilder.Append(Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

        PrepareRequest(_httpClient, request, urlBuilder);
        request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.RelativeOrAbsolute);
        PrepareRequest(_httpClient, request, urlBuilder.ToString());

        var response = await _httpClient.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var headers = ToHeaders(response);
        ProcessResponse(_httpClient, response);

        if ((int)response.StatusCode == 200)
        {
            var objectResponse = await ReadObjectResponseAsync<Alarm>(response, headers, cancellationToken);
            if (objectResponse.Object == null)
                throw new ApiException("Response was null which was not expected.", (int)response.StatusCode, objectResponse.Text, headers, null);

            return objectResponse.Object;
        }

        throw await CreateUnexpectedResponseAsync(response, headers, cancellationToken);
    }

    public Task<Alarm> AlarmsUpdateAsync(int id, UpdateAlarmRequest body) =>
        AlarmsUpdateAsync(id, body, CancellationToken.None);

    public async Task<Alarm> AlarmsUpdateAsync(int id, UpdateAlarmRequest body, CancellationToken cancellationToken)
    {
        using var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Put, "api/alarms");
        var json = JsonConvert.SerializeObject(body, JsonSerializerSettings);
        var content = new System.Net.Http.StringContent(json);
        content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
        request.Content = content;
        request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

        var urlBuilder = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(_baseUrl)) urlBuilder.Append(_baseUrl);
        urlBuilder.Append("api/alarms/");
        urlBuilder.Append(Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

        PrepareRequest(_httpClient, request, urlBuilder);
        request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.RelativeOrAbsolute);
        PrepareRequest(_httpClient, request, urlBuilder.ToString());

        var response = await _httpClient.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var headers = ToHeaders(response);
        ProcessResponse(_httpClient, response);

        if ((int)response.StatusCode == 200)
        {
            var objectResponse = await ReadObjectResponseAsync<Alarm>(response, headers, cancellationToken);
            if (objectResponse.Object == null)
                throw new ApiException("Response was null which was not expected.", (int)response.StatusCode, objectResponse.Text, headers, null);

            return objectResponse.Object;
        }

        throw await CreateUnexpectedResponseAsync(response, headers, cancellationToken);
    }

    public Task AlarmsDeleteAsync(int id) =>
        AlarmsDeleteAsync(id, CancellationToken.None);

    public async Task AlarmsDeleteAsync(int id, CancellationToken cancellationToken)
    {
        using var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Delete, "api/alarms");
        var urlBuilder = new System.Text.StringBuilder();
        if (!string.IsNullOrEmpty(_baseUrl)) urlBuilder.Append(_baseUrl);
        urlBuilder.Append("api/alarms/");
        urlBuilder.Append(Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

        PrepareRequest(_httpClient, request, urlBuilder);
        request.RequestUri = new Uri(urlBuilder.ToString(), UriKind.RelativeOrAbsolute);
        PrepareRequest(_httpClient, request, urlBuilder.ToString());

        var response = await _httpClient.SendAsync(request, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var headers = ToHeaders(response);
        ProcessResponse(_httpClient, response);

        if ((int)response.StatusCode == 200 || (int)response.StatusCode == 204)
            return;

        throw await CreateUnexpectedResponseAsync(response, headers, cancellationToken);
    }

    private static Dictionary<string, IEnumerable<string>> ToHeaders(System.Net.Http.HttpResponseMessage response)
    {
        var headers = new Dictionary<string, IEnumerable<string>>();
        foreach (var item in response.Headers)
            headers[item.Key] = item.Value;

        if (response.Content?.Headers != null)
        {
            foreach (var item in response.Content.Headers)
                headers[item.Key] = item.Value;
        }

        return headers;
    }

    private static async Task<ApiException> CreateUnexpectedResponseAsync(
        System.Net.Http.HttpResponseMessage response,
        Dictionary<string, IEnumerable<string>> headers,
        CancellationToken cancellationToken)
    {
        var responseData = response.Content == null
            ? null
            : await response.Content.ReadAsStringAsync(cancellationToken);
        return new ApiException(
            "The HTTP status code of the response was not expected (" + (int)response.StatusCode + ").",
            (int)response.StatusCode,
            responseData,
            headers,
            null);
    }
}

public class Alarm
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("time")]
    public DateTimeOffset? Time { get; set; }
}

public class CreateAlarmRequest
{
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("time")]
    public DateTimeOffset Time { get; set; }
}

public class UpdateAlarmRequest
{
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("time")]
    public DateTimeOffset Time { get; set; }
}
