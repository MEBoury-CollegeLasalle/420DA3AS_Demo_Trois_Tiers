using _420DA3AS_Demo_Trois_Tiers.BusinessLayer;
using System.Diagnostics;

namespace _420DA3AS_Demo_Trois_Tiers;

internal static class Program {
    /// <summary>
    /// Point d'entr�e de l'application
    /// </summary>
    [STAThread]
    static void Main() {
        // Initialisation de l'application par l'appel du constructeur
        // de ma classe MyApplication ( new MyApplication() )
        // Puis appel imm�diat de la m�thode OpenMainMenu() sur le r�sultat de
        // l'appel duconstructeur (donc sur la nouvelle instance de la classe MyApplication)
        new MyApplication().OpenMainMenu();
    }
}