using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerLibrary.Enums
{
    public enum CardValue
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    public enum Suits
    {
        Diamond = 1,
        Spade = 2,
        Hearts = 3,
        Club = 4
    }

    public enum GameStage
    {
        CompulsoryBets = 0,
        PreFlop = 1,
        Flop = 2,
        Turn = 3,
        River = 4,
        Showdown = 5
    }

    public enum AIPersonality
    {
        Bluffer = 0,
        Aggressive = 1,
        Cautious = 2,
        Folder = 3,
        Random = 4,
    }

    public enum NextMove
    {
        None = 0,
        Check = 1,
        Bet = 2,
        Fold = 3,
        AllIn = 4,
    }

    public enum PlayerRole
    {
        None = 0,
        Dealer = 1,
        HighBet = 2,
        LowBet = 3
    }
}
