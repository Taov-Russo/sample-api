using Sample.Api.Infrastructure.Http;

namespace Sample.Api.Models.Task;

public abstract class TaskModelBase : JsonModel
{
    public string Name { get; set; }
    public int AverageTime { get; set; }
}