using System.Threading.Tasks;
using Sample.Api.Infrastructure.Http;

namespace Sample.Api.Domain.SampleClient;

public interface ISampleClient
{
    Task<Response> Notify(int totalTime);
}