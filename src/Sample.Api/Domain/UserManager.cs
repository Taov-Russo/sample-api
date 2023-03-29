using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sample.Api.Data;
using Sample.Api.Infrastructure;
using Sample.Api.Infrastructure.Extensions;
using Sample.Api.Infrastructure.Http;
using Sample.Api.Models;
using Sample.Api.Models.Enums;

namespace Sample.Api.Domain;

public interface IUserManager
    {
        Task<Response<Guid, ErrorModel<ErrorCode>>> CreateUser(UserCreateModel model);
        Task<Response<UserModel>> GetUser(Guid userId);
        Task<Response> DeleteUser(Guid userId);
    }

public class UserManager : IUserManager
{
    private readonly ILogger<UserManager> logger;
    private readonly IUserRepository repository;

    public UserManager(ILogger<UserManager> logger, IUserRepository repository)
    {
        this.logger = logger;
        this.repository = repository;
    }

    public async Task<Response<Guid, ErrorModel<ErrorCode>>> CreateUser(UserCreateModel model)
    {
        var beginTime = DateTime.Now;
        try
        {
            var user = await repository.GetUser(model.Login);
            if (user != null)
            {
                logger.LogInformation($"{Method.GetName()} fail. Duration: {beginTime.GetDurationMilliseconds()} {model.ToLogString()}");
                return Response<Guid, ErrorModel<ErrorCode>>.BadRequest(new ErrorModel<ErrorCode>(ErrorCode.UserAlreadyExists));
            }

            var passwordHash = HashManager.GetPasswordHash512(model.Password);

            var userId = Guid.NewGuid();
            var rowsCount = await repository.CreateUser(model, userId, passwordHash);
            if (rowsCount != 1)
            {
                logger.LogError($"{Method.GetName()} error. {rowsCount} rows inserted. Duration: {beginTime.GetDurationMilliseconds()} {model.ToLogString()}");
                return Response<Guid, ErrorModel<ErrorCode>>.InternalServerError();
            }

            logger.LogInformation($"{Method.GetName()} success. Duration: {beginTime.GetDurationMilliseconds()} UserId: {userId}, {model.ToLogString()}");
            return Response<Guid, ErrorModel<ErrorCode>>.Ok(userId);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} {model.ToLogString()}");
            return Response<Guid, ErrorModel<ErrorCode>>.InternalServerError();
        }
    }

    public async Task<Response<UserModel>> GetUser(Guid userId)
    {
        var beginTime = DateTime.Now;
        try
        {
            var user = await repository.GetUser(userId);
            if (user == null)
            {
                logger.LogInformation($"{Method.GetName()} fail. User not found. Duration: {beginTime.GetDurationMilliseconds()} UserId: {userId}");
                return Response<UserModel>.NotFound();
            }

            return Response<UserModel>.Ok(user);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} UserId: {userId}");
            return Response<UserModel>.InternalServerError();
        }
    }

    public async Task<Response> DeleteUser(Guid userId)
    {
        var beginTime = DateTime.Now;
        try
        {
            var user = await repository.GetUser(userId);
            if (user == null)
            {
                logger.LogInformation($"{Method.GetName()} fail. User not found. Duration: {beginTime.GetDurationMilliseconds()} UserId: {userId}");
                return Response<UserModel>.NotFound();
            }

            var rowsCount = await repository.DeleteUser(userId);
            if (rowsCount != 1)
            {
                logger.LogInformation($"{Method.GetName()} error. {rowsCount} rows deleted. Duration: {beginTime.GetDurationMilliseconds()} UserId: {userId}");
                return Response.InternalServerError();
            }

            logger.LogInformation($"{Method.GetName()} success. Duration: {beginTime.GetDurationMilliseconds()} UserId: {userId}");
            return Response.Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} UserId: {userId}");
            return Response.InternalServerError();
        }
    }
}