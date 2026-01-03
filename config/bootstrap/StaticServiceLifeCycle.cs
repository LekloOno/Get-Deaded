using System.Threading.Tasks;

public static class StaticServiceLifeCycle<T>
{
    private static readonly TaskCompletionSource _tcs = new();

    public static Task Initialized => _tcs.Task;

    public static void MarkInitialized()
        => _tcs.TrySetResult();
}