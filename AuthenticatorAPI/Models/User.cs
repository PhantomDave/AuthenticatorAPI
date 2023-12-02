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
    public User(string username ,string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;
    }

    public string GenerateUserNonce()
    {
        Random rnd = new Random(DateTime.Now.Millisecond);
        long random = rnd.NextInt64(0, Int32.MaxValue);

        random = Password.Length * Email.Length + random;

        Nonce = ComputeSha256Hash(random.ToString());
        return Nonce;
    }

    public bool CheckUserCredentials(User? user, string password)
    {
        password = password.Replace(Nonce!, string.Empty);
        if (password.Equals(user.Password))
        {
            Token.GenerateUserToken(user);
            return true;
        }
        return false;
    }


    public string ComputeSha256Hash(string rawData)
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