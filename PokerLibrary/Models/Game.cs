﻿using Newtonsoft.Json;
using PokerLibrary.Enums;
using PokerLibrary.Interfaces;

namespace PokerLibrary.Models
{
    public class Game : IGame
    {
        public Deck? Deck { get; private set; }
        public int Pot { get; private set; }
        public List<Player>? Players { get; private set; }
        public GameStage CurrentStage { get; private set; }
        public int[]? Blinds { get; private set; }
        public List<Card>? TableCards { get; private set; }
        public dynamic? LastRoundWinner { get; private set; }

        public bool Started { get; set; }

        [JsonConstructor]
        public Game(Deck Deck, int Pot, List<Player> Players, GameStage CurrentStage, int[] Blinds, List<Card> TableCards, bool Started)
        {     
            this.Deck = Deck;
            this.Pot = Pot;
            this.Players = Players;
            this.Blinds = Blinds;
            this.CurrentStage = CurrentStage;
            this.TableCards = TableCards;
            this.Started = Started;
        }

        public Game()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            this.Deck = new Deck();
            this.Players = new List<Player>();
            this.TableCards = new List<Card>();
        }

        public void SetBlinds(int smallblind, int bigblind)
        {
            this.Blinds = new int[2] { smallblind, bigblind };
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
                        Pot += Blinds[0] + Blinds[1];
                    this.CurrentStage = GameStage.PreFlop;
                    break;
                }
                case GameStage.PreFlop:
                {
                    Player? i = GetNextPlayerInTurn(dealer);
                    while (i != dealer)
                    {
                        Console.WriteLine($"Dealing cards to: {i.Name}");
                        Deck.DealCards(i);
                        i = GetNextPlayerInTurn(i);
                        if (i == dealer)
                            Deck.DealCards(i);
                        //    i.PrepareMove(NextMove.Check);
                    }
                    this.CurrentStage = GameStage.Flop;
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
                        this.CurrentStage = GameStage.Turn;
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
                        this.CurrentStage = GameStage.River;
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
                        this.CurrentStage = GameStage.Showdown;
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
                Console.WriteLine($"Round Winner is: {p.Name}, Chips Given {Pot}, HandValue: {p.HandValue}");
                p.GivePlayerChips(Pot);

                LastRoundWinner = new
                {
                    dName = p.Name,
                    dPot = Pot,
                    Comb = p.HandValue,
                    dChipsWon = p.ChipsWon
                };
                this.Pot = 0;
                TableCards.Clear();
                this.Deck = Deck.GenerateNewDeck();
                ClearAllPlayerCards();
                int knocked = CheckRemainingPlayers();
                p.AddPlayerKnockedOut(knocked);
                this.CurrentStage = GameStage.CompulsoryBets;
                AdvanceGame();
                AdvanceGame();
            }
            catch (NullReferenceException)
            {
                return;
            }
        }

        private int CheckRemainingPlayers()
        {
            List<Player> newPlyList = new List<Player>(Players);
            Console.WriteLine(Players.Count());
            foreach (Player p in Players)
            {
                if (p.Chips <= 0 && p != Players.First())
                {
                    newPlyList.Remove(p);
                }
            }
            int knockedout = Players.Count() - newPlyList.Count();
            this.Players = newPlyList;
            Console.WriteLine($"{Players.Count()}, NEW: {newPlyList.Count()}");
            return knockedout;
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
                if (p.Move != NextMove.Fold && p.Move != NextMove.AllIn)
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
            this.Started = true;
            AdvanceGame();
        }

        public void SetupAIPlayers(int numplayer, int chips)
        {
            for (int i = 0; i < numplayer; i++)
            {
                AddPlayerToGame(new AIPlayer(chips));
            }
        }

        public Game ResetGame()
        {
            Player player = Players.First();
            Game game = new Game();
            ClearAllPlayerCards();
            game.AddPlayerToGame(player);
            return game;
        }

        public void AddToPot(int chips)
        {
            if (chips < 0)
                chips = -chips;
            Pot += chips;
        }
    }
}
