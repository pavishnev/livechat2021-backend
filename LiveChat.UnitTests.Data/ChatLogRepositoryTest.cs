using FakeItEasy;
using LiveChat.Data;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace LiveChat.UnitTests.Data
{
    public class ChatLogRepositoryTest:IDisposable
    {
        private readonly LiveChatDbContext _context;

        public ChatLogRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<LiveChatDbContext>();
            options.UseInMemoryDatabase("ChatLogRepositoryTestInMemoryDatabase");

            _context = new LiveChatDbContext(options.Options);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetAllChatLogs()
        {
            //arrange
            var count = 5;
            var fake = A.CollectionOfDummy<ChatLog>(count);
            _context.ChatLogs.AddRange(fake);
            _context.SaveChanges();
            var userRepository = new ChatLogRepository(_context);

            // Act
            var result = userRepository.GetAllChatLogs();
            // Assert
            Assert.Equal(count,result.Count);

        }

        [Fact]
        public void GetAgentId()
        {
            //arrange
            var chatLog = new ChatLog()
            {
                User = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                UserId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")}
            };
            _context.ChatLogs.AddRange(chatLog);
            _context.SaveChanges();
            var Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var userRepository = new ChatLogRepository(_context);

            // Act
            var result = userRepository.GetByAgentId(Id);
            // Assert
            Assert.Equal(Id, result[0].UserId);

        }
        [Fact]
        public void GetById()
        {
            //arrange
            var chatLog = new ChatLog()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                User = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                UserId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") }
            };
            _context.ChatLogs.AddRange(chatLog);
            _context.SaveChanges();
            var Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var userRepository = new ChatLogRepository(_context);

            // Act
            var result = userRepository.GetById(Id);
            // Assert
            Assert.Equal(Id, result.Id);

        }
        [Fact]
        public void GetBySessionId()
        {
            //arrange
            var chatLog = new ChatLog()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                User = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                UserId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                SessionId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")
            };
            _context.ChatLogs.AddRange(chatLog);
            _context.SaveChanges();
            var Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var userRepository = new ChatLogRepository(_context);

            // Act
            var result = userRepository.GetBySessionId(Id);
            // Assert
            Assert.Equal(Id, result[0].SessionId);
        }

        [Fact]
        public void Add()
        {
            //arrange
            var chatLog = new ChatLog()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                User = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                UserId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                SessionId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")
            };

            var userRepository = new ChatLogRepository(_context);

            // Act
            var result = userRepository.Add(chatLog);
            // Assert
            Assert.Equal(1,_context.ChatLogs.Count());

        }
        [Fact]
        public void Edit()
        {
            //arrange
            var chatLogOld = new ChatLog()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                User = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                UserId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                SessionId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Message ="Old"
            };
            var chatLogNew = new ChatLog()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                User = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                UserId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                SessionId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Message = "New"
            };

            _context.ChatLogs.AddRange(chatLogOld);
            _context.SaveChanges();
            var userRepository = new ChatLogRepository(_context);

            // Act
            var result = userRepository.Edit(chatLogNew);
            // Assert
            Assert.Equal(chatLogNew.Message,result.Message);

        }
        [Fact]
        public void Delete()
        {
            //arrange
            var chatLog = new ChatLog()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                User = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                UserId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Session = new Session() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                SessionId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Message = "Old"
            };

            _context.ChatLogs.AddRange(chatLog);
            _context.SaveChanges();
            var userRepository = new ChatLogRepository(_context);

            // Act
            userRepository.Delete(chatLog);
            // Assert
            Assert.Equal(0,_context.ChatLogs.Count());

        }
    }
}
