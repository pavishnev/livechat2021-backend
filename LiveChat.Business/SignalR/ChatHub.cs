using LiveChat.Business.Models;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiveChat.Business.SignalR
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private readonly ISessionService _sessionService;
        private readonly ISessionRepository _sessionRepository;
        private readonly IChatLogRepository _chatLogRepository;
        private readonly IUserRepository _userRepository;
        private static readonly List<ChatModel> _chats = new();
        public ChatHub(ISessionService sessionService, ISessionRepository repository, IChatLogRepository chatLogRepository, IUserRepository userRepository)
        {
            _sessionService = sessionService;
            _sessionRepository = repository;
            _chatLogRepository = chatLogRepository;
            _userRepository = userRepository;
        }
        public async Task BroadcastChatData(List<MessageModel> data) { await Clients.All.SendAsync("broadcastchatdata", data); }
        
        public void SendMessageToClient(MessageModel msg)
        {
            Console.WriteLine(JsonSerializer.Serialize(msg));
            try
            {
                var agentId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Clients.User(msg.SessionId).SendAsync("Receive", msg);
                
                ChatLog chatLog = new ChatLog();
                chatLog.Message = msg.Text;
                chatLog.IsSentByClient = msg.IsSentByClient;
                chatLog.Timestamp = msg.Timestamp;

                chatLog.UserId = _sessionService.GetAgentIdBySessionId(Guid.Parse(msg.SessionId));
                chatLog.SessionId = Guid.Parse(msg.SessionId);

                _chatLogRepository.Add(chatLog);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("Exception", ex);
            }
        }
  
        public async Task SendMessageToAgent(MessageModel msg)
        {
            Console.WriteLine(JsonSerializer.Serialize(msg));
            try
            {
                var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_sessionService.IsSessionInTheWaitingList(Guid.Parse(userId)))
                {
                    msg.SessionId = userId;
                    string toAgentId = _sessionService.GetAgentIdBySessionId(Guid.Parse(userId)).ToString();
                    if (!_chats.Any(x => x.User.Id == msg.SessionId))
                    {
                        ChatModel chat = new ChatModel();
                        var session = _sessionRepository.GetById(Guid.Parse(userId));
                        chat.User.Name = session.ClientName;
                        chat.User.Id = session.Id.ToString();
                        chat.User.WebsiteId = session.WebsiteId.ToString();
                        _chats.Add(chat);
                        await Clients.User(toAgentId).SendAsync("AddChat", chat);
                    }
                    await Clients.User(toAgentId).SendAsync("Receive", msg);



                    ChatLog chatLog = new ChatLog();
                    chatLog.Message = msg.Text;
                    chatLog.IsSentByClient = msg.IsSentByClient;
                    chatLog.Timestamp = msg.Timestamp;

                    chatLog.UserId = _sessionService.GetAgentIdBySessionId(Guid.Parse(msg.SessionId));
                    chatLog.SessionId = Guid.Parse(msg.SessionId);

                    _chatLogRepository.Add(chatLog);

                }
                else
                {
                    await Clients.Caller.SendAsync("Receive", new MessageModel() { SessionId = userId, IsSentByClient=false, Text= "Sorry, there is no available agents now. Please, try again later", Timestamp=DateTime.Now}) ;
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Exception", ex);
            }
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("Notify", "Chat started");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.Caller.SendAsync("Notify", "Chat finished");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
