using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Sample.Api.Controllers;

[Route("[controller]")]
public class HealthController : Controller
{
    [SwaggerOperation(Summary = "Check the health of the application")]
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Index()
    {
        return Ok("Ok");
    }
}