﻿using PokerLibrary.Enums;
using PokerLibrary.Models;

namespace GameAPI.Dtos
{
    public struct GameDTO
    {
        public int Pot { get; init; }
        public Player Player { get; set; }
        public List<OpponentDTO> Opponents {get; set; }
        public GameStage CurrentStage { get; init; }
        public int[]? Blinds { get; init; }
        public List<Card>? TableCards { get; init; }
        public dynamic? LastRoundWinner { get; init; }
    }
}
