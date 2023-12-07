using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using PokerLibrary.Enums;
using PokerLibrary.Interfaces;

namespace PokerLibrary.Models
{
    public class Game : IGame
    {
        public Deck Deck { get; private set; }
        public int Pot { get; private set; }
        public List<Player> Players { get; private set; }
        public GameStage CurrentStage { get; private set; }
        public int[]? Blinds { get; private set; }
        public List<Card> TableCards { get; private set; }

        public bool Started { get; set; }

        public Game()
        {
            Deck = new Deck();
            Players = new List<Player>();
            CurrentStage = GameStage.CompulsoryBets;
            TableCards = new List<Card>();
            Started = false;
        }

        public void SetBlinds(int smallblind, int bigblind)
        {
            Blinds = new int[2] { smallblind, bigblind };
        }

        public bool CanCheck()
        {
            foreach (var player in Players)
            {
                if (player.Move == NextMove.Bet || player.Move == NextMove.AllIn)
                    return false;
            }
            return true;
        }

        public bool AddPlayerToGame(Player player)
        {
            if (Players.Contains(player))
                return false;
            Players.Add(player);
            return true;
        }

        public Player GetNextPlayerInTurn(Player currentPlayer)
        {
            int indexof = Players.IndexOf(currentPlayer) + 1;
            if (indexof >= Players.Count())
                return Players.First();
            return Players[indexof];
        }

        private Player? FindDealer()
        {
            foreach (Player p in Players)
            {
                if (p.CurrentRole == PlayerRole.Dealer)
                    return p;
            }
            return null;
        }

        public GameStage AdvanceGame()
        {
            Player? dealer = FindDealer();

            if (dealer is null)
            {
                dealer = Players[0];
                dealer.SetRole(PlayerRole.Dealer);
            }

            switch (CurrentStage)
            {
                case GameStage.CompulsoryBets:
                {
                    Player smallBlindPlayer = GetNextPlayerInTurn(dealer!);
                    Player bigBlindPlayer = GetNextPlayerInTurn(smallBlindPlayer);
                    smallBlindPlayer.PrepareMove(NextMove.Bet, Blinds![0], true);
                    bigBlindPlayer.PrepareMove(NextMove.Bet, Blinds[1], true);
                    CurrentStage = GameStage.PreFlop;
                    break;
                }
                case GameStage.PreFlop:
                {
                    Player i = GetNextPlayerInTurn(dealer!);
                    while (i != dealer)
                    {
                        Deck.DealCards(i);
                        i.PrepareMove(NextMove.Check);
                        i = GetNextPlayerInTurn(i);
                    }
                    CurrentStage = GameStage.Flop;
                    break;
                }
                case GameStage.Flop:
                {
                    if (AllPlayerMadeAMove())
                    {
                        //I Discard the first card ad rules states
                        _ = Deck.GetCard();
                        //Here i put the first 3 card on the table
                        for (int i = 0; i < 3; i++)
                        {
                            TableCards.Add(Deck.GetCard());
                        }

                        ResetPlayerMoves();
                        CurrentStage = GameStage.Turn;
                    }
                    break;
                }
                case GameStage.Turn:
                {
                    if (AllPlayerMadeAMove())
                    {
                        //I Discard the first card ad rules states
                        _ = Deck.GetCard();
                        //Here i put the first 3 card on the table
                        TableCards.Add(Deck.GetCard());
                        ResetPlayerMoves();
                        CurrentStage = GameStage.River;
                    }
                    break;
                }
                case GameStage.River:
                {
                    if (AllPlayerMadeAMove())
                    {
                        //I Discard the first card ad rules states
                        _ = Deck.GetCard();
                        //Here i put the first 3 card on the table
                        TableCards.Add(Deck.GetCard());
                        ResetPlayerMoves();
                        CurrentStage = GameStage.Showdown;
                    }
                    break;
                }
                case GameStage.Showdown:
                {
                    HandleShowdown();
                    break;
                }
            }

            return CurrentStage;
        }

        private void HandleShowdown()
        {
            try
            {
                Player p =
                    GetRoundWinner()!
                    ?? throw new NullReferenceException("Player was null in Handle Showdown");
                p.GivePlayerChips(Pot);
                Pot = 0;
                TableCards.Clear();
                Deck.GenerateNewDeck();
                ClearAllPlayerCards();
                CheckRemainingPlayers();
                CurrentStage = GameStage.CompulsoryBets;
            }
            catch (NullReferenceException)
            {
                return;
            }
        }

        private void CheckRemainingPlayers()
        {
            List<Player> newPlyList = new List<Player>(Players);
            foreach (Player p in Players)
            {
                if (p.Chips == 0)
                {
                    newPlyList.Remove(p);
                }
            }
            Players = newPlyList;
        }

        private void ClearAllPlayerCards()
        {
            foreach (Player p in Players)
            {
                p.Hand.Clear();
            }
        }

        private Player? GetRoundWinner()
        {
            Player? p = null;
            foreach (Player ply in Players)
            {
                ply.CalculateHandValue(TableCards);
                int HandValue = ply.HandValue;

                p ??= ply;

                if (HandValue > p.HandValue)
                {
                    p = ply;
                }
            }
            return p;
        }

        private void ResetPlayerMoves()
        {
            foreach (Player p in Players)
            {
                if (p.Move == NextMove.Fold)
                    p.ResetMoveOfPlayer(p);
            }
        }

        private bool AllPlayerMadeAMove()
        {
            foreach (Player p in Players)
            {
                if (p.Move == NextMove.None)
                    return false;
            }
            return true;
        }

        public void StartGame()
        {
            Console.WriteLine("Starting Game..");
            Started = true;
            AdvanceGame();
        }

        public void SetupAIPlayers(int numplayer, int chips)
        {
            for (int i = 0; i < numplayer; i++)
            {
                AddPlayerToGame(new AIPlayer(chips));
            }
        }
    }
}
