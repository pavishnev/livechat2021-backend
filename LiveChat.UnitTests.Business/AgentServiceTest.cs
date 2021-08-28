using FakeItEasy;
using LiveChat.Business.Commons;
using LiveChat.Business.Models.RESTResponces;
using LiveChat.Business.Services;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LiveChat.UnitTests.Business
{
    public class AgentServiceTest
    {
        [Fact]
        public void GetWebSiteByUrl()
        {
            //arrange
            var passwordTokenRepository = A.Fake<IPasswordChangeTokenRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var chatLogRepository = A.Fake<IChatLogRepository>();

            var service = new AgentService(userRepository, passwordTokenRepository, websiteRepository, authOptions, chatLogRepository);
            var webUrl = "Test string";
            var website = new Website() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") };
            A.CallTo(() => websiteRepository.GetByUrl(webUrl)).Returns(website);
            //act
            var result = service.GetWebsiteByUrl(webUrl);
            //assert
            Assert.Equal(website.Id, result.Id);
        }

        [Theory, ClassData(typeof(IndexOfData))]
        public void GetAgentById(byte[] passwordHash)
        {
            //arrange
            var passwordTokenRepository = A.Fake<IPasswordChangeTokenRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var chatLogRepository = A.Fake<IChatLogRepository>();

            var id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var fakeLogs = new List<ChatLog>() { new ChatLog() { Session = new Session() } };
            var fakeTokens = A.CollectionOfDummy<PasswordChangeToken>(5);
            var service = new AgentService(userRepository, passwordTokenRepository, websiteRepository, authOptions, chatLogRepository);
            User agent = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), PasswordHash = passwordHash, PasswordChangeTokens = fakeTokens };
            var agentModel = new AgentViewModel()
            {
                Id = agent.Id
            };
            A.CallTo(() => userRepository.GetById(id)).Returns(agent);
            A.CallTo(() => chatLogRepository.GetByAgentId(id)).Returns(fakeLogs);
            //act
            var result = service.GetAgentById(id);
            //assert
            Assert.Equal(agentModel.Id, result.Id);
        }

        
        [Theory, ClassData(typeof(IndexOfData))]
        public void GetAllAgents(byte[] passwordHash)
        {
            //arrange
            var passwordTokenRepository = A.Fake<IPasswordChangeTokenRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var chatLogRepository = A.Fake<IChatLogRepository>();

            var id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var fakeTokens = A.CollectionOfDummy<PasswordChangeToken>(5);
            var service = new AgentService(userRepository, passwordTokenRepository, websiteRepository, authOptions, chatLogRepository);
            List<User> agents =new List<User>() { new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), PasswordHash =passwordHash, PasswordChangeTokens = fakeTokens } };
            A.CallTo(() => userRepository.GetAllUsers(id)).Returns(agents);
            A.CallTo(() => userRepository.GetById(id)).Returns(agents[0]);
            //act
            var result = service.GetAllAgents(id);
            //assert
            Assert.Equal(id, result[0].Id);
        }


        public class IndexOfData : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
        {
        new object[] { new byte[] { 1 }},
        new object[] { new byte[0] }
        };

            public IEnumerator<object[]> GetEnumerator()
            { return _data.GetEnumerator(); }

            IEnumerator IEnumerable.GetEnumerator()
            { return GetEnumerator(); }
        }
    }
}
