using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace SVN.Data.EntityFramework
{
    public static class ConnectionHelper
    {
        public static string GetConnectionString(ConnectionModel model)
        {
            var sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = model.DataSource,
                InitialCatalog = model.InitialCatalog,
                PersistSecurityInfo = true,
                UserID = model.UserID,
                Password = model.Password,
                MultipleActiveResultSets = true,
                ApplicationName = "EntityFramework"
            };
            var efBuilder = new EntityConnectionStringBuilder
            {
                Metadata = model.Metadata,
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = sqlBuilder.ConnectionString
            };
            return efBuilder.ConnectionString;
        }
    }
}