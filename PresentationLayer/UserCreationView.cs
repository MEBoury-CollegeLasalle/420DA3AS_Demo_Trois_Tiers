using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;

namespace _420DA3AS_Demo_Trois_Tiers.PresentationLayer;

internal partial class UserCreationView : Form {
    private readonly UserService userService;

    public UserCreationView(UserService userService) {
        this.userService = userService;
        InitializeComponent();
    }
}
