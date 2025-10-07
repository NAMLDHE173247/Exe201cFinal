using StyleGenie.Application.Dto.TryOn;

namespace StyleGenie.Application.TryOn;

public interface ITryOnService
{
    Task<TryOnResultDto> TryOnAsync(TryOnRequestDto req, CancellationToken ct = default);
}
