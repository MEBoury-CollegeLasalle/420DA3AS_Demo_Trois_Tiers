using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using _420DA3AS_Demo_Trois_Tiers.DataLayer;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer;
internal class MyApplication {
    public static bool DO_DEBUG = true;
    private readonly DataService dataService;
    private readonly DtoService<UserDTO> userService;
    private readonly MainMenu mainMenu;

    public MyApplication() {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        if (DO_DEBUG) {
            DebuggerService.InitDebugger();
        }
        DebuggerService.Info("Initializing application...");
        this.dataService = DataServiceFactory.GetDataService(ReadConnectionConfigurations());
        this.userService = new DtoService<UserDTO>(this.dataService);
        this.mainMenu = new MainMenu(this);
        DebuggerService.Success("Application initialization completed!");
    }

    public void OpenMainMenu() {
        try {
            Application.Run(this.mainMenu);
        } catch (Exception ex) {
            DebuggerService.Error(ex);
            throw;
        }
    }

    public void ExitApplication() {
        DebuggerService.Shutdown();
        Application.Exit();
    }

    public void OpenUserManagementWindow() {
        this.mainMenu.HideWindow();
        this.userService.OpenUserManagementView();
        this.mainMenu.ShowWindow();
    }

    private static DbConnectionOptions ReadConnectionConfigurations() {
        string dbTypeConfig = ConfigurationManager.AppSettings["dbType"]
            ?? throw new Exception("Application configurations do not contain the required [dbType] configuration key.");
        DbConnectionOptions options = new DbConnectionOptions(
            (DataServiceType) Enum.Parse(typeof(DataServiceType), dbTypeConfig, true
        ));
        DebuggerService.Warn($"Database type configuration detected: [{dbTypeConfig}].");


        foreach (string? key in ConfigurationManager.AppSettings.AllKeys) {
            string? value = ConfigurationManager.AppSettings[key];
            if (key is not null && value is not null) {
                bool parseAttempt;
                if (key.StartsWith("db") && key != "dbType") {
                    switch (key) {
                        case "dbProtocol":
                            DbConnectionProtocol protocol;
                            if (Enum.TryParse<DbConnectionProtocol>(value, true, out protocol)) {
                                options.Protocol = protocol;
                            }
                            break;
                        case "dbHost":
                            options.Host = value;
                            break;
                        case "dbPort":
                            int port;
                            parseAttempt = int.TryParse(value, out port);
                            if (parseAttempt) {
                                options.Port = port;
                            }
                            break;
                        case "dbInstanceName":
                            options.InstanceName = value;
                            break;
                        case "dbUsesIntegratedSecurity":
                            bool usesIntegratedSecurity;
                            parseAttempt = bool.TryParse(value, out usesIntegratedSecurity);
                            if (parseAttempt) {
                                options.UseIntegratedSecurity = usesIntegratedSecurity;
                            }
                            break;
                        case "dbUserName":
                            options.UserName = value;
                            break;
                        case "dbUserPassword":
                            options.UserPassword = value;
                            break;
                        case "dbDatabaseName":
                            options.DatabaseName = value;
                            break;
                        case "dbUsesAsync":
                            bool usesAsync;
                            parseAttempt = bool.TryParse(value, out usesAsync);
                            if (parseAttempt) {
                                options.UsesAsync = usesAsync;
                            }
                            break;
                        case "dbPipeName":
                            int connTimeout;
                            parseAttempt = int.TryParse(value, out connTimeout);
                            if (parseAttempt) {
                                options.ConnectionTimeoutSeconds = connTimeout;
                            }
                            break;
                        default:
                            options.AddOption(key.Substring(2), value);
                            break;
                    }
                }
            }
        }

        return options;
    }

    public static string GetRealTypeName(Type t) {
        if (!t.IsGenericType) {
            return t.Name;
        }

        StringBuilder sb = new StringBuilder();
        _ = sb.Append(t.Name.AsSpan(0, t.Name.IndexOf('`')));
        _ = sb.Append('<');
        bool appendComma = false;
        foreach (Type arg in t.GetGenericArguments()) {
            if (appendComma) {
                _ = sb.Append(',');
            }

            _ = sb.Append(GetRealTypeName(arg));
            appendComma = true;
        }
        _ = sb.Append('>');
        return sb.ToString();
    }
}
