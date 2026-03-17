namespace AuthService.Utils.Encryption;

public interface IEncryptor
{
    string GenerateSalt();

    string GetHash(
        string value,
        string salt
    );
}
