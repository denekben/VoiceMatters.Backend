using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoiceMatters.Application.UseCases.Statistic.Queries;
using VoiceMatters.Domain.Entities;

namespace VoiceMatters.WebUI.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatisticController : ControllerBase
    {
        private readonly ISender _sender;

        public StatisticController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<Statistic?>> GetStatistic()
        {
            var statistic = await _sender.Send(new GetStatistic());
            if (statistic == null)
                return NotFound();
            return Ok(statistic);
        }
    }
}
