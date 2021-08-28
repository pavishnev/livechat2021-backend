using LiveChat.Business.Models.RESTResponces;
using LiveChat.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Business.Services.Interfaces
{
    public interface IAgentService
    {
        Website GetWebsiteByUrl(string websiteUrl);
        AgentViewModel GetAgentById(Guid agentid);
        IList<AgentViewModel> GetAllAgents(Guid websiteId);

        void DeleteAgent(Guid agentId);
    }
}
