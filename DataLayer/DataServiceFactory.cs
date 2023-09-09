using _420DA3AS_Demo_Trois_Tiers.BusinessLayer;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer;

internal class DataServiceFactory {

    public static DataService GetDataService(DbConnectionOptions connectionOptions) {
        DbProviderFactory factory = GetFactory(connectionOptions);
        return new DataService(factory, connectionOptions.CreateConnection());
    }

    private static DbProviderFactory GetFactory(DbConnectionOptions connectionOptions) {
        Type factoryType = connectionOptions.DbProviderFactoryType ?? throw new Exception("null factory type reference in connection options.");
        FieldInfo fieldInfo = factoryType.GetField("Instance") ?? throw new Exception("factory type does not posess an 'Instance' field.");
        return fieldInfo.GetValue(null) as DbProviderFactory ?? throw new Exception("factory type 'Instance' property value is null or not an instance of DbProviderFactory");
        
    }
}
