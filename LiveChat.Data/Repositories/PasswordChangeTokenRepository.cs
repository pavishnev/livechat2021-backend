using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveChat.Data.Repositories
{
    public class PasswordChangeTokenRepository : IPasswordChangeTokenRepository
    {
        private LiveChatDbContext _context;
        public PasswordChangeTokenRepository(LiveChatDbContext context)
        {
            _context = context;
        }
        public PasswordChangeToken Add(PasswordChangeToken token)
        {
            token.Id = Guid.NewGuid();
            _context.PasswordChangeTokens.Add(token);
            _context.SaveChanges();
            return GetById(token.Id);
        }

        public void Delete(PasswordChangeToken token)
        {
            _context.PasswordChangeTokens.Remove(token);
            _context.SaveChanges();
        }

        public void Delete(Guid tokenId)
        {
             var token = GetById(tokenId);
            lock (_context)
            {
                _context.PasswordChangeTokens.Remove(token);
            }
             _context.SaveChanges();
        }

        public PasswordChangeToken Edit(PasswordChangeToken token)
        {
            throw new NotImplementedException();
        }

        public IList<PasswordChangeToken> GetAllTokens() => _context.PasswordChangeTokens.ToList();

        public PasswordChangeToken GetById(Guid id) => _context.PasswordChangeTokens.AsNoTracking().SingleOrDefault(x => x.Id == id);

        public IList<PasswordChangeToken> GetByUserId(Guid userId) => _context.PasswordChangeTokens.AsNoTracking().Where(x => x.UserId == userId).ToList();
    }
}
