using FaceLiveness.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FaceLiveness.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecognitionController : ControllerBase
    {
        private readonly IFaceClient _faceClient;

        public RecognitionController(IFaceClient faceClient)
        {
            _faceClient = faceClient;
        }

        [HttpPost("recognize")]
        public async Task<IActionResult> Recognize([FromBody] ImageRequest req)
        {
            if (string.IsNullOrEmpty(req.Image))
                return BadRequest(new { error = "image required" });

            try
            {
                var result = await _faceClient.RecognizeAsync(req.Image, req.Multi);
                return Ok(result);
            }
            catch (HttpRequestException hx)
            {
                return StatusCode(502, new { error = "Vision service error", detail = hx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public class ImageRequest
        {
            public string Image { get; set; } = default!;
            public bool Multi { get; set; } = true;
        }
    }
}
