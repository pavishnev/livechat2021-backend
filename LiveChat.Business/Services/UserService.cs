using LiveChat.Business.Commons;
using LiveChat.Business.Models;
using LiveChat.Business.Models.RESTRequests;
using LiveChat.Business.Models.RESTResponces;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Constants.Enums;
using LiveChat.Data.Entities;
using LiveChat.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace LiveChat.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IPasswordChangeTokenRepository _passwordTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IOptions<AuthOptions> _authOptions;
        private readonly SHA1 _sha1 = SHA1.Create();
        private readonly RNGCryptoServiceProvider _secureRandom = new RNGCryptoServiceProvider();
        public UserService(IUserRepository userRepository, IPasswordChangeTokenRepository passwordTokenRepository, IWebsiteRepository websiteRepository, IOptions<AuthOptions> authOptions)
        {
            _userRepository = userRepository;
            _passwordTokenRepository = passwordTokenRepository;
            _websiteRepository = websiteRepository;
            _authOptions = authOptions;
        }
        private byte[] ComputePasswordHash(string password, IEnumerable<byte> salt)
        {
            return _sha1.ComputeHash(Encoding.UTF8.GetBytes(password).Concat(salt).ToArray());
        }
        private byte[] CreateSalt()
        {
            var salt = new byte[_authOptions.Value.SaltSize];
            _secureRandom.GetBytes(salt);
            return salt;
        }
        public Guid RegisterAdmin(RegisterModel register)
        {
            // var user = _mapper.Map<User>(register);
            var newWebsite = new Website();
            newWebsite.WebsiteUrl = register.WebsiteUrl;
            _websiteRepository.Add(newWebsite);

            var user = new User();
            user.Name = register.Name;
            user.Email = register.Email;
            user.Role = Roles.Admin;
            user.Salt = CreateSalt();
            user.PasswordHash = ComputePasswordHash(register.Password, user.Salt);

            user.WebsiteId = newWebsite.Id;

            user =_userRepository.Add(user);
            return user.Id;
        }

        public Guid RegisterAgent(RegisterAgentModel register)
        {
            var user = new User();
           // var websiteId = _websiteRepository.GetAllWebsites().SingleOrDefault(x => x.WebsiteUrl == register.WebsiteUrl).Id;
            user.WebsiteId = register.WebsiteId;
            user.Name = register.Name;
            user.Email = register.Email;
            user.Role = Roles.Agent;
            user.Salt = CreateSalt();
            user.PasswordHash = new byte[0];

           user= _userRepository.Add(user);
            //В базе данных создается токен
            //Параметр ExpirationDate определяет дату, после которой регистрация агента будет невозможна
            //Параметр IsExpired получает значение true после того, как токен был использован
            //Если токен не был использован и дата истекла, то токен удаляется вместе с пользователем
            //Если токен был использован до истечения даты, то он остается в базе??

            PasswordChangeToken token = new();
            token.UserId = user.Id;
            token.ExpirationDate = DateTime.Now.AddMinutes(60);
            token.IsExpired = false;

            _passwordTokenRepository.Add(token);
            //return token.Id;
            return user.Id;
        }
        public Guid CompleteRegisterAgent(CompleteRegisterAgent complete)
        {
            var verificationToken = _passwordTokenRepository.GetById(complete.invitationCode);

            if (verificationToken != null && verificationToken.IsExpired == false && verificationToken.ExpirationDate > DateTime.Now)
            {
                var uncompletedUser = _userRepository.GetById(verificationToken.UserId);
                uncompletedUser.PasswordHash = ComputePasswordHash(complete.Password, uncompletedUser.Salt);
                _userRepository.Edit(uncompletedUser);
                _passwordTokenRepository.Delete(complete.invitationCode);
                return uncompletedUser.Id;
            }
            else return Guid.Empty;
        }
        public string GenerateJwt(Guid userId)
        {
            var user = _userRepository.GetById(userId);
            var websiteUrl = _websiteRepository.GetById(user.WebsiteId).WebsiteUrl;
            var authOptions = _authOptions.Value;

            var securityKey = authOptions.SymmetricSecurityKey;
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              authOptions.Issuer,
              authOptions.Audience,
              new[]
              {
          new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
          new Claim(JwtRegisteredClaimNames.Name, user.Name),
          new Claim(JwtRegisteredClaimNames.Email,user.Email),
          new Claim("role", user.Role),
          new Claim("websiteId",user.WebsiteId.ToString()),
          new Claim("websiteUrl",websiteUrl)
          
              },
              expires: DateTime.Now.AddSeconds(authOptions.TokenLifetime),     
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        //проверка наличия пользователя в базе данных 
        //если он есть, но он не подтвердил свой аккаунт в течении дозволенного времени
        //(не зашел и не сменил изначальный пароль)
        //его аккаунт удаляется, и становится доступным для повторной регистрации
        public bool HasUser(string email)
        {
             var user = _userRepository.GetByEmail(email);
            if (user == null) return false;
            var tokens = user.PasswordChangeTokens.Where(x => x.ExpirationDate > DateTime.Now && x.IsExpired == false);
            if (user.PasswordHash.Length == 0 && tokens.Count() == 0)
            {
                _userRepository.Delete(user);
                return false;
            }
            else return true;
        }
        
       
        public bool HasWebsite(string url)
        {
            return _websiteRepository.GetByUrl(url) != null;
        }

        public Guid? Authenticate(LoginModel login)
        {
            var user = _userRepository.GetByEmail(login.Email);
            if (user == null) return null;

            var isPasswordValid = ComputePasswordHash(login.Password, user.Salt)
              .SequenceEqual(user.PasswordHash);

            return isPasswordValid ? user.Id : null;
        }

      
    }
}