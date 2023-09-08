using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer;

internal class DataServiceFactory {

    public static DataService GetDataService(DbConnectionOptions connectionOptions) {
        DbProviderFactory factory = GetFactory(connectionOptions);
        return new DataService(factory, connectionOptions.GetConnection());
    }

    private static DbProviderFactory GetFactory(DbConnectionOptions connectionOptions) {
        DbProviderFactory factory;
        switch (connectionOptions.Type) {
            case DataServiceType.SQL_SERVER:
                factory = Microsoft.Data.SqlClient.SqlClientFactory.Instance;
                break;
            case DataServiceType.MYSQL:
                factory = MySql.Data.MySqlClient.MySqlClientFactory.Instance;
                break;
            case DataServiceType.ORACLE:
                factory = Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance;
                break;
            default:
                throw new Exception($"DataServiceType [{Enum.GetName(typeof(DataServiceType), connectionOptions.Type)}] is not supported.");
        }
        
        return factory;

    }
}
