using FakeItEasy;
using LiveChat.Business.Models;
using LiveChat.Business.Services;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LiveChat.UnitTests.Business
{
    public class SessionControlTest
    {
        [Fact]
        public async Task Run()
        {
            //arrange
            var sessionRepository = A.Fake<ISessionRepository>();
            var sessionService = A.Fake<ISessionService>();
            var fakeSessions = A.CollectionOfDummy<Session>(5);
            A.CallTo(() => sessionRepository.GetNotEndedSessions()).Returns(fakeSessions);
            A.CallTo(() => sessionService.DisconnectClient(A<Guid>.Ignored)).DoesNothing();
            var service = new SessionsControl(sessionService, sessionRepository);

            //act
            var result = await service.Run();
            //assert
            Assert.True(result);
        }
    }
}
