using Microsoft.AspNetCore.Mvc;

namespace Boundless.Backend.Controllers;

[Route("/connection")]
public class ConnectionController : Controller
{
    public IActionResult Index()
    {
        IsConnected = true;
        return new JsonResult(new { result = "connected" });
    }
}