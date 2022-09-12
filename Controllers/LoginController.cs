using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;

namespace Login.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public object Post(
            [FromBody]User user,
            [FromServices]UsersDAO usersDAO,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {
            bool credentialValidation = false;
            if (user != null && !String.IsNullOrWhiteSpace(user.UserID))
            {
                var userBase = usersDAO.Find(user.UserID);
                credentialValidation = (userBase != null &&
                    user.UserID == userBase.UserID &&
                    user.AccessKey == userBase.AccessKey);
            }
            
            if (credentialValidation)
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(user.UserID, "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserID)
                    }
                );

                DateTime dataCreate = DateTime.Now;
                DateTime dataExpiration = dataCreate +
                    TimeSpan.FromSeconds(tokenConfigurations.Seconds);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCreate,
                    Expires = dataExpiration
                });
                var token = handler.WriteToken(securityToken);

                return new
                {
                    authenticated = true,
                    created = dataCreate.ToString("yyyy-MM-dd HH:mm:ss"),
                    expiration = dataExpiration.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken = token,
                    message = "OK"
                };
            }
            else
            {
                return new
                {
                    authenticated = false,
                    message = "Failed to Authenticate!"
                };
            }
        }
    }
}