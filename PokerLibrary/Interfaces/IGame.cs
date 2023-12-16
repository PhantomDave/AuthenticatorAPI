using PokerLibrary.Enums;
using PokerLibrary.Models;

namespace PokerLibrary.Interfaces;

public interface IGame
{
    Deck Deck { get; }
    int Pot { get; }
    List<Player> Players { get; }
    GameStage CurrentStage { get; }
    int[]? Blinds { get; }
    List<Card> TableCards { get; }
    bool Started { get; }

    void SetBlinds(int smallblind, int bigblind);
    bool CanCheck();
    bool AddPlayerToGame(Player player);
    Player GetNextPlayerInTurn(Player currentPlayer);
    GameStage AdvanceGame();
    void StartGame();
    void SetupAiPlayers(int numplayer, int chips);
}
