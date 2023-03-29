using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sample.Api.Data;
using Sample.Api.Infrastructure.Extensions;
using Sample.Api.Infrastructure.Http;
using Sample.Api.Models.Task;

namespace Sample.Api.Domain;

public interface ITaskManager
{
    Task<Response<int>> CreateTask(TaskCreateModel model);
    Task<Response<TaskModel>> GetTask(int taskId);
    Task<Response<EmptyModel>> UpdateTask(int taskId, TaskUpdateModel model);
    Task<Response<EmptyModel>> DeleteTask(int taskId);
}

public class TaskManager : ITaskManager
{
    private readonly ILogger<TaskManager> logger;
    private readonly ITaskRepository repository;
    private readonly ClaimsPrincipal user;

    public TaskManager(ILogger<TaskManager> logger, ITaskRepository repository, IHttpContextAccessor httpContextAccessor)
    {
        this.logger = logger;
        this.repository = repository;
        user = httpContextAccessor.HttpContext.User;
    }

    public async Task<Response<int>> CreateTask(TaskCreateModel model)
    {
        var beginTime = DateTime.Now;
        try
        {
            var taskId = await repository.CreateTask(model, user.GetUserId().Value);
            logger.LogInformation($"{Method.GetName()} success. Duration: {beginTime.GetDurationMilliseconds()} {model.ToLogString()}");
            return Response<int>.Ok(taskId);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} {model.ToLogString()}");
            return Response<int>.InternalServerError();
        }
    }

    public async Task<Response<TaskModel>> GetTask(int taskId)
    {
        var beginTime = DateTime.Now;
        try
        {
            var task = await repository.GetTask(taskId, user.GetUserId().Value);
            if (task == null)
            {
                logger.LogInformation($"{Method.GetName()} fail. Task not found. Duration: {beginTime.GetDurationMilliseconds()} TaskId: {taskId}");
                return Response<TaskModel>.NotFound();
            }
            return Response<TaskModel>.Ok(task);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} TaskId: {taskId}");
            return Response<TaskModel>.InternalServerError();
        }
    }

    public async Task<Response<EmptyModel>> UpdateTask(int taskId, TaskUpdateModel model)
    {
        var beginTime = DateTime.Now;
        try
        {
            var task = await repository.GetTask(taskId, user.GetUserId().Value);
            if (task == null)
            {
                logger.LogInformation($"{Method.GetName()} fail. Task not found. Duration: {beginTime.GetDurationMilliseconds()} TaskId: {taskId}, {model.ToLogString()}");
                return Response<EmptyModel>.NotFound();
            }

            var rowsCount = await repository.UpdateTask(taskId, model);
            if (rowsCount != 1)
            {
                logger.LogError($"{Method.GetName()} error. {rowsCount} rows updated. Duration: {beginTime.GetDurationMilliseconds()} TaskId: {taskId}, {model.ToLogString()}");
                return Response<EmptyModel>.NotFound();
            }

            logger.LogInformation($"{Method.GetName()} success. Duration: {beginTime.GetDurationMilliseconds()} TaskId: {taskId}, {model.ToLogString()}");
            return Response<EmptyModel>.Ok(new EmptyModel());
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} TaskId: {taskId}, {model.ToLogString()}");
            return Response<EmptyModel>.InternalServerError();
        }
    }

    public async Task<Response<EmptyModel>> DeleteTask(int taskId)
    {
        var beginTime = DateTime.Now;
        try
        {
            var rowsCount = await repository.DeleteTask(taskId, user.GetUserId().Value);
            if (rowsCount != 1)
            {
                logger.LogInformation($"{Method.GetName()} fail. Task not found. Duration: {beginTime.GetDurationMilliseconds()} TaskId: {taskId}");
                return Response<EmptyModel>.NotFound();
            }
            logger.LogInformation($"{Method.GetName()} success. Duration: {beginTime.GetDurationMilliseconds()} TaskId: {taskId}");
            return Response<EmptyModel>.Ok(new EmptyModel());
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} TaskId: {taskId}");
            return Response<EmptyModel>.InternalServerError();
        }
    }
}