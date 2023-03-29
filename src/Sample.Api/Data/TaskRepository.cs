using System;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Sample.Api.Models;
using Sample.Api.Models.Task;

namespace Sample.Api.Data;

    public interface ITaskRepository
    {
        Task<int> CreateTask(TaskCreateModel model, Guid userId);
        Task<TaskModel> GetTask(int taskId, Guid userId);
        Task<int> UpdateTask(int taskId, TaskUpdateModel model);
        Task<int> DeleteTask(int taskId, Guid userId);
    }

public class TaskRepository : RepositoryBase, ITaskRepository
{
    public TaskRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> CreateTask(TaskCreateModel model, Guid userId)
    {
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<int>(@"
            INSERT INTO [Task]
            (
                [Name],
                AverageTime,
                UserId
            )
            OUTPUT
                inserted.Id
            VALUES
            (
                @Name,
                @AverageTime,
                @UserId
            )"
            , new
            {
                model.Name,
                model.AverageTime,
                userId
            });
    }

    public async Task<TaskModel> GetTask(int taskId, Guid userId)
    {
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<TaskModel>(@"
            SELECT
                [Id],
                [Name],
                AverageTime
            FROM [Task]
            WHERE
                Id = @TaskId
                AND UserId = @UserId"
            , new
            {
                taskId,
                userId
            });
    }

    public async Task<int> UpdateTask(int taskId, TaskUpdateModel model)
    {
        await using var connection = CreateConnection();
        var updatedQuery = new StringBuilder();
        var parameters = new DynamicParameters();
        parameters.Add("@TaskId", taskId);

        if (model.Name != null)
        {
            updatedQuery.AppendLine(",[Name] = @Name");
            parameters.Add("@Name", model.Name.Value);
        }
        if (model.AverageTime != null)
        {
            updatedQuery.AppendLine(",AverageTime = @AverageTime");
            parameters.Add("@AverageTime", model.AverageTime.Value);
        }

        return await connection.ExecuteAsync($@"
                UPDATE Task
                SET
                    UserId = UserId
                    {updatedQuery}
                WHERE
                    Id = @TaskId", parameters);
    }

    public async Task<int> DeleteTask(int taskId, Guid userId)
    {
        await using var connection = CreateConnection();
        return await connection.ExecuteAsync(@"
            DELETE FROM [Task]
            WHERE
                Id = @TaskId
                AND UserId = @UserId"
            , new
            {
                taskId,
                userId
            });;
    }
}