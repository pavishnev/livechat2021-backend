using LiveChat.Data.Entities;
using System;
using System.Collections.Generic;

namespace LiveChat.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetByEmail(string email);
        User GetById(Guid id);
        IList<User> GetAllUsers(Guid id);
        //IEnumerable<UserEntity> GetUsersExceptCurrent(int id);
        User Add(User user);
        User Edit(User user);
        void Delete(User user);
        public void Delete(Guid userId);

    }
}
