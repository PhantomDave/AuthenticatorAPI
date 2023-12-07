using Newtonsoft.Json;
using PokerLibrary.Enums;
using PokerLibrary.Exstensions;

namespace PokerLibrary.Models
{
    public class Deck
    {
        [JsonProperty]
        private Queue<Card> _cards;

        public Deck()
        {
            _cards = GenerateFullDeck()!;
            ShuffleDeck();
        }

        private Queue<Card>? GenerateFullDeck()
        {
            Queue<Card> cards = new Queue<Card>();
            foreach (CardValue card in Enum.GetValues(typeof(CardValue)))
            {
                Card card1 = new Card(card, Suits.Diamond);
                Card card2 = new Card(card, Suits.Hearts);
                Card card3 = new Card(card, Suits.Spade);
                Card card4 = new Card(card, Suits.Club);
                cards.Enqueue(card1);
                cards.Enqueue(card2);
                cards.Enqueue(card3);
                cards.Enqueue(card4);
            }
            return cards;
        }

        private void ShuffleDeck()
        {
            List<Card> _cardList = _cards.ToList();
            _cardList.Shuffle();
            _cards = new Queue<Card>(_cardList);
        }

        public Card GetCard() => _cards.Dequeue();

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
