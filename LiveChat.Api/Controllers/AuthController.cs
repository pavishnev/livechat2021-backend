using LiveChat.Business.Models;
using LiveChat.Business.Models.RESTRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveChat.Business.Services.Interfaces;
using LiveChat.Constants;
using LiveChat.Constants.Enums;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LiveChat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        private IActionResult GenerateUserInfo(Guid id)
        {
            return Ok(new
            {
                access_token = _userService.GenerateJwt(id)
            });
        }
        //Регистрация пользователя. Регистрирует нового администратора для нового сайта
        //Возвращает токен после регистрации
        [HttpPost("sign-up")]
        public IActionResult SignUpAdmin(RegisterModel register)
        {
            if (_userService.HasUser(register.Email))
            {
                return Conflict(new BadHttpRequestException("User with this email alerady exists"));
            }
            if (_userService.HasWebsite(register.WebsiteUrl))
            {
                return Conflict(new BadHttpRequestException("This website was already registered"));
            }

            var id = _userService.RegisterAdmin(register);

            //return Ok(id);
            return GenerateUserInfo(id);
        }

        //Регистрация Агента. Его может зарегистрировать только администратор соответствующего вебсайте
        //Пользователь автоматически привязывается к нему
        //Регистрация неполная, в базу заносятся все данные кроме пароля пользователя
        //Возвращает токен для завершения регистрации. Данный токен имеет срок годности и может быть
        //использован единожды. Позволяет пользователю установить новый пароль
        [HttpPost("sign-up-agent")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult SignUpAgent(RegisterAgentModel register)
        {
            var websiteId = User.FindAll("websiteId").Select(x => x.Value).FirstOrDefault();
            register.WebsiteId =new Guid(websiteId);
            if (_userService.HasUser(register.Email))
            {
                return Conflict(new BadHttpRequestException("User with this email alerady exists"));
            }
            //var agentRegistrationToken = _userService.RegisterAgent(register);
            //return Ok(new
            //{
            //    PasswordChangeTokenId = agentRegistrationToken
            //});
            var userId = _userService.RegisterAgent(register);
            return Ok(new
            {
                userId = userId
            });
        }

        //Завершение регистрации. Принимает токен для изменения пароля из предыдущего метода,
        //а также новый пароль.
        //Если токен не истек и все данные верны, пользователь будет зарегистрирован и получить JWT токен
        [HttpPost("sign-up-agent-complete")]
        public IActionResult SignUpAgentComplete(CompleteRegisterAgent complete)
        {
            var userId = _userService.CompleteRegisterAgent(complete);
            if(userId == Guid.Empty)
            {
                return Conflict(new BadHttpRequestException("Token expired or already used"));
            }
            return GenerateUserInfo(userId);
        }

        //Метод авторизации. Принимает логин и пароль
        //В случае успешной регистрации возвращает JWT токен
        [HttpPost("sign-in")]
        public IActionResult SignIn(LoginModel login)
        {
            var id = _userService.Authenticate(login);
            if (id == null)
            {
                return Conflict(new BadHttpRequestException("Wrong email or password"));
            }

            return GenerateUserInfo((Guid)id);
        }

        //  [Authorize(Roles = Roles.Admin +","+ Roles.Agent)]

        // string email = User.FindAll(ClaimTypes.Email).Select(x => x.Value).FirstOrDefault();
        // string webpage = User.FindAll(ClaimTypes.Webpage).Select(x => x.Value).FirstOrDefault();

        // StringValues headerValues;
        // var token = string.Empty;

        // if (Request.Headers.TryGetValue("Authorization", out headerValues))
        // {
        //     token = headerValues.FirstOrDefault();
        // }
        // token = token.Split(' ')[1];
        // var handler = new JwtSecurityTokenHandler();
        // var jwtSecurityToken = handler.ReadJwtToken(token);

        // if (_userService.HasUser(register.Email))
        // {
        //     return Conflict(new BadHttpRequestException("User with this email alerady exists"));
        // }

        //// var id = _userService.RegisterAgent(register);

        // return Ok();
        // return GenerateUserInfo(id);
    }
}
