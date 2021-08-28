using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Data.MockRepositories
{
    public class PasswordTokenMockRepository : IPasswordChangeTokenRepository
    {
        List<PasswordChangeToken> tokens = new List<PasswordChangeToken>();

        public PasswordChangeToken Add(PasswordChangeToken token)
        {
            token.Id = Guid.NewGuid();
            tokens.Add(token);
            return GetById(token.Id);
        }

        public void Delete(PasswordChangeToken token)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid tokenId)
        {
            throw new NotImplementedException();
        }

        public PasswordChangeToken Edit(PasswordChangeToken token)
        {
            throw new NotImplementedException();
        }

        public IList<PasswordChangeToken> GetAllTokens()
        {
            throw new NotImplementedException();
        }

        public PasswordChangeToken GetById(Guid id) => tokens.SingleOrDefault(x => x.Id == id);
   

        public PasswordChangeToken GetByUserId(Guid id)
        {
            throw new NotImplementedException();
        }

        IList<PasswordChangeToken> IPasswordChangeTokenRepository.GetByUserId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
