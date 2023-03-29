using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Domain;
using Sample.Api.Infrastructure.Http;
using Sample.Api.Models;
using Sample.Api.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;
using ControllerBase = Sample.Api.Infrastructure.Http.ControllerBase;

namespace Sample.Api.Controllers;

[Route("api/v3/auth")]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationManager authorizationManager;

    public AuthorizationController(IAuthorizationManager authorizationManager)
    {
        this.authorizationManager = authorizationManager;
    }

    [SwaggerOperation(Summary = "Perform login and password authorization")]
    [HttpPost]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel<ErrorCode>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        return MakeResponse(await authorizationManager.Login(request));
    }
}