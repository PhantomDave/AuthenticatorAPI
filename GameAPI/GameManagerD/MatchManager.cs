using GameAPI.GameManagerD;
using PokerLibrary.Models;

namespace GameManagerD;

public class MatchManager
{
    public Dictionary<string, Game> _games = new Dictionary<string, Game>();
    private static object _istanceLock = new();
    private static MatchManager? _instance = null;

    public static MatchManager ScoreboardManagerInstance
    {
        get
        {
            if (_instance == null)
            {
                lock (_istanceLock)
                {
                    _instance ??= new MatchManager();
                }
            }
            return _instance;
        }
    }

    public bool AddGameToManager(Game game, string email)
    {
        if (_games.TryAdd(email, game))
        {
            return true;
        }
        return false;
    }

    public Game? GetGame(string email)
    { 
        _games.TryGetValue(email, out Game? game);
        return game;
    }

    public void RemoveGame(string email)
    {
        _games.Remove(email);
    }
}
