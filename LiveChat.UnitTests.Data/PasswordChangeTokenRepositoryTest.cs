using FakeItEasy;
using LiveChat.Data;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace LiveChat.UnitTests.Data
{
    public class PasswordChangeTokenRepositoryTest : IDisposable
    {
        private readonly LiveChatDbContext _context;

        public PasswordChangeTokenRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<LiveChatDbContext>();
            options.UseInMemoryDatabase("PasswordChangeTokenRepositoryTestInMemoryDatabase");

            _context = new LiveChatDbContext(options.Options);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetAllTokens()
        {
            lock (_context)
            {
                //arrange
                var count = 5;
                var fakeTokens = A.CollectionOfDummy<PasswordChangeToken>(count);
                _context.PasswordChangeTokens.AddRange(fakeTokens);
                _context.SaveChanges();
                var repository = new PasswordChangeTokenRepository(_context);

                // Act
                var result = repository.GetAllTokens();
                // Assert
                Assert.Equal(count, result.Count);
            }
        }

        [Fact]
        public void GetById()
        {
            //arrange
            lock (_context)
            {
                var token = new PasswordChangeToken()
                {
                    Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")
                };
                _context.PasswordChangeTokens.AddRange(token);
                _context.SaveChanges();
                var Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
                var repository = new PasswordChangeTokenRepository(_context);

                // Act
                var result = repository.GetById(Id);
                // Assert
                Assert.Equal(Id, result.Id);
            }
        }
        [Fact]
        public void GetByUserId()
        {
            //arrange
            lock (_context)
            {
                var token = new PasswordChangeToken()
                {
                    Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                    User = new User() { Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7") },
                    UserId = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                };
                _context.PasswordChangeTokens.AddRange(token);
                _context.SaveChanges();
                var Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
                var repository = new PasswordChangeTokenRepository(_context);

                // Act
                var result = repository.GetByUserId(Id);
                // Assert
                Assert.Equal(Id, result[0].UserId);
            }
        }

        [Fact]
        public void Add()
        {
            lock (_context)
            {
                //arrange
                var token = new PasswordChangeToken()
                {
                    Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")
                };
                var repository = new PasswordChangeTokenRepository(_context);

                // Act
                var result = repository.Add(token);
                // Assert
                Assert.Equal(1, _context.PasswordChangeTokens.Count());
            }
        }

        [Fact]
        public void Edit()
        {
            lock (_context)
            {
                //arrange
                var repository = new PasswordChangeTokenRepository(_context);
                // Act
                // Assert
                Assert.Throws<NotImplementedException>(() => repository.Edit(new PasswordChangeToken()));
            }
        }

        [Fact]
        public void Delete()
        {
            lock (_context)
            {
                //arrange
                var token = new PasswordChangeToken()
                {
                    Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                };

                _context.PasswordChangeTokens.AddRange(token);
                _context.SaveChanges();
                var repository = new PasswordChangeTokenRepository(_context);

                // Act
                repository.Delete(token);
                // Assert
                Assert.Equal(0, _context.PasswordChangeTokens.Count());
            }
        }
    }
}
