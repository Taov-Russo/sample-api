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

public interface IAuthorizationManager
{
    Task<Response<LoginResponse, ErrorModel<ErrorCode>>> Login(LoginRequest request);
}

public class AuthorizationManager : IAuthorizationManager
{
    private readonly ILogger<AuthorizationManager> logger;
    private readonly IUserRepository repository;
    private readonly IJwtManager jwtManager;

    public AuthorizationManager(ILogger<AuthorizationManager> logger, IUserRepository repository, IJwtManager jwtManager)
    {
        this.logger = logger;
        this.repository = repository;
        this.jwtManager = jwtManager;
    }

    public async Task<Response<LoginResponse, ErrorModel<ErrorCode>>> Login(LoginRequest request)
    {
        var beginTime = DateTime.Now;
        try
        {
            var passwordHash = HashManager.GetPasswordHash512(request.Password);
            var user = await repository.GetUser(request.Login, passwordHash);
            if (user == null)
            {
                logger.LogInformation($"{Method.GetName()} failed. User not found. Duration: {beginTime.GetDurationMilliseconds()} Login: {request.Login}");
                return Response<LoginResponse, ErrorModel<ErrorCode>>.NotFound();
            }

            
            logger.LogInformation($"{Method.GetName()} success. Duration: {beginTime.GetDurationMilliseconds()} Login: {request.Login}");
            return Response<LoginResponse, ErrorModel<ErrorCode>>.Ok(new LoginResponse
            {
                Token = jwtManager.CreateAuthToken(user.UserId)
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{Method.GetName()} error. Duration: {beginTime.GetDurationMilliseconds()} Login: {request.Login}");
            return Response<LoginResponse, ErrorModel<ErrorCode>>.InternalServerError();
        }
    }
}