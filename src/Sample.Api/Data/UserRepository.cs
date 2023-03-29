using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Sample.Api.Models;

namespace Sample.Api.Data;

    public interface IUserRepository
    {
        Task<int> CreateUser(UserCreateModel model, Guid userId, byte[] passwordHash);
        Task<UserModel> GetUser(Guid userId);
        Task<UserModel> GetUser(string login);
        Task<UserModel> GetUser(string login, byte[] passwordHash);
        Task<int> DeleteUser(Guid userId);
    }

public class UserRepository : RepositoryBase, IUserRepository
{
    public UserRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> CreateUser(UserCreateModel model, Guid userId, byte[] passwordHash)
    {
        await using var connection = CreateConnection();
        return await connection.ExecuteAsync(@"
            INSERT INTO [User]
            (
                UserId,
                Login,
                PasswordHash
            )
            VALUES
            (
                @UserId,
                @Login,
                @PasswordHash
            )"
            , new
            {
                userId,
                model.Login,
                passwordHash
            });
    }

    public async Task<UserModel> GetUser(Guid userId)
    {
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(@"
                SELECT
                    UserId,
                    Login,
                    PasswordHash
                FROM [User]
                WHERE
                    UserId = @UserId", new { userId });
    }

    public async Task<UserModel> GetUser(string login)
    {
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(@"
                SELECT
                    UserId,
                    Login,
                    PasswordHash
                FROM [User]
                WHERE
                    Login = @Login",
            new { login });
    }

    public async Task<UserModel> GetUser(string login, byte[] passwordHash)
    {
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<UserModel>(@"
                SELECT
                    UserId,
                    Login,
                    PasswordHash
                FROM [User]
                WHERE
                    Login = @Login
                    AND PasswordHash = @PasswordHash"
            , new
            {
                login,
                passwordHash
            });
    }

    public async Task<byte[]> GetPasswordHashByUser(Guid userId)
    {
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<byte[]>(@"
            SELECT
              PasswordHash
            FROM [User]
            WHERE
              UserId = @userId", new { userId });
    }

    public async Task<int> DeleteUser(Guid userId)
    {
        await using var connection = CreateConnection();
        return await connection.ExecuteAsync(@"
            DELETE FROM [User]
            WHERE
              UserId = @userId", new { userId });;
    }
}