using System.Security.Cryptography;

namespace _420DA3AS_Demo_Trois_Tiers.BusinessLayer.Services;
internal class SecurityService {
    private const int SALT_SIZE = 16; // 128 bits
    private const int KEY_SIZE = 32; // 256 bits
    private const int CRYPT_ITERATIONS = 100000;
    private const char SEGMENT_DELIMITER = ':';
    private static readonly HashAlgorithmName CRYPT_ALGORITHM = HashAlgorithmName.SHA256;


    public SecurityService() {

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
