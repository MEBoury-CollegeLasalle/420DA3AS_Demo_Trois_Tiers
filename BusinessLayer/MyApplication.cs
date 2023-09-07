using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using _420DA3AS_Demo_Trois_Tiers.DataLayer;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using System.Configuration;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer;
internal class MyApplication {
    private readonly DataService dataService;
    private readonly DtoService<UserDTO> userService;
    private readonly MainMenu mainMenu;

    public MyApplication() {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        this.dataService = DataServiceFactory.GetDataService(ReadConnectionConfigurations());
        this.userService = new DtoService<UserDTO>(this.dataService);
        this.mainMenu = new MainMenu(this);
    }

    public void OpenMainMenu() {
        Application.Run(this.mainMenu);
    }

    public void ExitApplication() {
        Application.Exit();
    }

    public void OpenUserManagementWindow() {
        this.mainMenu.HideWindow();
        this.userService.OpenUserManagementView();
    }

    private static DbConnectionOptions ReadConnectionConfigurations() {
        DbConnectionOptions options = new DbConnectionOptions(
            (DataServiceType) Enum.Parse(typeof(DataServiceType),
            ConfigurationManager.AppSettings["dbType"]
            ?? throw new Exception("Application configurations do not contain the required [dbType] configuration key."),
            true
        ));
        DbConnectionProtocol protocol;
        if (Enum.TryParse<DbConnectionProtocol>(ConfigurationManager.AppSettings["dbProtocol"], true, out protocol)) {
            options.Protocol = (DbConnectionProtocol) protocol;
        }
        string? host = ConfigurationManager.AppSettings["dbHost"];
        if (host is not null) {
            options.Host = host;
        }
        int port;
        bool parseAttempt = int.TryParse(ConfigurationManager.AppSettings["dbPort"], out port);
        if (parseAttempt) {
            options.Port = port;
        }
        string? instanceName = ConfigurationManager.AppSettings["dbInstanceName"];
        if (instanceName is not null) {
            options.InstanceName = instanceName;
        }
        string? pipeName = ConfigurationManager.AppSettings["dbPipeName"];
        if (pipeName is not null) {
            options.PipeName = pipeName;
        }
        bool usesIntegratedSecurity;
        parseAttempt = bool.TryParse(ConfigurationManager.AppSettings["dbUsesIntegratedSecurity"], out usesIntegratedSecurity);
        if (parseAttempt) {
            options.UseIntegratedSecurity = usesIntegratedSecurity;
        }
        string? userName = ConfigurationManager.AppSettings["dbUserName"];
        if (userName is not null) {
            options.UserName = userName;
        }
        string? userPassword = ConfigurationManager.AppSettings["dbUserPassword"];
        if (userPassword is not null) {
            options.UserPassword = userPassword;
        }
        string? databaseName = ConfigurationManager.AppSettings["dbDatabaseName"];
        if (databaseName is not null) {
            options.DatabaseName = databaseName;
        }
        bool usesAsync;
        parseAttempt = bool.TryParse(ConfigurationManager.AppSettings["dbUsesAsync"], out usesAsync);
        if (parseAttempt) {
            options.UsesAsync = usesAsync;
        }
        int connTimeout;
        parseAttempt = int.TryParse(ConfigurationManager.AppSettings["dbConnectionTimeout"], out connTimeout);
        if (parseAttempt) {
            options.ConnectionTimeoutSeconds = connTimeout;
        }

        return options;
    }
}
