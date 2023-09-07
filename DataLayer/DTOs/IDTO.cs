using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
internal interface IDTO {

    public string GetDbTableName();

    public Type GetIdentifierType();

    public object GetIdentifierValue();

}
