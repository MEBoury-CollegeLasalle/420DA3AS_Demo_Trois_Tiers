using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
internal abstract class AbstractService {

    protected AbstractService() {
        if (MyApplication.DO_DEBUG) {
            DebuggerService.Info($"INITIALIZING SERVICE OF TYPE [{MyApplication.GetRealTypeName(this.GetType())}]...");
        }
    }

    public abstract void Shutdown();

}
