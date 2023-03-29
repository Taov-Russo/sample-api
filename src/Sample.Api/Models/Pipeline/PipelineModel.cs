using System.Collections.Generic;
using Sample.Api.Infrastructure.Http;

namespace Sample.Api.Models.Pipeline;

public class PipelineModel : JsonModel
{
    public int Id { get; set; }
    public List<int> TaskIds { get; set; }
}