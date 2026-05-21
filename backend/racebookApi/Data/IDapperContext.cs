using System.Data;

namespace racebookApi.Data
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}