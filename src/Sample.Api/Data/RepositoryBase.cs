using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Sample.Api.Data
{
    public class RepositoryBase
    {
        private readonly IConfiguration configuration;

        protected RepositoryBase(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected SqlConnection CreateConnection()
        {
            return new(configuration.GetConnectionString("Database"));
        }
    }
}