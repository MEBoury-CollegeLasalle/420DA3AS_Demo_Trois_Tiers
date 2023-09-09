using System.Security.Cryptography;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;

/// <summary>
/// Service d'encryption à sens unique (hash) de mots de passes. Utilise L'algorithme
/// de base SHA-256 avec l'algorithme d'étirement de clé PBKDF2 et l'ajout d'un sel 
/// cryptographique généré automatiquement. Ceci est considéré sécuritaire et applique 
/// les pratiques recommandées en date de 2022 (vous pouvez utiliser un tel système 
/// dans un environnement professionnel).<br/>
/// 
/// Permet l'encryption des mots de passes elle même:
/// <code>((mdpEnClair) -> { mdpHashé })</code>
/// 
/// Le hashage d'une valeur en utilisant les mêmes paramètres produit 
/// toujours le même résultat, et le hashage de valeurs différentes produit toujours 
/// des résultats différents. Ceci permet de valider un mot de passe sans jamais 
/// décrypter le hash enregistré:
/// <code>(mdpEnCclair, hash) -> { HashPassword(mdpEnClair) == hash })</code>
/// </summary>
internal class SecurityService : AbstractService {
    // Paramètres de hashage.
    private const int SALT_SIZE = 16; // sel cryptographique de 128 bits
    private const int KEY_SIZE = 32; // clé cryptographique de 256 bits
    private const int CRYPT_ITERATIONS = 100000; // itérations de l'étirement de clé
    private const char SEGMENT_DELIMITER = ':'; // séparateur des sections générées - NE PAS CHANGER (sinon hashs existants invalidables)
    private static readonly HashAlgorithmName CRYPT_ALGORITHM = HashAlgorithmName.SHA256; // algorithme de base


    public SecurityService() : base() {

    }

    public override void Shutdown() {
        // rien à faire dans ce service
    }

    public static string? HashPassword(string? clearPassword) {
        if (clearPassword == null) {
            return null;
        }
        byte[] salt = RandomNumberGenerator.GetBytes(SALT_SIZE);
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(
            clearPassword,
            salt,
            CRYPT_ITERATIONS,
            CRYPT_ALGORITHM,
            KEY_SIZE
        );
        return string.Join(
            SEGMENT_DELIMITER,
            Convert.ToHexString(key),
            Convert.ToHexString(salt),
            CRYPT_ITERATIONS,
            CRYPT_ALGORITHM
        );
    }


    public static bool VerifyPassword(string clearPassword, string hash) {
        string[] segments = hash.Split(SEGMENT_DELIMITER);
        byte[] key = Convert.FromHexString(segments[0]);
        byte[] salt = Convert.FromHexString(segments[1]);
        int iterations = int.Parse(segments[2]);
        HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);
        byte[] inputSecretKey = Rfc2898DeriveBytes.Pbkdf2(
            clearPassword,
            salt,
            iterations,
            algorithm,
            key.Length
        );
        return key.SequenceEqual(inputSecretKey);
    }

}
