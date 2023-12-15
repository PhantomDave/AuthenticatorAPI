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
        public Player(string Name, string Email, List<Card> Hand, int Chips, int Bet, NextMove Move, PlayerRole CurrentRole, int HandValue)
        {
            this.Name = Name;
            this.Email = Email;
            this.Hand = Hand;
            this.Chips = Chips;
            this.Bet = Bet;
            this.Move = Move;
            this.CurrentRole = CurrentRole;
            this.HandValue = HandValue;
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

        //TODO: Improve calculation of Hand value by taking in consideration the card Value like i do in the "Poker Hand" section
        public void CalculateHandValue(List<Card> tableCards)
        {
            try
            {

                List<Card> totalCards = new List<Card>();
                totalCards.AddRange(tableCards);
                totalCards.AddRange(Hand);
                Suits[] suits = totalCards.Select(c => c.suit).Distinct().ToArray();
                CardValue[] seq = totalCards.Select(c => c.value).OrderBy(s => s).ToArray();
                CardValue[] Poker = seq.GroupBy(c => c)
                    .Where(n => n.Count() == 4)
                    .Select(p => p.First())
                    .ToArray();
                CardValue[] Tris = seq.GroupBy(c => c)
                    .Where(n => n.Count() == 3)
                    .Select(p => p.First())
                    .ToArray();
                CardValue[] Pair = seq.GroupBy(c => c)
                    .Where(n => n.Count() == 2)
                    .Select(p => p.First())
                    .ToArray();

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
                if (Poker.Length > 0)
                {
                    if (
                        Poker.Contains(CardValue.Ten)
                        || Poker.Contains(CardValue.Jack)
                        || Poker.Contains(CardValue.Queen)
                        || Poker.Contains(CardValue.King)
                        || Poker.Contains(CardValue.Ace)
                    )
                    {
                        HandValue = 80;
                        return;
                    }
                    HandValue = 70;
                    return;
                }
                if (Tris.Length > 0 && Pair.Length > 0)
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
                if (Tris.Length > 0)
                {
                    HandValue = 50;
                    return;
                }
                if (Pair.Length > 1)
                {
                    HandValue = 30;
                    return;
                }
                if (Pair.Length > 0 && Pair.Length < 1)
                {
                    HandValue = 15;
                    return;
                }
                HandValue = (int)totalCards.MaxBy(c => (int)c.value)!.value;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.StackTrace);
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
