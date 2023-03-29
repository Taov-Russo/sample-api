using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Sample.Api.Models.Pipeline;

namespace Sample.Api.Data;

    public interface IPipelineRepository
    {
        Task<int> RunPipeline(int pipelineId);
        Task<int> CreatePipeline(PipelineCreateModel model, Guid userId);
        Task<PipelineModel> GetPipeline(int pipelineId, Guid userId);
        Task UpdatePepiline(int pipelineId, PipelineUpdateModel model);
    }

public class PipelineRepository : RepositoryBase, IPipelineRepository
{
    public PipelineRepository(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<int> RunPipeline(int pipelineId)
    {
        await using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<int>(@"
            INSERT INTO PipelineRunHistory
            (
                PipelineId,
                TotalExecutionTime
            )
            OUTPUT
                inserted.TotalExecutionTime
            SELECT
                PipelineId,
                SUM(AverageTime)
            FROM Pipeline p
            LEFT JOIN PipelineTask pt ON pt.PipelineId = p.[Id]
            LEFT JOIN Task t ON t.[Id] = pt.TaskId
            WHERE
                p.Id = @PipelineId
            GROUP BY PipelineId"
            , new
            {
                pipelineId
            });
    }

    public async Task<int> CreatePipeline(PipelineCreateModel model, Guid userId)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var tx = connection.BeginTransaction();
        try
        {
            var pipelineId = await connection.QueryFirstOrDefaultAsync<int>(@"
                INSERT INTO [PipeLine]
                (
                    UserId
                )
                OUTPUT
                    inserted.[Id]
                VALUES
                (
                    @UserId
                )"
                , new
                {
                    userId
                }, tx);

            if (model.TaskIds != null && model.TaskIds.Any())
            {
                var relationRaws = model.TaskIds.Select(t =>
                    new
                    {
                        pipelineId,
                        taskId = t
                    }).ToList();
                await connection.ExecuteAsync(@"
                    INSERT INTO PipelineTask
                    (
                        PipelineId,
                        TaskId
                    )
                    VALUES
                    (
                        @PipelineId,
                        @TaskId
                    )", relationRaws, tx);
            }

            tx.Commit();
            return pipelineId;
        }
        catch (Exception)
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<PipelineModel> GetPipeline(int pipelineId, Guid userId)
    {
        await using var connection = CreateConnection();
        var taskIds = await connection.QueryAsync<int>(@"
            SELECT
                TaskId
            FROM Pipeline p
            LEFT JOIN PipelineTask pt ON pt.PipelineId = p.[Id]
            WHERE
                Id = @PipelineId
                AND UserId = @UserId"
            , new
            {
                pipelineId,
                userId
            });

        return new PipelineModel
        {
            Id = pipelineId,
            TaskIds = taskIds.ToList()
        };
    }

    public async Task UpdatePepiline(int pipelineId, PipelineUpdateModel model)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@PipelineId", pipelineId);

        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var tx = connection.BeginTransaction();
        try
        {
            if (model.AddedTaskIds != null && model.AddedTaskIds.Any())
            {
                var addedRaws = model.AddedTaskIds.Select(t =>
                    new
                    {
                        pipelineId,
                        taskId = t
                    }).ToList();
                await connection.ExecuteAsync(@"
                    INSERT INTO PipelineTask
                    (
                        PipelineId,
                        TaskId
                    )
                    VALUES
                    (
                        @PipelineId,
                        @TaskId
                    )", addedRaws, tx);
            }

            if (model.DeletedTaskIds != null && model.DeletedTaskIds.Any())
            {
                await connection.ExecuteAsync(@"
                    DELETE FROM PipelineTask
                    WHERE
                        [PipelineId] = @PipelineId
                        AND TaskId IN @DeletedTaskIds"
                    , new
                    {
                        pipelineId,
                        model.DeletedTaskIds
                    }, tx);
            }

            tx.Commit();
        }
        catch (Exception)
        {
            tx.Rollback();
            throw;
        }
    }
}