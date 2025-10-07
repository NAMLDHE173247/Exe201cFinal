using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
// ĐÚNG
using StyleGenie.Application.Security;


namespace StyleGenie.Infrastructure.Security;

public sealed class SimpleAesGcmCrypto : ICrypto
{
    private readonly byte[] _key;

    public SimpleAesGcmCrypto(IConfiguration cfg)
    {
        var b64 = cfg["AppSecrets:MasterKeyBase64"] ?? Environment.GetEnvironmentVariable("APP_MASTER_KEY");
        if (string.IsNullOrWhiteSpace(b64)) throw new InvalidOperationException("APP_MASTER_KEY (or AppSecrets:MasterKeyBase64) missing");
        _key = Convert.FromBase64String(b64);
        if (_key.Length != 32) throw new InvalidOperationException("Master key must be 32 bytes (Base64 of 32 bytes).");
    }

    public string Encrypt(string plaintext)
    {
        var nonce = RandomNumberGenerator.GetBytes(12);
        var pt = Encoding.UTF8.GetBytes(plaintext);
        var ct = new byte[pt.Length];
        var tag = new byte[16];
        using var aes = new AesGcm(_key);
        aes.Encrypt(nonce, pt, ct, tag);

        var blob = new byte[nonce.Length + tag.Length + ct.Length];
        Buffer.BlockCopy(nonce, 0, blob, 0, 12);
        Buffer.BlockCopy(tag, 0, blob, 12, 16);
        Buffer.BlockCopy(ct, 0, blob, 28, ct.Length);
        return Convert.ToBase64String(blob);
    }

    public string Decrypt(string base64Cipher)
    {
        var blob = Convert.FromBase64String(base64Cipher);
        var nonce = new byte[12];
        var tag = new byte[16];
        var ct = new byte[blob.Length - 28];
        Buffer.BlockCopy(blob, 0, nonce, 0, 12);
        Buffer.BlockCopy(blob, 12, tag, 0, 16);
        Buffer.BlockCopy(blob, 28, ct, 0, ct.Length);

        using var aes = new AesGcm(_key);
        var pt = new byte[ct.Length];
        aes.Decrypt(nonce, ct, tag, pt);
        return Encoding.UTF8.GetString(pt);
    }
}
