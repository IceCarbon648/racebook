using Microsoft.Data.SqlClient;
using System.Data;

namespace racebookApi.Data
{
    public class DapperContext : IDapperContext
    {
        public IDbConnection CreateConnection()
            => new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"));
    }
}