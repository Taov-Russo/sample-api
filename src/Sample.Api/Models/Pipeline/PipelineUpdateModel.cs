using System.Collections.Generic;
using Sample.Api.Infrastructure.Http;

namespace Sample.Api.Models.Pipeline;

public class PipelineUpdateModel : JsonModel
{
    public List<int> AddedTaskIds { get; set; }
    public List<int> DeletedTaskIds { get; set; }
}