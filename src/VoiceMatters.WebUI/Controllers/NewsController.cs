using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceMatters.Application.UseCases.News.Commands;
using VoiceMatters.Application.UseCases.News.Queries;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.WebUI.Controllers
{
    [ApiController]
    [Route("api/news")]
    public class NewsController : ControllerBase
    {
        private readonly ISender _sender;

        public NewsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Policy = "IsNotBlocked")]
        public async Task<ActionResult<NewsDto?>> CreateNews([FromBody] CreateNews command)
        {
            var news = await _sender.Send(command);
            return Ok(news);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsNotBlocked")]
        public async Task<IActionResult> DeleteNews([FromRoute] Guid id)
        {
            await _sender.Send(new DeleteNews(id));
            return NoContent();
        }

        [HttpPut]
        [Authorize(Policy = "IsNotBlocked")]
        public async Task<ActionResult<NewsDto?>> UpdateNews([FromBody] UpdateNews command)
        {
            var news = await _sender.Send(command);
            return Ok(news);
        }

        [HttpGet]
        public async Task<ActionResult<List<NewsDto>?>> GetNews([FromQuery] GetNews query)
        {
            var news = await _sender.Send(query);
            if (news == null)
                return NotFound();
            return Ok(news);
        }
    }
}
