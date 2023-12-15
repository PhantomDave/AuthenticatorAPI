using PokerLibrary.Enums;
using Newtonsoft.Json;

namespace PokerLibrary.Models
{
    public class AIPlayer : Player
    {
        [JsonProperty]
        public AIPersonality AIPersonality { get; private set; }
        public static string[] Usernames = new string[]
        {
            "Davide0",
            "Martina",
            "Davide1",
            "Rohan",
            "Samuele",
            "Giuseppe",
            "Gabriele",
            "Michele",
            "Marco",
            "Vincenza",
            "Vincenzo",
            "Mario",
            "Simone",
            "Buongiooornooo"
        };
        
        [JsonConstructor]
        public AIPlayer(string Name, string Email, List<Card> Hand, int Chips, int Bet, NextMove Move, PlayerRole CurrentRole, int HandValue, AIPersonality AIPersonality)
            : base(Name, Email, Hand, Chips, Bet, Move, CurrentRole, HandValue)
        {
            this.AIPersonality = AIPersonality;
        }

        public AIPlayer(int chips)
            : base("", chips)
        {
            Random rnd = new Random();
            int rndValue = rnd.Next(0, Enum.GetValues(typeof(AIPersonality)).Length);
            AIPersonality = (AIPersonality)rndValue;
            rndValue = rnd.Next(0, Usernames.Length);
            SetUsername(Usernames[rndValue]);
        }

        public void CalculateNextMove(Game currentGame)
        {
            try
            {
                if (Move == NextMove.Fold  || Move == NextMove.AllIn)
                    return;

                List<Card> cards = new();
                cards.AddRange(Hand);
                cards.AddRange(currentGame.TableCards);
                CalculateHandValue(cards);
                int personalityAndRandomValue = CalculatePersonalityAndRandom();
                int moveRiskValue = CalculateRiskOfMove(currentGame);
                int RiskValue = personalityAndRandomValue + moveRiskValue - HandValue;
                //TODO: REMOVE DEBUG:

                Console.WriteLine(
                    $"Personality and Random: {personalityAndRandomValue} MOVE RISK; {moveRiskValue}, RISKVALUE: {RiskValue}"
                );

                Random random = new Random();
                int rnd = 0;
                if (RiskValue > 0)
                    rnd = random.Next(0, RiskValue);

                if (HandValue > 500)
                {
                    PrepareMove(NextMove.AllIn);
                    return;
                }

                if (RiskValue >= 100)
                {
                    if (currentGame.CanCheck())
                    {
                        PrepareMove(NextMove.Check);
                        return;
                    }
                    PrepareMove(NextMove.Fold);
                    return;
                }
                if (RiskValue >= 35 && RiskValue <= 75 && currentGame.CanCheck())
                {
                    PrepareMove(NextMove.Check, RiskValue);
                    return;
                }

                if (RiskValue <= 35)
                {
                    PrepareMove(NextMove.Bet, RiskValue + rnd);
                    currentGame.AddToPot(RiskValue + rnd);
                    return;
                }
            }
            catch (Exception ex)
            {
                PrepareMove(NextMove.Fold);
                Console.WriteLine("ERROR IN CALCULATE MOVE " + ex.StackTrace);
            }
        }

        private int CalculateRiskOfMove(Game currentGame)
        {
            int val = 0;
            foreach (Player player in currentGame.Players)
            {
                if (
                    player.Move == NextMove.Bet && player.Bet > Chips
                    || player.Move == NextMove.AllIn && player.Bet > Chips
                )
                {
                    val += 100;
                }

                if (player.Move == NextMove.AllIn)
                {
                    val += 50;
                }

                if (player.Move == NextMove.Check)
                {
                    val += 10;
                }
            }
            if (currentGame.Pot > Chips)
            {
                val += 100;
            }
            return val;
        }

        private int CalculatePersonalityAndRandom()
        {
            int val = 0;
            switch (AIPersonality)
            {
                case AIPersonality.Bluffer:
                {
                    val += 15;
                    break;
                }
                case AIPersonality.Aggressive:
                {
                    val += 20;
                    break;
                }
                case AIPersonality.Cautious:
                {
                    val += -10;
                    break;
                }
                case AIPersonality.Folder:
                {
                    val -= 50;
                    break;
                }
                case AIPersonality.Random:
                {
                    Random random = new Random();
                    val += random.Next(100);
                    break;
                }
            }
            return val;
        }
    }
}
