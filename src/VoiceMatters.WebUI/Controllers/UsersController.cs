using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceMatters.Application.UseCases.Petitions.Queries;
using VoiceMatters.Application.UseCases.Users.Queries;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.WebUI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("me")]
        [Authorize(Policy = "IsNotBlocked")]
        public async Task<ActionResult<ProfileDto?>> GetCurrentUser()
        {
            var profile = await _sender.Send(new GetCurrentUser());
            if (profile == null)
                return NotFound();
            return Ok(profile);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileDto?>> GetUser([FromRoute] Guid id)
        {
            var profile = await _sender.Send(new GetUser(id));
            if (profile == null)
                return NotFound();
            return Ok(profile);
        }

        [HttpGet]
        public async Task<ActionResult<List<ProfilePlateDto>?>> GetUserPlates(
            [FromQuery] GetUserPlates query)
        {
            var plates = await _sender.Send(query);
            if (plates == null)
                return NotFound();
            return Ok(plates);
        }

        [HttpGet("petitions")]
        public async Task<ActionResult<List<PetitionDto>?>> GetPetitionsByUserId(
            [FromQuery] GetPetitionsByUserId query)
        {
            var result = await _sender.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
