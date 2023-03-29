using System.Collections.Generic;
using Sample.Api.Infrastructure.Http;

namespace Sample.Api.Models.Pipeline;

public class PipelineCreateModel : JsonModel
{
    public List<int> TaskIds { get; set; }
}