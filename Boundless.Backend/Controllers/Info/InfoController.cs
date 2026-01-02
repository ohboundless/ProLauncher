using System.Text.Json;
using System.Text.RegularExpressions;
using Boundless.Backend.Controllers.Account.Models;
using Boundless.Backend.Exceptions;
using Boundless.Backend.Managers.Accounts;
using Boundless.Backend.Managers.Errors.Models;
using Boundless.Backend.Managers.Tokens.Models;
using Boundless.Backend.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Boundless.Backend.Controllers.Account;

[ApiController]
[Route("api/info")]
public class InfoController : ControllerBase
{
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus() 
    {
        try
        {
            CheckConfig();
            return new JsonResult(Config!.Status);
        }
        catch (Exception ex)
        {
            return await JsonError.HandleInternalAsync(HttpContext, null, ex);
        }
    }
}
