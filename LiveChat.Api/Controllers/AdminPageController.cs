using LiveChat.Business.Models.RESTRequests;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Constants.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveChat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPageController : ControllerBase
    {

        private readonly IAgentService _agentService;
        private readonly IUserService _userService;

        public AdminPageController(IAgentService agentService, IUserService userService)
        {
            _agentService = agentService;
            _userService = userService;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("get-all-agents")]
        public IActionResult GetAllAgents()
        {
            var websiteUrl = User.FindAll("websiteUrl").Select(x => x.Value).FirstOrDefault();
            var website = _agentService.GetWebsiteByUrl(websiteUrl);
            var availableAgents = _agentService.GetAllAgents(website.Id);
            return Ok(availableAgents);

        }
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("get-agent-credentials")]
        public IActionResult GetAgentCredentials(Guid id)
        {
            var availableAgent = _agentService.GetAgentById(id);
            if (availableAgent == null) return Conflict("No user with such id");
            else return Ok(availableAgent);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("delete-agent/{id}")]
        public IActionResult DeleteAgent(Guid id)
        {
            var availableAgent = _agentService.GetAgentById(id);
            if (availableAgent == null) return Conflict($"No user with such id : {id}");
            else
            {
                _agentService.DeleteAgent(id);
                return Ok();
            }

        }

    }
}
