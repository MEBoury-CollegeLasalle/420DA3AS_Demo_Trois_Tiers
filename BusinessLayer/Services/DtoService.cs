using _420DA3AS_Demo_Trois_Tiers.DataLayer;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using _420DA3AS_Demo_Trois_Tiers.PresentationLayer;
using System.Data;
using System.Diagnostics;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
internal class DtoService<TDTO> : AbstractService where TDTO : class, IDTO, new() {
    private readonly DataService dataService;

    public DtoService(DataService dataService) : base() {
        this.dataService = dataService;
    }

    public void OpenUserManagementView() {
        using (DtoView<TDTO> modal = new DtoView<TDTO>(this.dataService.GetDataTable<TDTO>())) {
            DialogResult result = modal.ShowDialog();
            if (result == DialogResult.OK) {
                int rowChanged = this.dataService.SaveChanges<TDTO>();
                _ = MessageBox.Show($"[{rowChanged}] rows have been updated!");
            } else {
                this.dataService.CancelChanges<TDTO>();
            }
        }
    }

}
