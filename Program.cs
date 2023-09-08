using _420DA3AS_Demo_Trois_Tiers.BusinessLayer;
using System.Diagnostics;

namespace _420DA3AS_Demo_Trois_Tiers;

internal static class Program {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
        new MyApplication().OpenMainMenu();
    }
}