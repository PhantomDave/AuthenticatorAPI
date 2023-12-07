using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace AuthenticatorAPI.Models;

public class User
{
    public string Username { get; set; }
    public string Email { get; private set; }

    [JsonProperty]
    private string Password { get; set; }

    [JsonIgnore]
    private string? Nonce { get; set; }

    [JsonConstructor]
    public User(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }

    public string GenerateUserNonce()
    {
        Random rnd = new(DateTime.Now.Millisecond);
        long random = rnd.NextInt64(0, int.MaxValue);

        random = Password.Length * Email.Length + random;

        Nonce = ComputeSha256Hash(random.ToString());
        Console.WriteLine("DEBUG: USE THIS TO LOGIN -> " + Nonce + Password);
        return Nonce;
    }

    public Token? CheckUserCredentials(User user, string password)
    {
        password = password.Replace(Nonce!, string.Empty);
        if (password.Equals(user.Password))
        {
            Nonce = "";
            return Token.GenerateUserToken(user);
        }
        Nonce = "";
        return null;
    }

    public static string ComputeSha256Hash(string rawData)
    {
        using SHA256 sha256Hash = SHA256.Create();
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        StringBuilder builder = new StringBuilder();
        foreach (byte t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }

    public void HashOwnPassword()
    {
        Password = ComputeSha256Hash(Password);
    }
}
