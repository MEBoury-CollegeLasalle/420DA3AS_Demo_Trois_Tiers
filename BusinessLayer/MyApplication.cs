using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using _420DA3AS_Demo_Trois_Tiers.DataLayer;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer;
internal class MyApplication {
    private readonly SecurityService securityService;
    private readonly SqlDataService dataService;
    private readonly UserService userService;
    private readonly MainMenu mainMenu;

    public MyApplication() {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        this.securityService = new SecurityService();
        this.dataService = new SqlDataService();
        this.userService = new UserService(this.securityService, this.dataService);
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
        this.userService.OpenUserManagementMenu();
    }
}
