using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;

namespace _420DA3AS_Demo_Trois_Tiers.PresentationLayer;
internal partial class DtoView : Form {

    public DtoView() {
        this.InitializeComponent();
    }

    private void ButtonSave_Click(object sender, EventArgs e) {
        this.DialogResult = DialogResult.OK;
    }

    private void ButtonCancel_Click(object sender, EventArgs e) {
        this.DialogResult = DialogResult.OK;
    }
}
