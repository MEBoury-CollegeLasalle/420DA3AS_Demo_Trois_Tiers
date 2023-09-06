using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _420DA3AS_Demo_Trois_Tiers.PresentationLayer;
internal partial class UserManagementView : Form {
    private readonly UserService userService;
    private UserDTO? selectedUser;

    public UserManagementView(UserService userService) {
        this.userService = userService;
        InitializeComponent();
    }

    public void LoadCombobox(List<UserDTO> userList) {
        this.userCombobox.Items.Clear();
        foreach (UserDTO user in userList) {
            // I can directly add users in the combobox because I have overridden
            // the ToString() method in UserDTO. That method is used to display
            // combobox items.
            _ = this.userCombobox.Items.Add(user);
        }
    }

    public void AddToCombobox(UserDTO user) {
        // I can directly add users in the combobox because I have overridden
        // the ToString() method in UserDTO. That method is used to display
        // combobox items.
        _ = this.userCombobox.Items.Add(user);
    }

    private void buttonCreateUser_Click(object sender, EventArgs e) {

    }

    private void buttonUserDetails_Click(object sender, EventArgs e) {

    }

    private void buttonEditUser_Click(object sender, EventArgs e) {

    }

    private void buttonUserDelete_Click(object sender, EventArgs e) {

    }

    private void UserComboBox_SelectedIndexChanged(object sender, EventArgs e) {
        this.selectedUser = (UserDTO)this.userCombobox.SelectedItem;
    }
}
