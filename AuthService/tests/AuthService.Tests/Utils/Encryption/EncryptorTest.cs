using AuthService.Utils.Encryption;

using JetBrains.Annotations;

namespace AuthService.Tests.Utils.Encryption;

[TestSubject(typeof(Encryptor))]
public class EncryptorTest
{
    private readonly Encryptor _encryptor = new();
    
    [Fact]
    public void GetSalt_ReturnNonNullValue()
    {
        // Act
        var salt = _encryptor.GetSalt();

        // Assert
        Assert.NotNull(salt);
        Assert.NotEmpty(salt);
    }

    [Fact]
    public void GetSalt_ReturnDifferentValuesEachTime()
    {
        // Act
        var salt1 = _encryptor.GetSalt();
        var salt2 = _encryptor.GetSalt();

        // Assert
        Assert.NotEqual(salt1, salt2);
    }

    [Fact]
    public void GetHash_WithSameInputAndSalt_ReturnSameHash()
    {
        // Arrange
        const string value = "password";
        var salt = _encryptor.GetSalt();

        // Act
        var hash1 = _encryptor.GetHash(value, salt);
        var hash2 = _encryptor.GetHash(value, salt);

        // Assert
        Assert.NotNull(hash1);
        Assert.NotNull(hash2);
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHash_WithDifferentSalts_ReturnDifferentHashes()
    {
        // Arrange
        const string value = "password";
        var salt1 = _encryptor.GetSalt();
        var salt2 = _encryptor.GetSalt();

        // Act
        var hash1 = _encryptor.GetHash(value, salt1);
        var hash2 = _encryptor.GetHash(value, salt2);

        // Assert
        Assert.NotNull(hash1);
        Assert.NotNull(hash2);
        Assert.NotEqual(hash1, hash2);
    }
}
