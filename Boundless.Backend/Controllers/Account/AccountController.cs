using System.Text.RegularExpressions;
using Boundless.Backend.Controllers.Account.Models;
using Boundless.Backend.Exceptions;
using Boundless.Backend.Managers.Accounts;
using Boundless.Backend.Managers.Errors.Models;
using Boundless.Backend.Managers.Tokens.Models;
using Boundless.Backend.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Boundless.Backend.Controllers.Account;

[ApiController]
[Route("account")]
public class AccountController(AccountManager Manager) : ControllerBase
{
    [HttpGet("token")]
    public async Task<IActionResult> CreateToken([FromForm] string username, [FromForm] string password /* sha256password */) 
    {
        try
        {
            var account = await Manager.GetByUsernameAsync(username);
            if (account == null || account.Password != password.ComputeHash())
                return await JsonError.HandleAsync(HttpContext, null, "InvalidUsernameOrPassword", "Invalid username or password.");

            var token = new Token(account.AccountId, HttpContext);
            await TokenInstance!.AddAsync(token);

            return TokenResponse.Create(token);
        }
        catch (Exception ex)
        {
            return await JsonError.HandleInternalAsync(HttpContext, null, ex);
        }
    }
    
    [HttpGet("create")]
    public async Task<IActionResult> CreateAccount([FromForm] string username, [FromForm] string password)
    {
        try
        {
            if (!Regex.IsMatch(username, "^[0-9A-Za-z]{3,16}$"))
                return await JsonError.HandleAsync(HttpContext, null, "InvalidUsername", 
                    "Username can only contain letters and numbers and must be 3-16 characters long.");
            
            if (!Regex.IsMatch(password, "^(?=.*?[0-9])(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^0-9A-Za-z]).{8,32}$"))
                return await JsonError.HandleAsync(HttpContext, null, "InvalidPassword", 
                    "Password must be 8-32 characters long and contain the following: One number, one lowercase letter, and one special character.");
            
            var account = await Manager.GetByUsernameAsync(username);
            if (account != null)
                return await JsonError.HandleAsync(HttpContext, null, "InvalidUsername", "Username already exists.");

            var token = await Manager.CreateAccountAsync(username, password, HttpContext);
            return TokenResponse.Create(token);
        }
        catch (Exception ex)
        {
            return await JsonError.HandleInternalAsync(HttpContext, null, ex);
        }
    }
}