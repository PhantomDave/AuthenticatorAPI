namespace AuthenticatorAPI.Models;

public class Token
{
    private string TokenId { get; set; }
    private DateTime TokenExpiration { get; set; }
    public User? User { get; private set; }

    public static List<Token> Tokens = new List<Token>();

    public Token(string tokenId, User? user)
    {
        TokenId = tokenId;
        User = user;
        TokenExpiration = DateTime.Now.AddMinutes(30);
    }

    public static void GenerateUserToken(User? user)
    {
        Token? token = new Token(Guid.NewGuid().ToString(), user);
        // TODO -> REMOVE DEBUG
        Console.WriteLine($"TOKEN CREATED: {token.TokenId} {token.User.Username}");
        Tokens.Add(token);
    }

    public static Token? GetUserTupleFromToken(string tokenId)
    {
        return Tokens.FirstOrDefault(t => t.TokenId == tokenId);
    }
    
    public static Token? GetTokenFromUser(User user)
    {
        Token? token = Tokens.FirstOrDefault(t => t?.User == user);
        return token;
    }
    
    public bool IsUserTokenValid(User user)
    {
        Token token = Tokens.FirstOrDefault(t => t?.User == user);
        if (token is null)
            return false;
        return token.TokenExpiration >= DateTime.Now;
    }
}