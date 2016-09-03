using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShareMe.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ShareMe.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        // POST api/token
        [HttpPost]
        public string Post(string token, string newToken, string comment)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ReturnMessage.ErrorMessage("token not supplied");
            }

            if (!TokenManager.AdminToken.Equals(token))
            {
                return ReturnMessage.ErrorMessage("unauthorised access, only admin token can be used to create tokens");
            }

            if (TokenManager.Tokens.Any(t => t.Key == newToken))
            {
                return ReturnMessage.ErrorMessage("token alreay exists");
            }

            if (string.IsNullOrWhiteSpace(newToken))
            {
                Token t = TokenManager.CreateToken();
                return ReturnMessage.OkTokenMessage("new token generated", t.Key);
            }
            else
            {
                TokenManager.CreateToken(newToken, comment);
                return ReturnMessage.OkTokenMessage("new token generated", newToken);
            }
        }

        // DELETE api/token/8bda29a8e47f6e5dbe5b99f4a2c93ab8
        [HttpDelete("{token}")]
        public string Delete(string token)
        {
            if (TokenManager.AdminToken.Equals(token))
            {
                return ReturnMessage.ErrorMessage("you can't delete the admin token!");
            }
            else
            {
                TokenManager.DeleteToken(token);
                return ReturnMessage.OkTokenMessage("token deleted", token);
            }
        }
    }
}
