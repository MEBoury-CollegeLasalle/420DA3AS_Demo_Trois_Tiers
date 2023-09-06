using _420DA3AS_Demo_Trois_Tiers.DataLayer;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DAOs;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using _420DA3AS_Demo_Trois_Tiers.PresentationLayer;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
internal class UserService {
    private readonly SecurityService securityService;
    private readonly SqlDataService dataService;
    private readonly UserCreationView userCreateView;

    public UserService(SecurityService securityService, SqlDataService dataService) {
        this.securityService = securityService;
        this.dataService = dataService;
        this.userCreateView = new UserCreationView(this);
    }

    public void OpenUserManagementMenu() {

    }

    public void OpenUserCreateView() {

    }

    private void OpenUserDisplayView(UserDTO dto) {

    }

    private void OpenUserEditView(UserDTO dto) {

    }

    private void OpenUserDeleteView(UserDTO dto) {

    }

    public UserDTO CreateUser(string userName, string password) {
        // create a user DTO object from the data received (also has the password)
        UserDTO user = new UserDTO(userName, this.securityService.HashPassword(password));

        // insert the new user in the database and return it
        return this.dataService.InsertUser(user);

    }
}
