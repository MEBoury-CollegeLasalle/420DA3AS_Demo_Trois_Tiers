using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer;

/// <summary>
/// Énumération
/// </summary>
public enum DataServiceType {
    SQL_SERVER,
    MYSQL,
    ORACLE
}
public enum DbConnectionProtocol {
    TCP,
    LPC,
    NAMED_PIPES
}
public class DbConnectionOptions {
    private const string SQLSERVER_TYPE_STRING = "SQL_SERVER";
    private const string MYSQL_TYPE_STRING = "MYSQL";
    private const string ORACLE_TYPE_STRING = "ORACLE";

    private static readonly Dictionary<string, Tuple<Type,Type>> SUPPORTED_DB_PROVIDERS = new Dictionary<string, Tuple<Type, Type>>() {
        { SQLSERVER_TYPE_STRING, new Tuple<Type, Type>(
            typeof(Microsoft.Data.SqlClient.SqlConnection), 
            typeof(Microsoft.Data.SqlClient.SqlClientFactory)
            ) },
        { MYSQL_TYPE_STRING, new Tuple<Type, Type>(
            typeof(MySql.Data.MySqlClient.MySqlConnection), 
            typeof(MySql.Data.MySqlClient.MySqlClientFactory)
            ) },
        { ORACLE_TYPE_STRING, new Tuple<Type, Type>(
            typeof(Oracle.ManagedDataAccess.Client.OracleConnection), 
            typeof(Oracle.ManagedDataAccess.Client.OracleClientFactory)
            ) }
    };

    private const string TCP_PROTOCOL_STRING = "TCP";
    private const string TCP_PROTOCOL_STRING_VALUE = "tcp";
    private const string LPC_PROTOCOL_STRING = "LPC";
    private const string LPC_PROTOCOL_STRING_VALUE = "lpc";
    private const string NAMED_PIPES_PROTOCOL_STRING = "NAMED_PIPES";
    private const string NAMED_PIPES_PROTOCOL_STRING_VALUE = "np";

    private static readonly Dictionary<string, string> SUPPORTED_CONNECTION_PROTOCOLS = new Dictionary<string, string>() {
        { TCP_PROTOCOL_STRING, TCP_PROTOCOL_STRING_VALUE },
        { LPC_PROTOCOL_STRING, LPC_PROTOCOL_STRING_VALUE },
        { NAMED_PIPES_PROTOCOL_STRING, NAMED_PIPES_PROTOCOL_STRING_VALUE }
    };

    private const string SERVER_HOST_OPTION_KEY = "server";
    private const string LOCALINSTANCE_HOST_VALUE = ".";
    private const string LOCALIP_HOST_VALUE = "127.0.0.1";
    private const string LOCALHOST_HOST_VALUE = "localhost";
    private const int SQLSERVER_DEFAULT_PORT = 1433;
    private const int MYSQL_DEFAULT_PORT = 3306;
    private const int ORACLE_DEFAULT_PORT = 1521;
    private const string ISEC_OPTION_KEY = "Integrated Security";
    private const string USER_OPTION_KEY = "User ID";
    private const string PW_OPTION_KEY = "Password";
    private const string DATABASE_OPTION_KEY = "Database";
    private const string ASYNC_OPTION_KEY = "Async";
    private const string TIMEOUT_OPTION_KEY = "Timeout";

    private int? port = null;
    private string? userName = null;
    private string? userPassword = null;
    private string? databaseName = null;
    private int connectionTimeoutSeconds = 15;

    public string? Type { get; private set; } = null;
    [JsonIgnore]
    public Type? DbProviderConnectionType { get; private set; } = null;
    [JsonIgnore]
    public Type? DbProviderFactoryType { get; private set; } = null;
    public string? Protocol { get; set; } = null;
    public string? Host { get; set; } = null;
    public int? Port {
        get { return this.port; }
        set {
            if (value is int && (value > 65536 || value < 0)) {
                throw new ArgumentException("Port number must be in the range 0 to 65536 inclusively.");
            }
            this.port = value;
        }
    }
    public string? InstanceName { get; set; } = null;
    public string? PipeName { get; set; } = null;
    public bool UseIntegratedSecurity { get; set; } = true;
    public string? UserName {
        private get { return this.userName; }
        set {
            if (value is not null && value.Length > 128) {
                throw new ArgumentException("User name must be 128 characters or less.");
            }
            this.userName = value;
        }
    }
    public string? UserPassword {
        private get { return this.userPassword; }
        set {
            if (value is not null && value.Length > 128) {
                throw new ArgumentException("User password must be 128 characters or less.");
            }
            this.userPassword = value;
        }
    }
    public string? DatabaseName {
        get { return this.databaseName; }
        set {
            if (value is not null && value.Length > 128) {
                throw new ArgumentException("Database name must be 128 characters or less.");
            }
            this.databaseName = value;
        }
    }
    public bool UsesAsync { get; set; } = false;
    public int ConnectionTimeoutSeconds {
        get { return this.connectionTimeoutSeconds; }
        set {
            if (value < 0 || value > int.MaxValue) {
                throw new ArgumentException("Timeout value in seconds must be between 0 and 2147483647 inclusively.");
            }
            this.connectionTimeoutSeconds = value;
        }
    }
    public Dictionary<string, string> OtherOptions { get; private set; }

    private DbConnectionOptions() {
        // other options dictionary initialization
        this.OtherOptions = new Dictionary<string, string>();
    }

    public static DbConnectionOptions LoadFromAppConfigs() {
        return new DbConnectionOptions().LoadApplicationConfigurations();
    }

    public void AddOption(string key, string value) {
        this.OtherOptions.Add(key, value);
    }

    public bool RemoveOption(string key) {
        return this.OtherOptions.Remove(key);
    }

    private DbConnectionOptions LoadApplicationConfigurations() {
        DebuggerService.Warn($"Reading database configurations...");

        // Database type and database type support check
        DebuggerService.Warn($"Analyzing database type configuration...");
        string dbTypeConfig = ConfigurationManager.AppSettings["dbType"]
            ?? throw new Exception("Application configurations do not contain the required [dbType] configuration key.");
        DebuggerService.Info($"Database type configuration detected: [{dbTypeConfig}].");
        if (!SUPPORTED_DB_PROVIDERS.ContainsKey(dbTypeConfig)) {
            string supportedTypes = String.Join(", ", SUPPORTED_DB_PROVIDERS.Keys.Select(typeString => {
                return typeString.ToString();
            }));
            throw new Exception($"Application does not support the database type specified ({dbTypeConfig}). Types supported: [{supportedTypes}]");
        }
        this.Type = dbTypeConfig;
        this.DbProviderConnectionType = SUPPORTED_DB_PROVIDERS[dbTypeConfig].Item1;
        this.DbProviderFactoryType = SUPPORTED_DB_PROVIDERS[dbTypeConfig].Item2;
        DebuggerService.Success($"Database type configuration successful! Provider: [{this.DbProviderFactoryType.Namespace}].");


        // Loop through all the application configurations settings (App.config 'sppSettings' section)
        foreach (string? key in ConfigurationManager.AppSettings.AllKeys) {
            string? value = ConfigurationManager.AppSettings[key];

            // for each setting with a key and a value
            if (key is not null && value is not null) {

                // Handle only settings whose key string starts with 'db' that is NOT 'dbType' (already handled)
                if (key.StartsWith("db") && key != "dbType") {

                    bool parseAttempt;
                    switch (key) {
                        case "dbProtocol":
                            DebuggerService.Warn($"Analyzing database connection protocol configuration...");
                            DebuggerService.Info($"Database connection protocol configuration detected: [{value}].");
                            if (!SUPPORTED_CONNECTION_PROTOCOLS.ContainsKey(key)) {
                                string supportedProtocols = String.Join(", ", SUPPORTED_CONNECTION_PROTOCOLS.Keys.Select(protocolString => {
                                    return protocolString.ToString();
                                }));
                                throw new Exception($"Application does not support the connection protocol specified ({key}). Types supported: [{supportedProtocols}]");
                            }
                            this.Protocol = SUPPORTED_CONNECTION_PROTOCOLS[key];
                            DebuggerService.Success($"Database connection protocol configuration successful! [{this.Protocol}]");
                            break;
                        case "dbHost":
                            DebuggerService.Warn($"Analyzing database host configuration...");
                            this.Host = value;
                            DebuggerService.Success($"Database host configuration set: [{this.Host}].");
                            break;
                        case "dbPort":
                            int port;
                            parseAttempt = int.TryParse(value, out port);
                            if (parseAttempt) {
                                this.Port = port;
                            }
                            break;
                        case "dbInstanceName":
                            DebuggerService.Warn($"Analyzing database instance name configuration...");
                            this.InstanceName = value;
                            DebuggerService.Success($"Database instance name configuration set: [{this.InstanceName}].");
                            break;
                        case "dbUsesIntegratedSecurity":
                            bool usesIntegratedSecurity;
                            parseAttempt = bool.TryParse(value, out usesIntegratedSecurity);
                            if (parseAttempt) {
                                this.UseIntegratedSecurity = usesIntegratedSecurity;
                            }
                            break;
                        case "dbUserName":
                            DebuggerService.Warn($"Analyzing database user name configuration...");
                            this.UserName = value;
                            DebuggerService.Success($"Database user name configuration set: [{this.UserName}].");
                            break;
                        case "dbUserPassword":
                            DebuggerService.Warn($"Analyzing database user password configuration...");
                            this.UserPassword = value;
                            DebuggerService.Success($"Database user password configuration set: [{new string('*', value.Length)}].");
                            break;
                        case "dbDatabaseName":
                            DebuggerService.Warn($"Analyzing database name configuration...");
                            this.DatabaseName = value;
                            DebuggerService.Success($"Database name configuration set: [{this.DatabaseName}].");
                            break;
                        case "dbUsesAsync":
                            bool usesAsync;
                            parseAttempt = bool.TryParse(value, out usesAsync);
                            if (parseAttempt) {
                                this.UsesAsync = usesAsync;
                            }
                            break;
                        case "dbPipeName":
                            int connTimeout;
                            parseAttempt = int.TryParse(value, out connTimeout);
                            if (parseAttempt) {
                                this.ConnectionTimeoutSeconds = connTimeout;
                            }
                            break;
                        default:
                            DebuggerService.Warn($"Analyzing connection unmanaged option configuration...");
                            string optionKey = key.Substring(2);
                            this.AddOption(optionKey, value);
                            DebuggerService.Success($"Database connection unmanaged option configuration set: [{optionKey} -> {value}].");
                            break;
                    }
                }
            }
        }

        // DEFAULTS
        // FIXME: Check this part. Might cause problems, I've read somewhere that when using explicit protocol
        // it must be manually configured to be allowed in the server
        /*
        if (this.Protocol is null) {
            if (this.PipeName is not null) {
                this.Protocol = SUPPORTED_CONNECTION_PROTOCOLS[NAMED_PIPES_PROTOCOL_STRING];
                
            } else {
                this.Protocol = SUPPORTED_CONNECTION_PROTOCOLS[TCP_PROTOCOL_STRING];
            }
            DebuggerService.Warn($"No database host configuration found. Defaulting to [{this.Protocol}].");
        }
        */
        if (this.Host is null) {
            if (this.InstanceName is not null || this.PipeName is not null) {
                // default host for local instance
                this.Host = LOCALINSTANCE_HOST_VALUE;

            } else if (this.Port is not null) {
                // default host (localhost IP address) for port-configured instance
                this.Host = LOCALIP_HOST_VALUE;

            } else {
                // default host (localhost)
                this.Host = LOCALHOST_HOST_VALUE;

            }
            DebuggerService.Warn($"No database host configuration found. Defaulting to [{this.Host}].");
        }
        if (this.Port is null && this.Protocol == SUPPORTED_CONNECTION_PROTOCOLS[TCP_PROTOCOL_STRING]) {
            // Default port
            switch (this.Type) {
                case SQLSERVER_TYPE_STRING:
                    this.Port = SQLSERVER_DEFAULT_PORT;
                    break;
                case MYSQL_TYPE_STRING:
                    this.Port = MYSQL_DEFAULT_PORT;
                    break;
                case ORACLE_TYPE_STRING:
                    this.Port = ORACLE_DEFAULT_PORT;
                    break;
            }
            DebuggerService.Warn($"No database port configuration found. Defaulting to default port [{this.Port}] for type [{this.Type}].");
        }

        DebuggerService.Success("Database connection configuration options loaded!");
        DebuggerService.Log(this.ToString());

        return this;
    }

    public virtual string GetConnectionString() {

        StringBuilder sb = new StringBuilder();

        // Handling protocol / host / port|instance
        _ = sb.Append(SERVER_HOST_OPTION_KEY).Append('='); // server=

        if (this.Protocol is not null) {
            _ = sb.Append(this.Protocol)
                .Append(':');
            if (this.Protocol == NAMED_PIPES_PROTOCOL_STRING_VALUE) {
                _ = sb.Append(@"\\").Append(this.Host);
                if (this.PipeName is not null) {
                    _ = sb.Append("\\pipe\\").Append(this.PipeName);
                }

            } else {
                _ = sb.Append(this.Host);
                if (this.InstanceName is not null) {
                    _ = sb.Append('\\').Append(this.InstanceName);
                } 
                if (this.Port is not null) {
                    _ = sb.Append(',').Append(this.Port.ToString());
                }

            }
            switch (this.Protocol) {
                case NAMED_PIPES_PROTOCOL_STRING_VALUE:
                    break;
                case LPC_PROTOCOL_STRING_VALUE:
                case TCP_PROTOCOL_STRING_VALUE:
                default:
                    break;
            }
        } else {
            _ = sb.Append(this.Host);
            if (this.InstanceName is not null) {
                _ = sb.Append('\\').Append(this.InstanceName);
            } else if (this.Port is not null) {
                _ = sb.Append(',').Append(this.Port.ToString());
            }
        }
        _ = sb.Append(';');

        // Handle user, password or integrated security
        if (this.UseIntegratedSecurity) {
            _ = sb.Append(ISEC_OPTION_KEY).Append("=true;");
        } else {
            _ = this.UserName ?? throw new Exception("Connections not using integrated security require a user name.");
            _ = this.UserPassword ?? throw new Exception("Connections not using integrated security require a user password.");

            _ = sb.Append(ISEC_OPTION_KEY).Append("=false;")
                .Append(USER_OPTION_KEY).Append('=').Append(this.UserName).Append(';')
                .Append(PW_OPTION_KEY).Append('=').Append(this.UserPassword).Append(';');
        }

        // Handle database
        if (this.DatabaseName is not null) {
            _ = sb.Append(DATABASE_OPTION_KEY).Append('=').Append(this.DatabaseName).Append(';');
        }

        // Handle asynchronous operation enabling
        if (this.UsesAsync) {
            _ = sb.Append(ASYNC_OPTION_KEY).Append("=true;");
        }

        // Handle connection timeout
        _ = sb.Append(TIMEOUT_OPTION_KEY).Append('=').Append(this.ConnectionTimeoutSeconds).Append(';');

        // handle other options
        foreach (KeyValuePair<string, string> kvp in this.OtherOptions) {
            _ = sb.Append(kvp.Key).Append('=').Append(kvp.Value).Append(';');
        }

        return sb.ToString();
    }

    public virtual DbConnection CreateConnection() {


        // Create & return the connection objects for the supported connection types
        string connectionString = this.GetConnectionString();
        Type connectionType = this.DbProviderConnectionType ?? throw new Exception("null connection type reference in connection options.");
        ConstructorInfo constrInfo = connectionType.GetConstructors()[0] ?? throw new Exception("connection type does not posess a constructor with a single string parameter.");
        DbConnection connection = constrInfo.Invoke(new object?[] { connectionString }) as DbConnection ?? throw new Exception($"Created connection object is null or not an instance od DbConnection.");
        
        DebuggerService.Success($"Connection object of type [{MyApplication.GetRealTypeName(connection.GetType())}] successfully constructed:");
        DebuggerService.Success($"Connection string: [{connectionString}]");

        try {
            DebuggerService.Warn("Testing database connection...");
            connection.Open();
            DebuggerService.Success("Database connection successfully established!");
        } catch (Exception ex) {
            DebuggerService.Error("Database connection failure!");
            DebuggerService.Error(ex);
            throw;
        }

        return connection;

    }

    public override string ToString() {
        string jsonString = JsonSerializer.Serialize(this);
        JsonNode jsonNode= JsonNode.Parse(jsonString);
        jsonNode["DbProviderConnectionType"] = this.DbProviderConnectionType.FullName;
        jsonNode["DbProviderFactoryType"] = this.DbProviderFactoryType.FullName;
        return jsonNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }
}
