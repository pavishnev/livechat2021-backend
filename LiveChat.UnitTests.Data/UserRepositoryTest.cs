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
  public  class UserRepositoryTest:IDisposable
    {
        private readonly LiveChatDbContext _context;

        public UserRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<LiveChatDbContext>();
            options.UseInMemoryDatabase("UserRepositoryTestInMemoryDatabase");

            _context = new LiveChatDbContext(options.Options);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetAllUsers()
        {
            //arrange
            var count = 5;
            var fakeUsers = A.CollectionOfDummy<User>(count);
            _context.Users.AddRange(fakeUsers);
            _context.SaveChanges();
            var repository = new UserRepository(_context);

            // Act
            var result = repository.GetAllUsers( new Guid());
            // Assert
            Assert.Equal(count, result.Count);
        }

        [Fact]
        public void GetByEmail()
        {
            //arrange
            var user = new User()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Email = "c"
            };
            _context.Users.AddRange(user);
            _context.SaveChanges();
            var email ="c";
            var repository = new UserRepository(_context);
            // Act
            var result = repository.GetByEmail(email);
            // Assert
            Assert.Equal(email, result.Email);
        }
        [Fact]
        public void GetById()
        {
            //arrange
            var user = new User()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
            };
            _context.Users.AddRange(user);
            _context.SaveChanges();
            var Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7");
            var repository = new UserRepository(_context);

            // Act
            var result = repository.GetById(Id);
            // Assert
            Assert.Equal(Id, result.Id);
        }

        [Fact]
        public void Add()
        {
            //arrange
            var user = new User()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")
            };

            var repository = new UserRepository(_context);

            // Act
            var result = repository.Add(user);
            // Assert
            Assert.Equal(1, _context.Users.Count());
        }
        [Fact]
        public void Edit()
        {
            //arrange
            var userOld = new User()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Name="Old"
            };
            var userNew = new User()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7"),
                Name="New"
            };

            _context.Users.AddRange(userOld);
            _context.SaveChanges();
            var repository = new UserRepository(_context);

            // Act
            var result = repository.Edit(userNew);
            // Assert
            Assert.Equal(userNew.Name, result.Name);

        }
        [Fact]
        public void Delete()
        {
            //arrange
            var user = new User()
            {
                Id = new Guid("83A9E26C-07B1-4821-B4A3-56CA06EBF4C7")
            };

            _context.Users.AddRange(user);
            _context.SaveChanges();
            var repository = new UserRepository(_context);

            // Act
            repository.Delete(user);
            // Assert
            Assert.Equal(0, _context.Users.Count());

        }
    }
}
