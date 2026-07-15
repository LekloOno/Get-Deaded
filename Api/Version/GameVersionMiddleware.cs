namespace Api.Version;

public class GameVersionMiddleware
{
    private readonly RequestDelegate _next;


    public GameVersionMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task InvokeAsync(
        HttpContext httpContext,
        IGameVersionResolver resolver,
        GameVersionContext context)
    {
        if (!httpContext.Request.Headers.TryGetValue(
            "X-Game-Version",
            out var versionHeader))
        {
            await _next(httpContext);
            return;
        }

        var version = await resolver.ResolveAsync(versionHeader!);

        if (version is null)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync(
                "Unknown game version");
            return;
        }

        context.Set(version);


        await _next(httpContext);
    }
}