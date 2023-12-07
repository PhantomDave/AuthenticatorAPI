namespace AuthenticatorAPI.Models;

public class Token
{
    private string TokenId { get; set; }
    public string TokenGuid
    {
        get => TokenId;
    }
    private DateTime TokenExpiration { get; set; }
    public User User { get; private set; }

    public static List<Token> Tokens = new List<Token>();

    public Token(User user)
    {
        TokenId = Guid.NewGuid().ToString();
        User = user;
        TokenExpiration = DateTime.Now.AddMinutes(30);
    }

    public static Token? GetTokenFromTokenId(string tokenId) =>
        Tokens.FirstOrDefault(t => t.TokenId == tokenId);

    public static Token GenerateUserToken(User user)
    {
        Token token = new Token(user);
        Tokens.Add(token);
        return token;
    }

    public bool IsUserTokenValid(Token token)
    {
        if (token is null)
            return false;
        return token.TokenExpiration >= DateTime.Now;
    }
}
