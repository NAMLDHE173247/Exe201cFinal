namespace StyleGenie.Application.Images;

public interface IImageReadService
{
    Task<(string ContentType, byte[] Bytes)?> GetAsync(long imageId, CancellationToken ct = default);
}
