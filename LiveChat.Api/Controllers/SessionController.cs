using LiveChat.Business.Models;
using LiveChat.Business.Models.RESTRequests;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Constants.Enums;
using LiveChat.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace LiveChat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;
        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpPost("start-session")]
        public IActionResult StartSession(ClientModel newClient)
        {
            try
            {
                return Ok(new { sessionToken = _sessionService.GenerateJwt(_sessionService.StartSession(newClient)) });
            }
            catch (Exception ex)
            {
                return Conflict(new BadHttpRequestException("Something went wrong"));
            }
        }

        [HttpPost("stop-session")]
        public IActionResult DisconnectClient()
        {
            try
            {
                var id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (_sessionService.SessionIsActive(id))
                {
                    _sessionService.DisconnectClient(id);
                    return Ok();
                }
                else
                    return Conflict(new BadHttpRequestException("Session with this id isn`t acive"));
            }
            catch (Exception)
            {
                return Conflict(new BadHttpRequestException("Session with this id isn`t acive"));
            }
            
        }

        [HttpGet("session-status/{sessionId}")]
        public IActionResult IsSessionActive(string sessionId)
        {
            try
            {
                return Ok(new { IsSessionActive = _sessionService.SessionIsActive(Guid.Parse(sessionId)) });
            }
            catch (Exception ex)
            {
                return Conflict(new BadHttpRequestException("Something went wrong"));
            }
        }

        [HttpPost("agent-online")]
        [Authorize(Roles = Roles.Agent)]
        public IActionResult AddAgent()
        {
            var id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            AgentModel agent = new AgentModel(id);
            if (!_sessionService.IsAgentOnline(agent))
            {
                _sessionService.AddAgentOnline(agent);
                return Ok();
            }
            else
                return Conflict(new BadHttpRequestException("This agent is already online"));
        }

        [HttpPost("agent-offline")]
        [Authorize(Roles = Roles.Agent)]
        public IActionResult RemoveAgent()
        {
            var id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            AgentModel agent = new AgentModel(id);
            if (_sessionService.IsAgentOnline(agent))
            {
                _sessionService.RemoveAgentOnline(agent);
                return Ok();
            }
            else
                return Conflict(new BadHttpRequestException("This agent isn`t online"));

        }
        [HttpGet("test")]
        public void RemoveAsgent(ClientModel client)
        {
           

        }
    }
}
