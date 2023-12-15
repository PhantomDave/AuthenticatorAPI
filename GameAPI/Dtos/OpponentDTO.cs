using PokerLibrary.Enums;
using PokerLibrary.Models;

namespace GameAPI.Dtos
{
    public struct OpponentDTO
    {
        public string Name { get; init; }
        public int Chips { get; init; }
        public int Bet { get; init; }
        public NextMove Move { get; init; }
        public PlayerRole CurrentRole { get; init; }
        public int HandValue { get; init; }


        public static OpponentDTO GenerateStructFromClass(Player player)
        {
            OpponentDTO playerDTO = new()
            {
                Name = player.Name,
                Chips = player.Chips,
                Bet = player.Bet,
                Move = player.Move,
                CurrentRole = player.CurrentRole,
                HandValue = player.HandValue,
            };

            return playerDTO;
        }
    }
}
