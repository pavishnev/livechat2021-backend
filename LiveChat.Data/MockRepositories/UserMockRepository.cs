using LiveChat.Constants.Enums;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Data.MockRepositories
{
    public class UserMockRepository : IUserRepository
    {
        List<User> users = new List<User>{
            new User{
                Id=Guid.NewGuid(),
                Name="Aboba",
                Email="aboba@yopmail.com",
                Role=Roles.Admin,
                WebsiteId = Guid.Parse("8dea0b6d-c6cc-4189-acde-eada87c16b9a"),
                PasswordHash=new byte[20]{102,93,212,110,29,2,252,154,103,211,16,185,159,44,146,52,174,44,155,222},
                Salt=new byte[8]{190,232,70,32,53,73,76,154},
                //password = 1
            },
              new User{
                Id=Guid.NewGuid(),
                Name="Svitlana",
                Email="svitlana@yopmail.com",
                Role=Roles.Agent,
                 WebsiteId = Guid.Parse("8dea0b6d-c6cc-4189-acde-eada87c16b9a"),
                PasswordHash=new byte[20]{102,93,212,110,29,2,252,154,103,211,16,185,159,44,146,52,174,44,155,222},
                Salt=new byte[8]{190,232,70,32,53,73,76,154},
                //password = 1
            }
        };

        public IList<User> GetAllUsers(Guid id)
        {
            throw new NotImplementedException();
        }

        public User GetByEmail(string email)
        {
            return users.SingleOrDefault(x => x.Email == email);
        }

        public User GetById(Guid id)
        {
            return users.SingleOrDefault(x => x.Id == id);
        }
        public User Add(User user)
        {
            user.Id = Guid.NewGuid();
            users.Add(user);
            return user;
        }

        public void Delete(User user)
        {
            throw new NotImplementedException();
        }

        public User Edit(User user)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
