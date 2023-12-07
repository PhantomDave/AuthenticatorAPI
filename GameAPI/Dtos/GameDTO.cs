using PokerLibrary.Enums;
using PokerLibrary.Models;

namespace GameAPI.DTOs;

public class GameDTO
{
    public Deck Deck { get; set; }
    public int Pot { get; set; }
    public List<Player> Players { get; set; }
    public GameStage CurrentStage { get; set; }
    public int[] Blinds { get; set; }
    public List<Card> TableCards { get; set; }
}
