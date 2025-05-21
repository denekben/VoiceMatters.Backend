using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoiceMatters.Application.UseCases.Identity.Commands;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.WebUI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthenticationController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokensDto?>> RegisterNewUser([FromForm] RegisterNewUser command)
        {
            var tokens = await _sender.Send(command);
            return Ok(tokens);
        }

        [HttpPost("signin")]
        public async Task<ActionResult<TokensDto?>> SignIn([FromBody] SignIn command)
        {
            var tokens = await _sender.Send(command);
            return Ok(tokens);
        }

        [HttpGet]
        [Route("access-token")]
        public async Task<ActionResult<string?>> RefreshExpiredToken([FromQuery] RefreshExpiredToken command)
        {
            var result = await _sender.Send(command);
            if (result == null)
                return Unauthorized();
            return Ok(result);
        }
    }

}
