using _420DA3AS_Demo_Trois_Tiers.DataLayer;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using _420DA3AS_Demo_Trois_Tiers.PresentationLayer;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
internal class DtoService<TDTO> where TDTO : class, IDTO, new() {
    private readonly DataService dataService;

    public DtoService(DataService dataService) {
        this.dataService = dataService;
    }

    public void OpenUserManagementView() {
        using (DtoView modal = new DtoView()) {
            DialogResult result = modal.ShowDialog();
            if (result == DialogResult.OK) {
                int rowChanged = this.dataService.SaveChanges<TDTO>();
                _ = MessageBox.Show($"[{rowChanged}] have been updated!");
            } else {
                this.dataService.CancelChanges<TDTO>();
            }
        }
    }

}
