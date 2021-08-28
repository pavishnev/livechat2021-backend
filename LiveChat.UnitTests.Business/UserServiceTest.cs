using FakeItEasy;
using LiveChat.Business.Commons;
using LiveChat.Business.Models;
using LiveChat.Business.Models.RESTRequests;
using LiveChat.Business.Services;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace LiveChat.UnitTests.Business
{
    public class UserServiceTest
    {
        [Fact]
        public void RegisterAdmin()
        {
            //arrange
            var passwordTokenRepository = A.Fake<IPasswordChangeTokenRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new UserService(userRepository, passwordTokenRepository, websiteRepository, authOptions);

            A.CallTo(() => websiteRepository.Add(A<Website>.Ignored)).Returns(new Website());
            A.CallTo(() => userRepository.Add(A<User>.Ignored)).Returns(new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") });

            RegisterModel register = new RegisterModel() { WebsiteUrl = "test url", Password = "test password" };

            //act
            var result = service.RegisterAdmin(register);
            //assert
            Assert.NotEqual(result, new Guid());
        }
        [Fact]
        public void RegisterAgent()
        {
            //arrange
            var passwordTokenRepository = A.Fake<IPasswordChangeTokenRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new UserService(userRepository, passwordTokenRepository, websiteRepository, authOptions);

            A.CallTo(() => websiteRepository.Add(A<Website>.Ignored)).Returns(new Website());
            A.CallTo(() => userRepository.Add(A<User>.Ignored)).Returns(new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") });
            A.CallTo(() => passwordTokenRepository.Add(A<PasswordChangeToken>.Ignored)).Returns(new PasswordChangeToken());
            RegisterAgentModel register = new RegisterAgentModel() { Email = "test email" };

            //act
            var result = service.RegisterAgent(register);
            //assert
            Assert.NotEqual(result, new Guid());
        }

        [Fact]
        public void CompleteRegisterAgent()
        {
            //arrange
            var passwordTokenRepository = A.Fake<IPasswordChangeTokenRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new UserService(userRepository, passwordTokenRepository, websiteRepository, authOptions);

            A.CallTo(() => userRepository.Edit(A<User>.Ignored)).Returns(new User());
            A.CallTo(() => userRepository.GetById(A<Guid>.Ignored)).Returns(new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Salt = new byte[] { 1 } });
            A.CallTo(() => passwordTokenRepository.GetById(A<Guid>.Ignored)).Returns(new PasswordChangeToken() { IsExpired = false, ExpirationDate = DateTime.Now.AddDays(9), UserId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }); ;
            CompleteRegisterAgent register = new CompleteRegisterAgent() { invitationCode = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"), Password = "1" };

            //act
            var result = service.CompleteRegisterAgent(register);
            //assert
            Assert.NotEqual(result, Guid.Empty);
        }

        [Theory, ClassData(typeof(IndexOfData))]
        public void HasUser(User user, bool shouldBe)
        {
            //arrange
            var passwordTokenRepository = A.Fake<IPasswordChangeTokenRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new UserService(userRepository, passwordTokenRepository, websiteRepository, authOptions);

            A.CallTo(() => userRepository.GetByEmail(A<string>.Ignored)).Returns(user);
            A.CallTo(() => userRepository.Delete(A<Guid>.Ignored)).DoesNothing();
            //act
            var result = service.HasUser("");
            //assert
            Assert.Equal(result, shouldBe);
        }

        [Fact]
        public void HasWebSite()
        {
            //arrange
            var passwordTokenRepository = A.Fake<IPasswordChangeTokenRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new UserService(userRepository, passwordTokenRepository, websiteRepository, authOptions);

            A.CallTo(() => websiteRepository.GetByUrl(A<string>.Ignored)).Returns(new Website());
            //act
         var result=   service.HasWebsite("");
            //assert
            Assert.True(result);
        }

        [Fact]
        public void Authentificate()
        {
            //arrange
            var passwordTokenRepository = A.Fake<IPasswordChangeTokenRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var authOptions = A.Fake<IOptions<AuthOptions>>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            var service = new UserService(userRepository, passwordTokenRepository, websiteRepository, authOptions);

            LoginModel login = new LoginModel() {Email ="", Password= "1" };

            A.CallTo(() => websiteRepository.GetByUrl(A<string>.Ignored)).Returns(new Website());
            A.CallTo(() => userRepository.GetByEmail(A<string>.Ignored)).Returns(new User() { Salt = new byte[1] { 1 }, PasswordHash = new byte[1] { 1 } });
            //act
            var result = service.Authenticate(login);
            //assert
            Assert.Null(result);
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
                    SaltSize = 1
                }));
            _provider = services.BuildServiceProvider();

            var passwordTokenRepository = A.Fake<IPasswordChangeTokenRepository>();
            var userRepository = A.Fake<IUserRepository>();
            var websiteRepository = A.Fake<IWebsiteRepository>();
            IOptions<AuthOptions> options = _provider.GetService<IOptions<AuthOptions>>();
            var service = new UserService(userRepository, passwordTokenRepository, websiteRepository, options);
            var Id = new Guid();

            A.CallTo(() => websiteRepository.GetById(A<Guid>.Ignored)).Returns(new Website() { WebsiteUrl="test"});
            A.CallTo(() => userRepository.GetById(A<Guid>.Ignored))
                .Returns(new User() { Id=new Guid(),Email="test",Role="Admin", Name="Test", WebsiteId=new Guid()});
            //act
            var result = service.GenerateJwt(Id);
            //assert
            Assert.NotNull(result);
        }

        public class IndexOfData : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]>
        {
        new object[] {new User() { 
            PasswordChangeTokens = new List<PasswordChangeToken>() { new PasswordChangeToken() { ExpirationDate=DateTime.Now.AddDays(9), IsExpired=false} }, 
            PasswordHash=new byte[1]{ 1} },true },
        new object[] { new User() { 
            PasswordChangeTokens = new List<PasswordChangeToken>() { new PasswordChangeToken() { ExpirationDate = DateTime.Now.AddDays(9), IsExpired = true } },
            PasswordHash = new byte[0] {  } },false }
        };

            public IEnumerator<object[]> GetEnumerator()
            { return _data.GetEnumerator(); }

            IEnumerator IEnumerable.GetEnumerator()
            { return GetEnumerator(); }
        }
    }
}

