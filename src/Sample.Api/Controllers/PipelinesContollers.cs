using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Domain;
using Sample.Api.Models.Pipeline;
using Swashbuckle.AspNetCore.Annotations;
using ControllerBase = Sample.Api.Infrastructure.Http.ControllerBase;

namespace Sample.Api.Controllers;

[Authorize]
[Route("api/v1/[controller]")]
public class PipelinesContollers : ControllerBase
{
    private readonly IPipelineManager manager;

    public PipelinesContollers(IPipelineManager manager)
    {
        this.manager = manager;
    }

    [SwaggerOperation("Run a pipeline")]
    [HttpPost("{pipelineId:int}/run")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RunPipeline([FromRoute] int pipelineId)
    {
        return MakeResponse(await manager.RunPipeline(pipelineId));
    }

    [SwaggerOperation("Create a pipeline")]
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePipeline([FromBody] PipelineCreateModel request)
    {
        return MakeResponse(await manager.CreatePipeline(request));
    }

    [SwaggerOperation("Get a pipeline")]
    [HttpGet("{pipelineId:int}")]
    [ProducesResponseType(typeof(PipelineModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPipeline([FromRoute] int pipelineId)
    {
        return MakeResponse(await manager.GetPipeline(pipelineId));
    }

    [SwaggerOperation("Update a pipeline")]
    [HttpPut("{pipelineId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePipeline([FromRoute] int pipelineId, [FromBody] PipelineUpdateModel request)
    {
        return MakeResponse(await manager.UpdatePipeline(pipelineId, request));
    }
}