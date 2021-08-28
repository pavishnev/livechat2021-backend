using LiveChat.Business.Models;
using LiveChat.Business.Models.RESTRequests;
using LiveChat.Business.Models.RESTResponces;
using LiveChat.Data.Entities;
using System;
using System.Collections.Generic;


namespace LiveChat.Business.Services.Interfaces
{
  public interface IUserService
  {
       
    Guid? Authenticate(LoginModel login);

    Guid RegisterAdmin(RegisterModel login);
    
    Guid RegisterAgent(RegisterAgentModel login);

    Guid CompleteRegisterAgent(CompleteRegisterAgent login);
    bool HasUser(string email);
    
    bool HasWebsite(string url);

    string GenerateJwt(Guid userId);

  }
}
