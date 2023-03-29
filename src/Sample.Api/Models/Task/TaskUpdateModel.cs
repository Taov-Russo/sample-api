using Sample.Api.Infrastructure;
using Sample.Api.Infrastructure.Http;

namespace Sample.Api.Models.Task;

public class TaskUpdateModel : JsonModel
{
    public Field<string> Name { get; set; }
    public Field<int> AverageTime { get; set; }
}