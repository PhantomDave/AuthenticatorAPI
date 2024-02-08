 using Newtonsoft.Json;
using PokerLibrary.Enums;
using PokerLibrary.Exstensions;

namespace PokerLibrary.Models
{
    public class Deck
    {
        [JsonProperty]
        private List<Card> _cards;

        public Deck()
        {
            _cards = GenerateFullDeck()!;
            ShuffleDeck();
        }

        private List<Card>? GenerateFullDeck()
        {
            List<Card> cards = new List<Card>();
            foreach (CardValue card in Enum.GetValues(typeof(CardValue)))
            {
                Card card1 = new Card(card, Suits.Diamond);
                Card card2 = new Card(card, Suits.Hearts);
                Card card3 = new Card(card, Suits.Spade);
                Card card4 = new Card(card, Suits.Club);
                cards.Add(card1);
                cards.Add(card2);
                cards.Add(card3);
                cards.Add(card4);
            }
            return cards;
        }

        private void ShuffleDeck()
        {
            _cards.Shuffle();
        }

        public Card GetCard()
        {
            Card card = _cards.First();
            _cards.Remove(card);
            return card;
        }

        public Deck GenerateNewDeck()
        {
            return new Deck();
        }

        public void DealCards(Player player)
        {
            Card card = GetCard();
            player.Hand.Add(card);
            card = GetCard();
            player.Hand.Add(card);
        }
    }
}
