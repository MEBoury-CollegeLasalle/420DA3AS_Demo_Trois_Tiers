using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer;

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
    private static readonly string HOST_OPTION_KEY = "server";
    private static readonly string LOCALHOST_HOST_VALUE = "(local)";
    private static readonly int SQLSERVER_DEFAULT_PORT = 1433;
    private static readonly int MYSQL_DEFAULT_PORT = 3306;
    private static readonly int ORACLE_DEFAULT_PORT = 1521;
    private static readonly string ISEC_OPTION_KEY = "Integrated Security";
    private static readonly string USER_OPTION_KEY = "User ID";
    private static readonly string PW_OPTION_KEY = "Password";
    private static readonly string DATABASE_OPTION_KEY = "Database";
    private static readonly string ASYNC_OPTION_KEY = "Async";
    private static readonly string TIMEOUT_OPTION_KEY = "Async";

    private int? port = null;
    private string? userName = null;
    private string? userPassword = null;
    private string? databaseName = null;
    private int connectionTimeoutSeconds = 15;
    private Dictionary<string, string> OtherOptions { get; set; }

    public DataServiceType Type { get; private set; }
    public DbConnectionProtocol Protocol { get; set; } = DbConnectionProtocol.TCP;
    public string Host { get; set; } = LOCALHOST_HOST_VALUE;
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
            if (value is string && value.Length > 128) {
                throw new ArgumentException("User name must be 128 characters or less.");
            }
            this.userName = value;
        }
    }
    public string? UserPassword {
        private get { return this.userPassword; }
        set {
            if (value is string && value.Length > 128) {
                throw new ArgumentException("User password must be 128 characters or less.");
            }
            this.userPassword = value;
        }
    }
    public string? DatabaseName {
        get { return this.databaseName; }
        set {
            if (value is string && value.Length > 128) {
                throw new ArgumentException("Database name must be 128 characters or less.");
            }
            this.databaseName = value;
        }
    }
    public bool UsesAsync { get; set; } = true;
    public int ConnectionTimeoutSeconds { 
        get { return this.connectionTimeoutSeconds; } 
        set { 
            if (value < 0 || value > Int32.MaxValue) {
                throw new ArgumentException("Timeout value in seconds must be between 0 and 2147483647 inclusively.");
            }
            this.connectionTimeoutSeconds = value;
        } 
    }

    public DbConnectionOptions(DataServiceType type) {
        this.Type = type;

        // Default port
        switch (type) {
            case DataServiceType.SQL_SERVER:
                this.Port = SQLSERVER_DEFAULT_PORT;
                break;
            case DataServiceType.MYSQL:
                this.Port = MYSQL_DEFAULT_PORT;
                break;
            case DataServiceType.ORACLE:
                this.Port = ORACLE_DEFAULT_PORT;
                break;
        }

        // other options dictionary initialization
        this.OtherOptions = new Dictionary<string, string>();
    }

    public void AddOption(string key, string value) {
        this.OtherOptions.Add(key, value);
    }

    public bool RemoveOption(string key) {
        return this.OtherOptions.Remove(key);
    }

    public virtual DbConnection GetConnection() {
        StringBuilder sb = new StringBuilder();


        // Handling protocol / host / port|instance
        _ = sb.Append(HOST_OPTION_KEY).Append('=');
        switch (this.Protocol) {
            case DbConnectionProtocol.NAMED_PIPES:
                _ = this.PipeName ?? throw new Exception("Cannot create connection string for Named Pipe connection with no pipe name given.");
                _ = sb.Append(@"np:\\").Append(this.Host).Append("\\pipe\\").Append(this.PipeName);
                break;
            case DbConnectionProtocol.LPC:
                _ = sb.Append("lpc:").Append(this.Host);
                if (this.InstanceName is not null) {
                    _ = sb.Append('\\').Append(this.InstanceName);
                }
                break;
            case DbConnectionProtocol.TCP:
            default:
                _ = sb.Append("tcp:").Append(this.Host);
                if (this.InstanceName is not null) {
                    _ = sb.Append('\\').Append(this.InstanceName);
                } else if (this.Port is not null) {
                    _ = sb.Append(',').Append(this.Port.ToString());
                }
                break;
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
        _ = sb.Append(TIMEOUT_OPTION_KEY).Append('=').Append(this.ConnectionTimeoutSeconds.ToString()).Append(';');

        // handle other options
        foreach (KeyValuePair<string,string> kvp in this.OtherOptions) {
            _ = sb.Append(kvp.Key).Append('=').Append(kvp.Value).Append(';');
        }

        // Create & return the connection objects for the supported connection types
        switch (this.Type) {
            case DataServiceType.MYSQL:
                return new MySqlConnection(sb.ToString());
            case DataServiceType.ORACLE:
                return new OracleConnection(sb.ToString());
            case DataServiceType.SQL_SERVER:
            default:
                return new SqlConnection(sb.ToString());
        }
        
    }
}
