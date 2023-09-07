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
internal partial class DtoView : Form {
    private UserDTO? selectedUser;

    public DtoView() {
        this.InitializeComponent();
    }

    private void buttonSave_Click(object sender, EventArgs e) {
        this.DialogResult = DialogResult.OK;
    }

    private void buttonCancel_Click(object sender, EventArgs e) {
        this.DialogResult = DialogResult.OK;
    }
}
