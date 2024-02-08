using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using PokerLibrary.Enums;

namespace PokerLibrary.Models
{
    public class Player
    {
        public string Name { get; private set; }
        public string? Email { get; private set; }
        public List<Card> Hand { get; private set; }
        public int Chips { get; private set; }
        public int Bet { get; private set; }
        public NextMove Move { get; private set; }
        public PlayerRole CurrentRole { get; private set; }
        public int HandValue { get; private set; }
        public int ChipsWon { get; private set; }
        public int PlayersKnockedOut { get; private set; }
        public int TablesWon { get; private set; }

        [JsonConstructor]
        public Player(string name, string email, List<Card> hand, int chips, int bet, NextMove move, PlayerRole currentRole, int handValue)
        {
            this.Name = name;
            this.Email = email;
            this.Hand = hand;
            this.Chips = chips;
            this.Bet = bet;
            this.Move = move;
            this.CurrentRole = currentRole;
            this.HandValue = handValue;
        }

        public Player() { }

        public Player(string email, int chips)
        {
            Name = "";
            Email = email;
            Hand = new List<Card>();
            Chips = chips;
        }

        public void SetChips(int chips)
        {
            Chips = chips;
        }

        public void SetUsername(string name)
        {
            Name = name;
        }

        public void ResetMoveOfPlayer(Player p)
        {
            p.Move = NextMove.None;
        }
        
        public void CalculateHandValue(List<Card> tableCards)
        {
            try
            {
                List<Card> totalCards = new List<Card>();
                totalCards.AddRange(tableCards);
                totalCards.AddRange(Hand);
                if (totalCards.Count == 0)
                {
                    HandValue = 0;
                    return;
                }
                Suits[] suits = totalCards.Select(c => c.Suit).Distinct().ToArray();
                CardValue[] seq = totalCards.Select(c => c.Value).OrderBy(s => s).ToArray();
                CardValue[] poker = seq.GroupBy(c => c)
                    .Where(n => n.Count() == 4)
                    .Select(p => p.First())
                    .ToArray();
                CardValue[] tris = seq.GroupBy(c => c)
                    .Where(n => n.Count() == 3)
                    .Select(p => p.First())
                    .ToArray();
                CardValue[] pair = seq.GroupBy(c => c)
                    .Where(n => n.Count() == 2)
                    .Select(p => p.First())
                    .ToArray();
                
                if (seq.Length > 0)
                {
                    if (seq.Zip(seq.Skip(1), (a, b) => (a + 1) == b).All(x => x))
                    {
                        if (seq[0] == CardValue.Ten && seq[4] == CardValue.Ace)
                        {
                            HandValue = 500;
                            return;
                        }
                        HandValue = 100;
                        return;
                    }
                }

                if (poker.Length > 0)
                {
                    if (
                        poker.Contains(CardValue.Ten)
                        || poker.Contains(CardValue.Jack)
                        || poker.Contains(CardValue.Queen)
                        || poker.Contains(CardValue.King)
                        || poker.Contains(CardValue.Ace)
                    )
                    {
                        HandValue = 80;
                        return;
                    }
                    HandValue = 70;
                    return;
                }
                if (tris.Length > 0 && pair.Length > 0)
                {
                    //KIASTOFUL
                    HandValue = 65;
                    return;
                }
                if (suits.Length == 1)
                {
                    HandValue = 60;
                    return;
                }
                if (tris.Length > 0)
                {
                    HandValue = 50;
                    return;
                }
                if (pair.Length > 1)
                {
                    HandValue = 30;
                    return;
                }
                if (pair.Length > 0 && pair.Length < 1)
                {
                    HandValue = 15;
                    return;
                }
                HandValue = (int)totalCards.MaxBy(c => (int)c.Value)!.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                HandValue = -100;
            }
        }

        public void PrepareMove(NextMove move, int bet = 0, bool forced = false)
        {

            if (Chips < bet && forced && move == NextMove.Bet)
                move = NextMove.AllIn;

            if (bet < 0)
                bet = -bet;

            if (move == NextMove.Bet)
            {
                Bet = bet;
                Chips -= bet;
            }

            if (move == NextMove.AllIn)
            {
                Bet = Chips;
                Chips = 0;
            }
            Move = move;
        }

        public void GivePlayerChips(int pot)
        {
            Chips += pot;
            ChipsWon += pot;
        }

        public void AddPlayerKnockedOut(int number = 1)
        {
            PlayersKnockedOut += number;
        }

        public void AddTableWon()
        {
            TablesWon++;
        }

        public void SetRole(PlayerRole role)
        {
            CurrentRole = role;
        }
    }
}
