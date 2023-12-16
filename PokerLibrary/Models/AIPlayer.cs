using PokerLibrary.Enums;
using Newtonsoft.Json;

namespace PokerLibrary.Models
{
    public class AiPlayer : Player
    {
        [JsonProperty]
        public AiPersonality AiPersonality { get; private set; }
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
        public AiPlayer(string name, string email, List<Card> hand, int chips, int bet, NextMove move, PlayerRole currentRole, int handValue, AiPersonality aiPersonality)
            : base(name, email, hand, chips, bet, move, currentRole, handValue)
        {
            this.AiPersonality = aiPersonality;
        }

        public AiPlayer(int chips)
            : base("", chips)
        {
            Random rnd = new Random();
            int rndValue = rnd.Next(0, Enum.GetValues(typeof(AiPersonality)).Length);
            AiPersonality = (AiPersonality)rndValue;
            rndValue = rnd.Next(0, Usernames.Length);
            SetUsername(Usernames[rndValue]);
        }

        public void CalculateNextMove(Game currentGame)
        {
            try
            {
                if (Move == NextMove.Fold || Move == NextMove.AllIn)
                {
                    PrepareMove(Move);
                    return;
                }

                List<Card> cards = new();
                cards.AddRange(Hand);
                cards.AddRange(currentGame.TableCards);
                CalculateHandValue(cards);
                int personalityAndRandomValue = CalculatePersonalityAndRandom();
                int moveRiskValue = CalculateRiskOfMove(currentGame);
                int riskValue = personalityAndRandomValue + moveRiskValue - HandValue;
                //TODO: REMOVE DEBUG:

                Console.WriteLine(
                    $"Personality and Random: {Name} {AiPersonality} {personalityAndRandomValue} MOVE RISK; {moveRiskValue}, RISKVALUE: {riskValue}"
                );

                Random random = new Random();
                int rnd = 0;
                if (riskValue > 0)
                    rnd = random.Next(0, riskValue);

                if (HandValue > 500)
                {
                    PrepareMove(NextMove.AllIn);
                    return;
                }

                if (riskValue >= 100)
                {
                    if (currentGame.CanCheck())
                    {
                        PrepareMove(NextMove.Check);
                        return;
                    }
                    PrepareMove(NextMove.Fold);
                    return;
                }
                if (riskValue >= 50 && riskValue < 99 && currentGame.CanCheck())
                {
                    PrepareMove(NextMove.Check, riskValue);
                    return;
                }

                if (riskValue < 49 &&  riskValue > 0)
                {
                    PrepareMove(NextMove.Bet, riskValue + rnd);
                    currentGame.AddToPot(riskValue + rnd);
                    return;
                }

                if (riskValue < 0)
                {
                    PrepareMove(NextMove.AllIn, Chips);
                    currentGame.AddToPot(Chips);
                    return;
                }
                PrepareMove(NextMove.Fold);
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
                switch (player.Move)
                {
                    case NextMove.Bet:
                        case NextMove.AllIn:
                        {
                            if (player.Bet > Chips)
                                val += 70;
                            else val += 30;
                            break;
                        }

                    case NextMove.Check:
                        val += 10;
                        break;
                    case NextMove.Fold:
                    case NextMove.None:
                    default:
                        val += 0;
                        break;
                }
            }
            if (currentGame.Pot > Chips)
            {
                val -= 20;
            }
            return val;
        }

        private int CalculatePersonalityAndRandom()
        {
            int val = 0;
            switch (AiPersonality)
            {
                case AiPersonality.Bluffer:
                {
                    val -= 20;
                    break;
                }
                case AiPersonality.Aggressive:
                {
                    val -= 30;
                    break;
                }
                case AiPersonality.Cautious:
                {
                    val += 30;
                    break;
                }
                case AiPersonality.Folder:
                {
                    val += 35;
                    break;
                }
                case AiPersonality.Random:
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
