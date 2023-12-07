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
        public CardValue value { get; private set; }
        public Suits suit { get; private set; }

        public Card(CardValue value, Suits suit)
        {
            this.value = value;
            this.suit = suit;
        }

        public override string ToString() => $"{value} of {suit}"; 
    }
}
