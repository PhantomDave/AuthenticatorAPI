namespace PokerLibrary.Models;

public class Scoreboard
{
    public string Email { get; private set; }
    public int ChipsWon { get; private set; }
    public int PlayersKnockedOut { get; private set; }
    public int TablesWon { get; private set; }

    public Scoreboard(string email, int chips, int knockedout, int tableswon)
    {
        Email = email;
        ChipsWon = chips;
        PlayersKnockedOut = knockedout;
        TablesWon = tableswon;
    }

    public Scoreboard SumScores(Scoreboard scoreboard)
    {
        ChipsWon += scoreboard.ChipsWon;
        PlayersKnockedOut += scoreboard.PlayersKnockedOut;
        TablesWon += TablesWon;
        return this;
    }
}
