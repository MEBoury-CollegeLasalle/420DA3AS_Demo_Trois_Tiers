using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer;

internal class DataServiceFactory {

    public static DataService GetDataService(DbConnectionOptions connectionOptions) {
        DbProviderFactory factory = GetFactory(connectionOptions);
        switch (connectionOptions.Type) {
            case DataServiceType.SQL_SERVER:
                return new DataService(factory);
            case DataServiceType.MYSQL:
            case DataServiceType.ORACLE:
            default:
                throw new NotImplementedException();
        }
    }

    private static DbProviderFactory GetFactory(DbConnectionOptions connectionOptions) {
        DbConnection connection = connectionOptions.GetConnection();
        return connection.GetType()
            .GetProperty("DbProviderFactory", BindingFlags.NonPublic | BindingFlags.Instance)?
            .GetValue(connection) is not DbProviderFactory factory
            ? throw new Exception($"Failure to create DbProviderFactory instance for type [{connectionOptions.Type}].")
            : factory;

    }
}
