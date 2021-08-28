using LiveChat.Business.Commons;
using LiveChat.Business.Models;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace LiveChat.Business.Services
{
    public class SessionService : ISessionService
    {
        public static List<AgentModel> agentsOnline { get; set; } = new List<AgentModel>();
        public static List<Session> waitingList { get; set; } = new List<Session>();
        private readonly ISessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IOptions<AuthOptions> _authOptions;

        public SessionService(ISessionRepository sessionRepository, IUserRepository userRepository, IOptions<AuthOptions> authOptions, IWebsiteRepository websiteRepository)
        {
            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
            _authOptions = authOptions;
            _websiteRepository = websiteRepository;
        }

        public Guid StartSession(ClientModel newClient)
        {
           var Website= _websiteRepository.GetById(Guid.Parse(newClient.WebsiteId));
            if (Website == null)
                throw new Exception();
            var session = new Session()
            {
                ClientName = newClient.Name,
                Website =Website,
                StartedAt = DateTime.Now 
            };

                session = _sessionRepository.Add(session);
            if (agentsOnline.Any(x => x.WebsiteId == Website.Id.ToString().ToLower()))
                ConnectClientToAgent(session);
            else
                waitingList.Add(session);
            return session.Id;
        }

        public void ConnectClientToAgent(Session session)
        {
            if (!SessionWasConnectedToAgent(session.Id))
            {
                var agents = agentsOnline.Where(x => x.WebsiteId == session.WebsiteId.ToString()).ToList();
                if (agents.Count() == 0 || agentsOnline.Count() == 0) throw new Exception();
                agentsOnline.Where(x => x.ClientsOnline.Count() == agents.Min(a => a.ClientsOnline.Count()) && x.WebsiteId == session.WebsiteId.ToString()).FirstOrDefault().ClientsOnline.Add(session);
            }
        }

        public void DisconnectClient(Guid id)
        {
            var session = _sessionRepository.GetById(id);
            if (waitingList.Any(x => x.Id == id))
                waitingList.Remove(session);
            else
            {
                agentsOnline?.Where(x => x.ClientsOnline.Any(y => y.Id == session.Id))?.FirstOrDefault()?.ClientsOnline
               ?.Remove(agentsOnline?.Where(x => x.ClientsOnline.Any(y => y.Id == session.Id))?.FirstOrDefault()?.ClientsOnline?.Where(y => y.Id == session.Id)?.FirstOrDefault());
            }

            session.EndedAt = DateTime.Now;
            _sessionRepository.Edit(session);
        }

        public void AddAgentOnline(AgentModel agent)
        {
            agent.WebsiteId = _userRepository.GetById(agent.Id).WebsiteId.ToString();
            var clients = waitingList.Where(x => x.WebsiteId.ToString() == agent.WebsiteId);
            foreach (var item in clients.ToList())
            {
                agent.ClientsOnline.Add(item);
                waitingList.Remove(item);
            }
            agentsOnline.Add(agent);
        }

        public void RemoveAgentOnline(AgentModel agent)
        {
            agent.WebsiteId = _userRepository.GetById(agent.Id).WebsiteId.ToString();
            agent = agentsOnline.Where(x => x.Id == agent.Id).FirstOrDefault();
            if (agent != null)
            {
                agentsOnline.Remove(agent);
                if (agentsOnline.Any(x => x.WebsiteId == agent.WebsiteId))
                {
                    foreach (var item in agent.ClientsOnline)
                    {
                        ConnectClientToAgent(item);
                    }
                }
                else
                {
                    foreach (var item in agent.ClientsOnline)
                    {
                        waitingList.Add(item);
                    }
                }
            }
        }
        public bool SessionWasConnectedToAgent(Guid id)
        {
            return agentsOnline.Any(x => x.ClientsOnline.Any(y => y.Id == id));
        }
        public bool SessionIsActive(Guid Id)
        {
            return _sessionRepository.GetById(Id) != null && _sessionRepository.GetById(Id).EndedAt == null;
        }
        public bool SessionExists(Guid Id)
        {
            return _sessionRepository.GetById(Id) != null;
        }

        public bool IsAgentOnline(AgentModel agent)
        {
            return agentsOnline.Any(x => x.Id == agent.Id);
        }

        public string GenerateJwt(Guid sessionId)
        {

            var authOptions = _authOptions.Value;
            var securityKey = authOptions.SymmetricSecurityKey;
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              authOptions.Issuer,
              authOptions.Audience,
              new[]
              {
                  new Claim(JwtRegisteredClaimNames.Sub, sessionId.ToString())
              },
              expires: DateTime.Now.AddYears(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Guid GetAgentIdBySessionId(Guid sessionId)
        {
            AgentModel agent = agentsOnline?.Where(x => x.ClientsOnline.Any(y => y.Id == sessionId)).FirstOrDefault();

            if (agent == null) throw new Exception();

            return agent.Id;
        }
        public bool IsSessionInTheWaitingList(Guid id)
        {
            return waitingList.Any(X509EncryptingCredentials => X509EncryptingCredentials.Id == id);
        }
    }
}
