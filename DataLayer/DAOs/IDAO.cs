using System.Data;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer.DAOs;
internal interface IDAO {

    public DataTable GetDataTable();

    public void LoadData();

    public int SaveChanges();

    public void CancelChanges();

}
