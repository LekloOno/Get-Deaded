using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Api;

public class GameVersionHeaderHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Remove("X-Game-Version");

        request.Headers.Add(
            "X-Game-Version",
            ApiConfig.GameVersion);

        return base.SendAsync(request, cancellationToken);
    }
}