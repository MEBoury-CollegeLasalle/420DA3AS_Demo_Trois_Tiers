using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer.DAOs;
internal interface IDAO {

    public DataTable GetDataTable();

    public void LoadData();

    public int SaveChanges();

    public void CancelChanges();

}
