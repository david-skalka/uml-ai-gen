using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TodoApp.Api;

public class JHipsterResponseHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
            response.StatusCode = HttpStatusCode.OK;
        if (response.StatusCode == HttpStatusCode.NoContent)
            response.StatusCode = HttpStatusCode.OK;

        return response;
    }
}
