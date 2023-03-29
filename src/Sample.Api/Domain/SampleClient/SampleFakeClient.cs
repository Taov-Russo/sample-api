using System.Threading.Tasks;
using Sample.Api.Infrastructure.Http;

namespace Sample.Api.Domain.SampleClient;

public class SampleFakeClient : ISampleClient
{
    public async Task<Response> Notify(int totalTime)
    {
        return Response.Ok();
    }
}