using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using StyleGenie.Application.Images;

namespace StyleGenie.Api.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private readonly IImageReadService _images;
    public ImagesController(IImageReadService images) => _images = images;

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Lấy ảnh theo Id")]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        var res = await _images.GetAsync(id, ct);
        if (res == null) return NotFound();
        return File(res.Value.Bytes, res.Value.ContentType);
    }
}
