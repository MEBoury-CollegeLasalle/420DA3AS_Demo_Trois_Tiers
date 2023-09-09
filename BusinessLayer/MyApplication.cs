using _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
using _420DA3AS_Demo_Trois_Tiers.DataLayer;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer;

/// <summary>
/// Classe centrale représentant l'application elle-même
/// </summary>
internal class MyApplication {
    /// <summary>
    /// Champ statique activant ou non le debugger graphique minimal que j'ai créé.
    /// TODO: envoyer cette configuration dans App.config et lire celle-ci depuis le fichier.
    /// </summary>
    public static bool DO_DEBUG = true;
    /// <summary>
    /// Champ privé pour un objet de type <see cref="DataService"/>.
    /// Voir documentation de la classe <see cref="DataService"/> pour description dudit service.
    /// </summary>
    private readonly DataService dataService;
    /// <summary>
    /// Champ privé pour un objet de type <see cref="DtoService{T}"/>.
    /// Voir documentation de la classe <see cref="DtoService{T}"/> pour description dudit service.
    /// </summary>
    private readonly DtoService<UserDTO> userService;
    /// <summary>
    /// Champ privé pour un objet de type <see cref="MainMenu"/>; la fenêtre de menu principal de l'application.
    /// </summary>
    private readonly MainMenu mainMenu;
    /// <summary>
    /// Champ privé pour un objet de type <see cref="DbConnectionOptions"/>; une classe créée
    /// pour représenter et contenir les configurations de l'application au sujet de la connexion
    /// à une base de données.
    /// </summary>
    DbConnectionOptions databaseConnectionOptions;

    public MyApplication() {

        // NOTE:
        // Cette ligne de code est nécessaire d'être exécuté avant la création de quelque objet
        // de classe Form (windows forms) que ce soit.
        ApplicationConfiguration.Initialize();

        // Démarrer le debugger graphique si le champ bouléen est setté à VRAI
        if (DO_DEBUG) {
            DebuggerService.InitDebugger();
        }

        // Affichage d'une ligne informative. Je ne commenterai plus
        // les prochains appels de méthode de DebuggerService; ils ont
        // tous pour but d'afficher des infos de débogage.
        DebuggerService.Info("Initializing application...");

        // Initialisation des instances d'objets utilisées par l'application:

        // initialisation d'un objet de configuration de base de données
        // à partir des configurations contenue dans le fichier App.config
        this.databaseConnectionOptions = DbConnectionOptions.LoadFromAppConfigs();

        // dataService représente un service/façade d'accès aux données.
        // Le service est créé par une class de type Factory qui crée l'instance
        // du service à partir des configurations de l'application
        this.dataService = DataServiceFactory.GetDataService(this.databaseConnectionOptions);

        // userService est une classe service qui offre des fonctionalités de gestion
        // des utilisateurs (UserDTO).
        // Le service lui-même (DtoService) est une classe générique, ce qui veut dire une classe qui
        // s'adapte en fonction d'un type d'objet passé en paramètre de type (le <Type> )
        // Donc,
        // DtoService<UserDTO> est pour la gestion des UserDTO;
        // DtoService<ProductDTO> serait pour la gestion des produits s'il y en avait...
        this.userService = new DtoService<UserDTO>(this.dataService);

        // Initialisation de l'objet de type Form (MainMenu hérite de Form), la fenêtre
        // du menu principal.
        this.mainMenu = new MainMenu(this);

        DebuggerService.Success("Application initialization completed!");

        // Rendu à ce point, l'application s'est initialisé et a créé les objets / services
        // dont elle a besoin. Elle est prète à être démarrée réellement.
    }

    /// <summary>
    /// Ouverture de la fenêtre du menu principal.
    /// </summary>
    /// <remarks>
    /// Cette méthode n'est PAS STATIQUE afin de forcer la création 
    /// - et donc l'initialisation - de l'application avant de la
    /// démarrer (elle ne peut être appelée que sur une instance de <see cref="MyApplication"/>
    /// existante / construite.
    /// </remarks>
    public void OpenMainMenu() {

        int exitCode = 0;

        // Exécution de l'application englobé dans un bloc try-catch.
        // En cas d'exception (erreur qui peut être gérée) lancée (throw X;)
        // le code du bloc "catch" sera exécuté.
        try {
            // démarre le loop d'affichage avec la fenêtre de menu principal comme Form
            // principal.
            Application.Run(this.mainMenu);

        } catch (Exception ex) {
            // Passe l'exception attrapée au débugger pour affichage du message d'erreur
            // et de la stack trace.
            DebuggerService.Error(ex);
            exitCode = 1;

            // Re-lance l'exception sans la modifier pour maintenir les fonctionnalités
            // du débugger de Visual Studio / votre IDE
            throw;


        } finally { 
            // Ferme l'application avec le code de sortie 1 (un problème est survenu).
            // Les blocs finally sont exécutés peu importe de si une exception a été
            this.ExitApplication(exitCode);
        }
    }

    /// <summary>
    /// Ferme l'application avec le code de sortie par défaut 0 (pas de problème indiqué)
    /// </summary>
    public void ExitApplication() {
        this.ExitApplication(0);
    }

    /// <summary>
    /// Ferme l'application avec le code de sortie reçu en paramètre.
    /// </summary>
    /// <param name="exitCode">Le code de sortie de l'application</param>
    public void ExitApplication(int exitCode) {
        // Appèle la méthode Dispose() (de C#) des objets de type Form pour les
        // fermer et libérer les ressources mémoire.
        this.mainMenu.Dispose();

        // Appèle la méthode Shutdown() de tous les services de l'application
        // ( Shutdown() est requise dans les services par leur extension de la
        // classe abstraite AbstractService.
        this.dataService.Shutdown();
        this.userService.Shutdown();
        // DebuggerService est un cas un peu différent parce qu'il agit comme un singleton:
        // une classe qui ne peut avoir qu'une seule instance existante (constructeur
        // privé, accès par méthode statique GetInstance() ).
        DebuggerService.GetInstance().Shutdown();

        // ferme l'application
        Environment.Exit(exitCode);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OpenUserManagementWindow() {
        this.mainMenu.HideWindow();
        this.userService.OpenDtoManagementWindow();
        this.mainMenu.ShowWindow();
    }

    public static string GetRealTypeName(Type t) {
        if (!t.IsGenericType) {
            return t.Name;
        }

        StringBuilder sb = new StringBuilder();
        _ = sb.Append(t.Name.AsSpan(0, t.Name.IndexOf('`')));
        _ = sb.Append('<');
        bool appendComma = false;
        foreach (Type arg in t.GetGenericArguments()) {
            if (appendComma) {
                _ = sb.Append(',');
            }

            _ = sb.Append(GetRealTypeName(arg));
            appendComma = true;
        }
        _ = sb.Append('>');
        return sb.ToString();
    }
}
