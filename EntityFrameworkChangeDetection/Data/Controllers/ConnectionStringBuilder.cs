using Data.Constants;
using System.Data.SqlClient;

namespace Data.Controllers
{
    public static class ConnectionStringBuilder
    {
        public static string CreateDatabaseConnectionString()
        {
            var sqlBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = DatabaseConnection.Server,
                InitialCatalog = DatabaseConnection.ApplicationDatabase,
                PersistSecurityInfo = true,
                IntegratedSecurity = false,
                MultipleActiveResultSets = true,
                UserID = DatabaseConnection.UserId,
                Password = DatabaseConnection.Password
            };

            return sqlBuilder.ConnectionString;
        }
    }
}
