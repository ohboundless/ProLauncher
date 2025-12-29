using Boundless.Backend.Controllers.Account.Models;
using Boundless.Backend.Exceptions;
using Boundless.Backend.Managers.Accounts;
using Boundless.Backend.Managers.Tokens.Models;
using Microsoft.AspNetCore.Mvc;

namespace Boundless.Backend.Controllers.Account;

[ApiController]
[Route("account")]
public class AccountController(AccountManager Manager) : ControllerBase
{
    [HttpGet("token")]
    public async Task<ContentResult> CreateToken([FromForm] string username, [FromForm] string password /* sha256password */) 
    {
        var account = await Manager.GetByUsernameAsync(username);
        if (account == null)
            throw new InvalidUsernameException(username);

        if (account.Password != password)
            throw new IncorrectPasswordException(username);

        var token = Token.Generate(account.AccountId, HttpContext);
        await TokenInstance!.AddAsync(token);
        
        return TokenResponse.Create(token);
    }
    
    [HttpGet("create")]
    public async Task<ContentResult> CreateAccount([FromForm] string username, [FromForm] string password) // sha256password
    {
        var account = await Manager.GetByUsernameAsync(username);
        if (account != null)
            throw new AccountAlreadyExistsException(username);

        var token = await Manager.CreateAccountAsync(username, password, HttpContext);
        return TokenResponse.Create(token);
    }
}