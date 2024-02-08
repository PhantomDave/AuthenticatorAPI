using System.Text.Json.Serialization;

namespace PokerLibrary.Models;

public class Scoreboard
{
    public string Email { get; private set; }
    public int ChipsWon { get; private set; }
    public int PlayersKnockedOut { get; private set; }
    public int TablesWon { get; private set; }

    [JsonConstructor]
    public Scoreboard(string email, int chipswon, int playersknockedout, int tableswon)
    {
        Email = email;
        ChipsWon = chipswon;
        PlayersKnockedOut = playersknockedout;
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
