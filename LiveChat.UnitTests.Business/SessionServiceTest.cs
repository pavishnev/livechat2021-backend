using System;
using Xunit;
using LiveChat.Business.Services;
using FakeItEasy;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using LiveChat.Business.Commons;
using LiveChat.Business.Models;
using System.Threading.Tasks;
using LiveChat.Data.Entities;
using System.Linq;
using Moq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using Xunit.Sdk;
using Microsoft.Extensions.DependencyInjection;

namespace LiveChat.UnitTests.Business
{
    public class SessionServiceTest
    {
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class TestBeforeAfter : BeforeAfterTestAttribute
        {
            public override void Before(MethodInfo methodUnderTest)
            {
                SessionService.agentsOnline.Clear();
                SessionService.waitingList.Clear();
            }

            public override void After(MethodInfo methodUnderTest)
            {
                SessionService.agentsOnline.Clear();
                SessionService.waitingList.Clear();
            }
        }

        [Fact]
        [TestBeforeAfter]
        public void AddAgentOnline_ShouldAddAgentToLost()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();

            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var agentFakeRes = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            A.CallTo(() => userRepository.GetById(agent.Id)).Returns(agentFakeRes);
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            //act
            service.AddAgentOnline(agent);
            //assert
            Assert.Single(SessionService.agentsOnline);
            Assert.Equal(agent.Id, SessionService.agentsOnline.FirstOrDefault().Id);
        }

        [Fact]
        [TestBeforeAfter]
        public void AddAgentOnline_ShouldAddAgentToListAndClearWaitingList()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();

            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var agentFakeRes = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var session = new Session() { WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            A.CallTo(() => userRepository.GetById(agent.Id)).Returns(agentFakeRes);
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);
            SessionService.waitingList.Add(session);

            //act
            service.AddAgentOnline(agent);
            //assert
            Assert.Single(SessionService.agentsOnline);
            Assert.Equal(agent.Id, SessionService.agentsOnline.FirstOrDefault().Id);
            Assert.DoesNotContain(session, SessionService.waitingList);
            Assert.Single(SessionService.agentsOnline.FirstOrDefault().ClientsOnline);
        }

        [Fact]
        [TestBeforeAfter]
        public void RemoveAgentOnline_ShouldRemoveAgentWithoutClients()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();

            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var agentFakeRes = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var session = new Session() { WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            A.CallTo(() => userRepository.GetById(agent.Id)).Returns(agentFakeRes);
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);
            SessionService.agentsOnline.Add(agent);

            //act
            service.RemoveAgentOnline(agent);
            //assert
            Assert.DoesNotContain(agent, SessionService.agentsOnline);
        }

        [Fact]
        [TestBeforeAfter]
        public void RemoveAgentOnline_ShouldRemoveAgentWithClients_NoOtherAgents()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();

            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var agentFakeRes = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var session = new Session() { WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            A.CallTo(() => userRepository.GetById(agent.Id)).Returns(agentFakeRes);
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);
            SessionService.agentsOnline.Add(new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = "83A9E26C-07B1-4821-B4A3-56CA06EBF4C7", ClientsOnline = new List<Session>() { session } });
            //act
            service.RemoveAgentOnline(agent);
            //assert
            Assert.DoesNotContain(agent, SessionService.agentsOnline);
            Assert.Single(SessionService.waitingList);
        }
        [Fact]
        [TestBeforeAfter]
        public void RemoveAgentOnline_ShouldRemoveAgentWithClients_OtherAgentsOnline()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();

            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var agentFakeRes = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var session = new Session() { WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var secondAgentId = new Guid("00000000-07B1-4821-B4A3-56CA06EBF4C7");

            A.CallTo(() => userRepository.GetById(agent.Id)).Returns(agentFakeRes);
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);
            SessionService.agentsOnline.Add(new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = "83A9E26C-07B1-4821-B4A3-56CA06EBF4C7".ToLower(), ClientsOnline = new List<Session>() { session } });
            SessionService.agentsOnline.Add(new AgentModel() { Id = secondAgentId, WebsiteId = "83A9E26C-07B1-4821-B4A3-56CA06EBF4C7".ToLower(), ClientsOnline = new List<Session>() });
            //act
            service.RemoveAgentOnline(agent);
            //assert
            Assert.DoesNotContain(agent, SessionService.agentsOnline);
            Assert.Single(SessionService.agentsOnline.FirstOrDefault().ClientsOnline);
        }

        [Fact]
        [TestBeforeAfter]
        public void AddNewSession_NoAgentsAvailable()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();

            var newClient = new ClientModel() { Name = "Test", WebsiteId = "83A9E26C-07B1-4821-B4A3-56CA06EBF4C7".ToLower() };
            var session = new Session() { WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            A.CallTo(() => websiteRepository.GetById(Guid.Parse(newClient.WebsiteId))).Returns(new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") });
            A.CallTo(() => sessionRepository.Add(A<Session>.Ignored)).Returns(session);
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);
            //act
            service.StartSession(newClient);
            //assert
            Assert.Single(SessionService.waitingList);

        }

        [Fact]
        [TestBeforeAfter]
        public void AddNewSession_AgentOnline()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();

            var newClient = new ClientModel() { Name = "Test", WebsiteId = "83A9E26C-07B1-4821-B4A3-56CA06EBF4C7".ToLower() };
            var sessionRes = new Session { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);
            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = "83A9E26C-07B1-4821-B4A3-56CA06EBF4C7".ToLower() };

            A.CallTo(() => websiteRepository.GetById(Guid.Parse(newClient.WebsiteId))).Returns(sessionRes.Website);
            A.CallTo(() => sessionRepository.Add(A<Session>.Ignored)).Returns(sessionRes);
            SessionService.agentsOnline.Add(agent);
            //act
            service.StartSession(newClient);
            //assert
            Assert.Single(SessionService.agentsOnline.FirstOrDefault().ClientsOnline);

        }

        [Fact]
        [TestBeforeAfter]
        public void DisconnectClient_ShouldRemoveClientFromWaitingList()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            var id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            A.CallTo(() => sessionRepository.Edit(A<Session>.Ignored)).Returns(session);
            A.CallTo(() => sessionRepository.GetById(A<Guid>.Ignored)).Returns(session);
            SessionService.waitingList.Add(session);
            //act
            service.DisconnectClient(id);
            //assert
            Assert.DoesNotContain(session, SessionService.waitingList);

        }

        [Fact]
        [TestBeforeAfter]
        public void DisconnectClient_ShouldRemoveClientFromAgentList()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            var id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = "83A9E26C-07B1-4821-B4A3-56CA06EBF4C7".ToLower(), ClientsOnline = new List<Session>() { session } };

            A.CallTo(() => sessionRepository.Edit(A<Session>.Ignored)).Returns(session);
            A.CallTo(() => sessionRepository.GetById(A<Guid>.Ignored)).Returns(session);
            SessionService.agentsOnline.Add(agent);
            //act
            service.DisconnectClient(id);
            //assert
            Assert.DoesNotContain(session, SessionService.agentsOnline.FirstOrDefault().ClientsOnline);

        }

        [Fact]
        [TestBeforeAfter]
        public void GetAgentId()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            var id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = "83A9E26C-07B1-4821-B4A3-56CA06EBF4C7".ToLower(), ClientsOnline = new List<Session>() { session } };

            SessionService.agentsOnline.Add(agent);
            //act
            var result = service.GetAgentIdBySessionId(id);
            //assert
            Assert.Equal(id, result);

        }

        [Fact]
        [TestBeforeAfter]
        public void IsAgentOnline_true()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            var session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            SessionService.agentsOnline.Add(agent);
            //act
            var result = service.IsAgentOnline(agent);
            //assert
            Assert.True(result);

        }

        [Fact]
        [TestBeforeAfter]
        public void IsAgentOnline_false()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            var session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            //act
            var result = service.IsAgentOnline(agent);
            //assert
            Assert.False(result);

        }

        [Fact]
        [TestBeforeAfter]
        public void IsSessionExists()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            var session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            A.CallTo(() => sessionRepository.GetById(A<Guid>.Ignored)).Returns(session);

            //act
            var result = service.SessionExists(session.Id);
            //assert
            Assert.True(result);

        }

        [Fact]
        [TestBeforeAfter]
        public void IsSessionActive()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            var session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            A.CallTo(() => sessionRepository.GetById(A<Guid>.Ignored)).Returns(session);

            //act
            var result = service.SessionIsActive(session.Id);
            //assert
            Assert.True(result);

        }

        [Fact]
        [TestBeforeAfter]
        public void SessionWasConnectedToAgent_True()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            var session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), ClientsOnline = new List<Session>() { session } };

            SessionService.agentsOnline.Add(agent);
            //act
            var result = service.SessionWasConnectedToAgent(session.Id);
            //assert
            Assert.True(result);

        }

        [Fact]
        [TestBeforeAfter]
        public void SessionWasConnectedToAgent_False()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            var session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };

            //act
            var result = service.SessionWasConnectedToAgent(session.Id);
            //assert
            Assert.False(result);

        }

        [Fact]
        [TestBeforeAfter]
        public void ConnectClientToAgent()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();

            var session = new Session { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);
            var agent = new AgentModel() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), WebsiteId = "83A9E26C-07B1-4821-B4A3-56CA06EBF4C7".ToLower() };

            SessionService.agentsOnline.Add(agent);
            //act
            service.ConnectClientToAgent(session);
            //assert
            Assert.Single(SessionService.agentsOnline.FirstOrDefault().ClientsOnline);


        }

        [Fact]
        [TestBeforeAfter]
        public void IsSessionInTheWaitingList_true()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();

            var session = new Session { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);

            SessionService.waitingList.Add(session);
            //act
           var result = service.IsSessionInTheWaitingList(session.Id);
            //assert
            Assert.True(result);
        }

        [Fact]
        [TestBeforeAfter]
        public void IsSessionInTheWaitingList_false()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();

            var session = new Session { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }, WebsiteId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            var service = new SessionService(sessionRepository, userRepository, authOptions, websiteRepository);
            //act
            var result = service.IsSessionInTheWaitingList(session.Id);
            //assert
            Assert.False(result);
        }

        [Fact]
        public void GenerateJwt()
        {
            //arrange
            ServiceProvider _provider;
            var services = new ServiceCollection();
            services.AddTransient<IOptions<AuthOptions>>(
                provider => Options.Create<AuthOptions>(new AuthOptions
                {
                    Issuer = "test",
                    Audience = "test",
                    Secret = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                    SaltSize= 1
                }));
            _provider = services.BuildServiceProvider();
            var sessionRepository = A.Fake<ISessionRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            IOptions<AuthOptions> options = _provider.GetService<IOptions<AuthOptions>>();
            var service = new SessionService(sessionRepository, userRepository, options, websiteRepository);
            var Id = new Guid();
            //act
            var result = service.GenerateJwt(Id);
            //assert
            Assert.NotNull(result);
        }

    }

}
