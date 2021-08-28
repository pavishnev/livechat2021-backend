using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveChat.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private LiveChatDbContext _context;
        public UserRepository(LiveChatDbContext context)
        {
            _context = context;
        }
        public User Add(User user)
        {
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
        public void Delete(Guid userId)
        {
            var user = GetById(userId);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public User Edit(User user)
        {
            var toEdit = _context.Users.SingleOrDefault(c => c.Id == user.Id);
            if (toEdit != null)
            {
                toEdit.Name = user.Name;
                toEdit.Email = user.Email;
                toEdit.Role = user.Role;
                toEdit.PasswordHash = user.PasswordHash;
                toEdit.Salt = user.Salt;
                toEdit.WebsiteId = user.WebsiteId;
                _context.Users.Update(toEdit);
                _context.SaveChanges();
            }
            return toEdit;
        }

        public IList<User> GetAllUsers(Guid websiteId) => _context.Users.Where(x=>x.WebsiteId== websiteId).ToList();
 

        public User GetByEmail(string email) => _context.Users.AsNoTracking().Include(x=>x.PasswordChangeTokens).SingleOrDefault(x => x.Email == email);
   

        public User GetById(Guid id) =>  _context.Users.AsNoTracking().SingleOrDefault(x => x.Id == id);
        
    }
}
