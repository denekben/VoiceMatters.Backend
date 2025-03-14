using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceMatters.Application.UseCases.Administration.Commands;

namespace VoiceMatters.WebUI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin", Policy = "IsNotBlocked")]
    public class AdministrationController : ControllerBase
    {
        private readonly ISender _sender;

        public AdministrationController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPut("assign-user-to-role")]
        public async Task<IActionResult> AssignUserToRole([FromQuery] AssignUserToRole command)
        {
            await _sender.Send(command);
            return Ok();
        }

        [HttpPut("block-petition")]
        public async Task<IActionResult> BlockPetition([FromQuery] BlockPetition command)
        {
            await _sender.Send(command);
            return Ok();
        }

        [HttpPut("block-user")]
        public async Task<IActionResult> BlockUser([FromQuery] BlockUser command)
        {
            await _sender.Send(command);
            return Ok();
        }

        [HttpPut("unblock-petition")]
        public async Task<IActionResult> UnblockPetition([FromQuery] UnblockPetition command)
        {
            await _sender.Send(command);
            return Ok();
        }

        [HttpPut("unblock-user")]
        public async Task<IActionResult> UnblockUser([FromQuery] UnblockUser command)
        {
            await _sender.Send(command);
            return Ok();
        }
    }
}
