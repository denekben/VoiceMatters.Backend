using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceMatters.Application.UseCases.Petitions.Commands;
using VoiceMatters.Application.UseCases.Petitions.Queries;
using VoiceMatters.Application.UseCases.Users.Queries;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.WebUI.Controllers
{
    [ApiController]
    [Route("api/petitions")]
    public class PetitionController : ControllerBase
    {
        private readonly ISender _sender;

        public PetitionController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("{id}/complete")]
        [Authorize(Policy = "IsNotBlocked")]
        public async Task<IActionResult> CompletePetition([FromRoute] Guid id)
        {
            await _sender.Send(new CompletePetition(id));
            return NoContent();
        }

        [HttpPost]
        [Authorize(Policy = "IsNotBlocked")]
        public async Task<ActionResult<PetitionDto?>> CreatePetition([FromForm] CreatePetition command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsNotBlocked")]
        public async Task<IActionResult> DeletePetition([FromRoute] Guid id)
        {
            await _sender.Send(new DeletePetition(id));
            return NoContent();
        }

        [HttpPost("{id}/sign")]
        [Authorize(Policy = "IsNotBlocked")]
        public async Task<IActionResult> SignPetition([FromRoute] Guid id)
        {
            await _sender.Send(new SignPetition(id));
            return NoContent();
        }

        [HttpPut]
        [Authorize(Policy = "IsNotBlocked")]
        public async Task<ActionResult<PetitionDto?>> UpdatePetition([FromForm] UpdatePetition command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize(Policy = "IsNotBlocked")]
        public async Task<ActionResult<List<PetitionDto>?>> GetCurrentUserPetitions(
            [FromQuery] GetCurrentUserPetitions query)
        {
            var result = await _sender.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetitionDto?>> GetPetition([FromRoute] Guid id)
        {
            var result = await _sender.Send(new GetPetition(id));
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<PetitionDto>?>> GetPetitions(
            [FromQuery] GetPetitions query)
        {
            var result = await _sender.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<ProfilePlateDto>?>> GetUserPlatesByPetitionId(
            [FromQuery] GetUserPlatesByPetitionId query)
        {
            var plates = await _sender.Send(query);
            if (plates == null)
                return NotFound();
            return Ok(plates);
        }
    }
}
