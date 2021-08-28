using LiveChat.Business.Commons;
using LiveChat.Business.Models.RESTResponces;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Constants.Enums;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Business.Services
{
    public class AgentService: IAgentService
    {
        private readonly IPasswordChangeTokenRepository _passwordTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatLogRepository _chatLogRepository;
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IOptions<AuthOptions> _authOptions;
    
        public AgentService(IUserRepository userRepository, 
            IPasswordChangeTokenRepository passwordTokenRepository, 
            IWebsiteRepository websiteRepository, 
            IOptions<AuthOptions> authOptions,
             IChatLogRepository chatLogRepository)
        {
            _userRepository = userRepository;
            _passwordTokenRepository = passwordTokenRepository;
            _websiteRepository = websiteRepository;
            _authOptions = authOptions;
            _chatLogRepository = chatLogRepository;
        }
        private bool IsUserWaitingForApproval(Guid id)
        {
          //  var tokens = _passwordTokenRepository.GetByUserId(id);
            var user = _userRepository.GetById(id);
            //    var unexpiredTokens = tokens.Where(x => x.IsExpired == false && x.ExpirationDate>DateTime.Now).ToList();
            //   if (unexpiredTokens.Count == 0) return false;
            if (user.PasswordHash.Length == 0) return true;
            else return false;
        }
        public Website GetWebsiteByUrl(string websiteUrl)
        {
            return _websiteRepository.GetByUrl(websiteUrl);
        }
        public AgentViewModel GetAgentById(Guid agentid)
        {
            var agent = _userRepository.GetById(agentid);
            if (agent == null) return null;
            string status = IsUserWaitingForApproval(agent.Id) ? "Waiting for approval" : "Active";
            var chatLogsTookPartIn = _chatLogRepository.GetByAgentId(agentid)
                .Where(x => x.Session.EndedAt != null)
                .Select(x=>x.SessionId)
                .Distinct();
            var passwordChangeTokens = _passwordTokenRepository.GetByUserId(agent.Id);
            var passwordChangeToken = passwordChangeTokens
                .Where(x => x.IsExpired == false && x.ExpirationDate > DateTime.Now)
                .SingleOrDefault();
            var agentModel = new AgentViewModel()
            {
                Id = agent.Id,
                Name = agent.Name,
                Email = agent.Email,
                Status = status,
                CompletedChatsCount = chatLogsTookPartIn.Count(),
                InvitationCode = passwordChangeToken != null ? passwordChangeToken.Id : null
            };
            return agentModel;
        }
        public IList<AgentViewModel> GetAllAgents(Guid websiteId)
        {
            var agents = _userRepository.GetAllUsers(websiteId);
            agents = agents.Where(x => x.Role != Roles.Admin).ToList();
            List<AgentViewModel> filteredAgents = new();
            foreach (var agent in agents)
            {
                string status = IsUserWaitingForApproval(agent.Id) ? "Waiting for approval" : "Active";
                filteredAgents.Add(new AgentViewModel
                {
                    Id = agent.Id,
                    Name = agent.Name,
                    Email = agent.Email,
                    Status = status,
              
                });
            }
            return filteredAgents;
        }

        public void DeleteAgent(Guid agentId)
        {
            _userRepository.Delete(agentId);
        }
    }
}
