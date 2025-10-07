namespace StyleGenie.Application.TryOn;

public interface ITryOnProvider
{
    Task<(string TaskId, Func<CancellationToken, Task<byte[]>> Downloader)>
        CreateAndPollAsync(long tenantId,
                           string clothType, bool hdMode,
                           byte[] model, byte[] cloth, byte[]? lower,
                           CancellationToken ct);
}
