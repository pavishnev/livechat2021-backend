using LiveChat.Business.Models.RESTResponces;
using LiveChat.Business.Services;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Constants.Enums;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories;
using LiveChat.Data.Repositories.Interfaces;
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
    public class ChatLogController : ControllerBase
    {
        private readonly IChatLogService _chatLogService;

        public ChatLogController(
            IChatLogService chatLogService)
        {
            _chatLogService = chatLogService;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("session-history/{pageIndex}/{pageSize}")]
        public IActionResult SessionHistory(int pageIndex, int pageSize)
        {
            try
            {
                string websiteId = User.FindAll("websiteId").Select(x => x.Value).FirstOrDefault();

                return Ok(_chatLogService
                    .GetSessionsPageByPage(
                        Guid.Parse(websiteId),
                        pageIndex,
                        pageSize
                    ));
            }
            catch (Exception ex)
            {
                return Conflict(new BadHttpRequestException(ex.ToString()));
            }
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("get-sessions-count")]
        public IActionResult SessionCount()
        {
            try
            {
                string websiteId = User.FindAll("websiteId").Select(x => x.Value).FirstOrDefault();

                return Ok( _chatLogService
                    .GetSessionsCount(new Guid(websiteId)));
            }
            catch (Exception ex)
            {
                return Conflict(new BadHttpRequestException(ex.ToString()));
            }
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("chat-history/{sessionId}")]
        public IActionResult ChatHistory(string sessionId)
        {
            try
            {
                return Ok(_chatLogService.GetChatLog(
                        Guid.Parse(sessionId)
                    ));
            }
            catch (Exception ex)
            {
                return Conflict(new BadHttpRequestException(ex.ToString()));
            }
        }
    }
}
