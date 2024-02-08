using PokerLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerLibrary.Models
{
    public class Card
    {
        public CardValue Value { get; private set; }
        public Suits Suit { get; private set; }

        public Card(CardValue value, Suits suit)
        {
            this.Value = value;
            this.Suit = suit;
        }

        public override string ToString() => $"{Value} of {Suit}"; 
    }
}
