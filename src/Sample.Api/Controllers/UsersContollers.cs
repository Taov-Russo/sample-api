using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Domain;
using Sample.Api.Infrastructure.Http;
using Sample.Api.Models;
using Sample.Api.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;
using ControllerBase = Sample.Api.Infrastructure.Http.ControllerBase;

namespace Sample.Api.Controllers;

[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserManager manager;

    public UsersController(IUserManager manager)
    {
        this.manager = manager;
    }

    [SwaggerOperation("Create an user")]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel<ErrorCode>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateModel model)
    {
        return MakeResponse(await manager.CreateUser(model));
    }

    [SwaggerOperation("Get an user")]
    [Authorize]
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser([FromRoute] Guid userId)
    {
        return MakeResponse(await manager.GetUser(userId));
    }

    [SwaggerOperation("Delete an user")]
    [Authorize]
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
    {
        return MakeResponse(await manager.DeleteUser(userId));
    }
}