using _420DA3AS_Demo_Trois_Tiers.DataLayer;
using _420DA3AS_Demo_Trois_Tiers.DataLayer.DTOs;
using _420DA3AS_Demo_Trois_Tiers.PresentationLayer;
using System.Data;
using System.Diagnostics;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;

/// <summary>
/// Classe générique représentant un service de gestion pour un type de modèle (DTO) spécifié.
/// Le type du modèle géré est spécifié par le paramètre de type .
/// <br/><typeparamref name="TDTO"/> doit:
/// <list type="bullet">
/// <item>être une structure de type <see langword="class"/></item>
/// <item>hériter de <see cref="IDTO"/></item>
/// <item>implémenter un constructeur sans paramètres ou ne posséder que le constructeur par défaut (<see langword="new()"/>)</item>
/// </list>
/// L'utilisation de génériques permet ici d'avoir une classe de service qui s'adapte à n'importe quel classe
/// de modèle/DTO au lieu d'avoir une classe de service différente pour chaque modèle/DTO.
/// </summary>
/// <remarks>
/// Hérite de <see cref="AbstractService"/>.
/// </remarks>
/// <typeparam name="TDTO">Le type de modèle géré par la classe</typeparam>
internal class DtoService<TDTO> : AbstractService where TDTO : class, IDTO, new() {
    // Champs privés de la classe

    /// <summary>
    /// Champ privé pour la fenêtre de gestion
    /// </summary>
    private readonly DtoView<TDTO> dtoManagementWindow;
    private readonly DataService dataservice;

    /// <summary>
    /// Constructeur de la classe <see cref="DtoService{TDTO}"/>.
    /// </summary>
    /// <param name="dataService">Le service d'Accès aux données utiliser pour gérer les instances de <typeparamref name="TDTO"/>.</param>
    public DtoService(DataService dataService) : base() {
        // Sauvegarde du service de données reçu en paramètre dans un champ d'instance
        // pout utilisation ailleurs dans la classe.
        this.dataservice = dataService; // ASSOCIATION

        // Création d'une instance de DtoView, une classe fe fenêtre de gestion pour un type de modèle/DTO.
        // NOTEZ que DtoView est, tout comme DtoService, GÉNÉRIQUE; c'est à dire qu'elle s'adapte en fonction
        // d'un type recu en paramètre de type. Ainsi, DtoView<type> gère l'Affichage pour les modèles dont
        // la classe est 'type'.
        // Ici, je passe le type reçu lors de la création de l'instance de DtoService (TDTO).
        // Donc, un le DtoService pour un type de modèle/DTO spécifié va créer une fenêtre de gestion pour
        // le même type de modèle/DTO:
        // DtoService<UserDTO> va créer une fenêtre DtoView<UserDTO> ,
        // DtoService<ProductDTO> va créer une fenêtre DtoView<ProductDTO> etc...
        this.dtoManagementWindow = new DtoView<TDTO>(this.dataservice.GetDataTable<TDTO>()); // COMPOSITION
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Shutdown() {
        // Fermeture des et libération des ressources de la fenêtre de gestion.
        // Je ferme cette fenêtre ici parce que ladite fenêtre est créée par le constructeur;
        // Il s'agit d'une relation de COMPOSITION: DtoService est COMPOSÉ d'une fenêtre
        // de gestion: il est est le propriétaire et la fenêtre n'a pas de raison d'être
        // si le service est fermé/détruit.
        this.dtoManagementWindow.Dispose();

        // Inversement, je ne ferme pas le service de données (dataService) parce que celui-ci
        // est REÇU par le constructeur; ça, c'est une relation d'ASSOCIATION: DtoService est
        // ASSOCIÉ au DataService, mais il n'en est pas le propriétaire: d'autres services peuvent
        // l'utiliser et il ne perd donc pas nécessairement sa raison d'être lorsque le DtoService
        // est fermé/détruit.
    }

    /// <summary>
    /// Ouvre la fenêtre de gestion des objets du type de DTO du service (<see cref="{TDTO}"/>).
    /// Cette fenêtre est MODALE (suspend le reste de l'application tant qu'elle n'Est pas fermée).
    /// Si le résultat de l'interaction modale est OK, sauvegarde des changements dans la base de
    /// données; autrement, annulation de tout changement apporté dans la fenêtre de gestion.
    /// </summary>
    public void OpenDtoManagementWindow() {
        // ouvrir la fenêtre en mode modal et obtenir le résultat lors de sa fermeture
        DialogResult result = this.dtoManagementWindow.ShowDialog();

        if (result == DialogResult.OK) {
            // Si le résultat est OK, utilisation du service de données (dataService)
            // pour sauvegarder les changements aportés aux modèles du type géré.
            // NOTE: la méthode DataService.SaveChanges() est GÉNÉRIQUE; c'Est à dire
            // que c'ette méthode s'adapte en fonction du paramètre de type qui lui est passé.
            // Ici, il s'agit du même type que celui passé pour créer le DtoService (TDTO)
            // Donc, DtoService<UserDTO> va appeler  DataService.SaveChanges<UserDTO>() etc...
            int rowChanged = this.dataservice.SaveChanges<TDTO>();

            // affichage de confirmation du nombre de lignes ajoutées/modifiées/supprimées
            _ = MessageBox.Show($"[{rowChanged}] rows have been updated!");

        } else {
            // Sinon, utilisation du service de données (dataService)
            // pour annuler (revert) les changements aportés aux modèles du type géré.
            // NOTE: la méthode DataService.CancelChanges() est GÉNÉRIQUE; c'Est à dire
            // que c'ette méthode s'adapte en fonction du paramètre de type qui lui est passé.
            // Ici, il s'agit du même type que celui passé pour créer le DtoService (TDTO)
            // Donc, DtoService<UserDTO> va appeler  DataService.CancelChanges<UserDTO>() etc...
            this.dataservice.CancelChanges<TDTO>();
        }
    }
}
