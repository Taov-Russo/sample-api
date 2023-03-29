using System.Net.Http;
using System.Threading.Tasks;
using Sample.Api.Infrastructure.Http;

namespace Sample.Api.Domain.SampleClient;

public class SampleClient : RestClientBase, ISampleClient
{
    public SampleClient(HttpClient httpClient) : base(httpClient, httpClient.BaseAddress.ToString())
    {
    }

    public async Task<Response> Notify(int totalTime)
    {
        return await Post("api/v1/notify", new {totalTime});
    }
}