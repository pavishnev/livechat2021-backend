using LiveChat.Data.Entities;
using System;
using System.Collections.Generic;

namespace LiveChat.Data.Repositories.Interfaces
{
    public interface IPasswordChangeTokenRepository
    {    
        PasswordChangeToken GetById(Guid id);
        IList<PasswordChangeToken> GetByUserId(Guid id);
        IList<PasswordChangeToken> GetAllTokens();
        
        PasswordChangeToken Add(PasswordChangeToken token);
        PasswordChangeToken Edit(PasswordChangeToken token);
        void Delete(PasswordChangeToken token);
        void Delete(Guid tokenId);
    }
}

