using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoiceMatters.Application.UseCases.Tags.Queries;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.WebUI.Controllers
{
    [ApiController]
    [Route("api/tags")]
    public class TagsController : ControllerBase
    {
        private readonly ISender _sender;

        public TagsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<List<TagDto>?>> GetTags(
            [FromQuery] GetTags query)
        {
            var tags = await _sender.Send(query);
            if (tags == null)
                return NotFound();
            return Ok(tags);
        }
    }
}
