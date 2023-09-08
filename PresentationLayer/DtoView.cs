using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using System.Data;

namespace _420DA3AS_Demo_Trois_Tiers.PresentationLayer;
internal partial class DtoView<TDTO> : Form where TDTO : class, IDTO, new() {

    public DtoView(DataTable dtoTable) {
        this.InitializeComponent();
        BindingSource bind = new BindingSource();
        bind.DataSource = dtoTable;
        this.dataGridView1.DataSource = bind;
    }

    private void ButtonSave_Click(object sender, EventArgs e) {
        this.DialogResult = DialogResult.OK;
    }

    private void ButtonCancel_Click(object sender, EventArgs e) {
        this.DialogResult = DialogResult.OK;
    }
}
