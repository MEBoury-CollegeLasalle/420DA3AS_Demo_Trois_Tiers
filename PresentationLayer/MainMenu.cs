using _420DA3AS_Demo_Trois_Tiers.BusinessLayer;

namespace _420DA3AS_Demo_Trois_Tiers;

internal partial class MainMenu : Form {
    private readonly MyApplication myApplication;

    public MainMenu(MyApplication application) {
        this.myApplication = application;
        InitializeComponent();
    }

    public void HideWindow() {
        this.Hide();
    }

    public void ShowWindow() {
        this.Show();
    }

    private void ButtonManageUsers_Click(object sender, EventArgs e) {

    }

    private void buttonExit_Click(object sender, EventArgs e) {
        this.myApplication.ExitApplication();
    }
}
