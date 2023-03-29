using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Domain;
using Sample.Api.Infrastructure.Http;
using Sample.Api.Models.Task;
using Swashbuckle.AspNetCore.Annotations;
using ControllerBase = Sample.Api.Infrastructure.Http.ControllerBase;

namespace Sample.Api.Controllers;

[Authorize]
[Route("api/v1/[controller]")]
public class TasksContollers : ControllerBase
{
    private readonly ITaskManager manager;

    public TasksContollers(ITaskManager manager)
    {
        this.manager = manager;
    }

    [SwaggerOperation("Create a task")]
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateModel request)
    {
        return MakeResponse(await manager.CreateTask(request));
    }

    [SwaggerOperation("Get a task")]
    [HttpGet("{taskId:int}")]
    [ProducesResponseType(typeof(TaskModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTask([FromRoute] int taskId)
    {
        return MakeResponse(await manager.GetTask(taskId));
    }

    [SwaggerOperation("Update a task")]
    [HttpPut("{taskId:int}")]
    [ProducesResponseType(typeof(EmptyModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTask([FromRoute] int taskId, [FromBody] TaskUpdateModel request)
    {
        return MakeResponse(await manager.UpdateTask(taskId, request));
    }

    [SwaggerOperation("Delete a task")]
    [HttpDelete("{taskId:int}")]
    [ProducesResponseType(typeof(EmptyModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask([FromRoute] int taskId)
    {
        return MakeResponse(await manager.DeleteTask(taskId));
    }
}