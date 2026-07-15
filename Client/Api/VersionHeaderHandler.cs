using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Client.Api;

public class GameVersionHeaderHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Remove("X-Game-Version");

        var version =
            Assembly.GetExecutingAssembly()
                .GetName()
                .Version!;

        var versionKey = $"{version.Major}.{version.Minor}.{version.Build}";

        GD.Print(version);

        request.Headers.Add(
            "X-Game-Version",
            versionKey);

        return base.SendAsync(request, cancellationToken);
    }
}