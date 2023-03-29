using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sample.Api.Data;
using Sample.Api.Domain.SampleClient;
using Sample.Api.Infrastructure.Extensions;
using Sample.Api.Infrastructure.Http;
using Sample.Api.Models.Pipeline;
using Serilog;

namespace Sample.Api.Domain;

public interface IPipelineManager
{
    Task<Response<int>> RunPipeline(int pipelineId);
    Task<Response<int>> CreatePipeline(PipelineCreateModel request);
    Task<Response<PipelineModel>> GetPipeline(int pipelineId);
    Task<Response> UpdatePipeline(int pipelineId, PipelineUpdateModel request);
}

public class PipelineManager : IPipelineManager
{
    private readonly ILogger<PipelineManager> logger;
    private readonly IPipelineRepository repository;
    private readonly ClaimsPrincipal user;
    private readonly ISampleClient client;

    public PipelineManager(ILogger<PipelineManager> logger, IPipelineRepository repository, IHttpContextAccessor httpContextAccessor,
        ISampleClient client)
    {
        this.logger = logger;
        this.repository = repository;
        this.client = client;
        user = httpContextAccessor.HttpContext.User;
    }

    public async Task<Response<int>> RunPipeline(int pipelineId)
    {
        var beginTime = DateTime.Now;
        try
        {
            var pipeline = await repository.GetPipeline(pipelineId, user.GetUserId().Value);
            if (pipeline == null)
            {
                logger.LogInformation($"{Method.GetName()} fail. Pipeline not found. Duration: {beginTime.GetDurationMilliseconds()} PipelineId: {pipelineId}");
                return Response<int>.NotFound();
            }

            var totalTime = await repository.RunPipeline(pipelineId);

            var response = await client.Notify(totalTime);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"{Method.GetName()} error. SampleClient Notify StatusCode: {response.StatusCode}. Duration: {beginTime.GetDurationMilliseconds()} PipelineId: {pipelineId}");
                return Response<int>.Ok(totalTime);
            }

            logger.LogInformation($"{Method.GetName()} success. Duration: {beginTime.GetDurationMilliseconds()} PipelineId: {pipelineId}");
            return Response<int>.Ok(totalTime);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} PipelineId: {pipelineId}");
            return Response<int>.InternalServerError();
        }
    }

    public async Task<Response<int>> CreatePipeline(PipelineCreateModel request)
    {
        var beginTime = DateTime.Now;
        try
        {
            var pipelineId = await repository.CreatePipeline(request, user.GetUserId().Value);
            runPipeline(pipelineId);

            logger.LogInformation($"{Method.GetName()} success. Duration: {beginTime.GetDurationMilliseconds()} {request.ToLogString()}");
            return Response<int>.Ok(pipelineId);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} {request.ToLogString()}");
            return Response<int>.InternalServerError();
        }
    }

    public async Task<Response<PipelineModel>> GetPipeline(int pipelineId)
    {
        var beginTime = DateTime.Now;
        try
        {
            var pipeline = await repository.GetPipeline(pipelineId, user.GetUserId().Value);
            if (pipeline == null)
            {
                logger.LogInformation($"{Method.GetName()} fail. Pipeline not found. Duration: {beginTime.GetDurationMilliseconds()} PipelineId: {pipelineId}");
                return Response<PipelineModel>.NotFound();
            }
            return Response<PipelineModel>.Ok(pipeline);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} PipelineId: {pipelineId}");
            return Response<PipelineModel>.InternalServerError();
        }
    }

    public async Task<Response> UpdatePipeline(int pipelineId, PipelineUpdateModel request)
    {
        var beginTime = DateTime.Now;
        try
        {
            var pipeline = await repository.GetPipeline(pipelineId, user.GetUserId().Value);
            if (pipeline == null)
            {
                logger.LogInformation($"{Method.GetName()} fail. Pipeline not found. Duration: {beginTime.GetDurationMilliseconds()} PipelineId: {pipelineId}, {request.ToLogString()}");
                return Response.NotFound();
            }

            await repository.UpdatePepiline(pipelineId, request);
            runPipeline(pipelineId);

            logger.LogInformation($"{Method.GetName()} success. Duration: {beginTime.GetDurationMilliseconds()} PipelineId: {pipelineId}, {request.ToLogString()}");
            return Response<int>.Ok(pipelineId);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} PipelineId: {pipelineId}, {request.ToLogString()}");
            return Response<int>.InternalServerError();
        }
    }

    private void runPipeline(int pipelineId)
    {
        // In this case, I would prefer to use RMQ for asynchrony, but if I would add it to a project you had difficulty deploying and configuring it initially.
        // Trigger for running a pipeline.
        Task.Run(async () =>
        {
            await RunPipeline(pipelineId);
        });
    }
}