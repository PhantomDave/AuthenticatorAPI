using PokerLibrary.Models;

namespace PokerLibraryUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckStraightFlush()
        {
            Game _game = new Game();
            Player player = new Player("test@test.it", 1000);
            Card card = new Card(PokerLibrary.Enums.CardValue.Two, PokerLibrary.Enums.Suits.Diamond);
            Card card1 = new Card(PokerLibrary.Enums.CardValue.Three, PokerLibrary.Enums.Suits.Diamond);
            Card card2 = new Card(PokerLibrary.Enums.CardValue.Four, PokerLibrary.Enums.Suits.Diamond);
            Card card3 = new Card(PokerLibrary.Enums.CardValue.Five, PokerLibrary.Enums.Suits.Diamond);
            Card card4 = new Card(PokerLibrary.Enums.CardValue.Six, PokerLibrary.Enums.Suits.Diamond);

            player.Hand.Add(card);
            player.Hand.Add(card1);
            _game.TableCards.Add(card2);
            _game.TableCards.Add(card3);
            _game.TableCards.Add(card4);

            player.CalculateHandValue(_game.TableCards);

            Assert.AreEqual(player.HandValue, 100);
        }

        [TestMethod]
        public void CheckRoyalFlush()
        {
            Game _game = new Game();
            Player player = new Player("test@test.it", 1000);
            Card card = new Card(PokerLibrary.Enums.CardValue.Ten, PokerLibrary.Enums.Suits.Diamond);
            Card card1 = new Card(PokerLibrary.Enums.CardValue.Jack, PokerLibrary.Enums.Suits.Diamond);
            Card card2 = new Card(PokerLibrary.Enums.CardValue.Queen, PokerLibrary.Enums.Suits.Diamond);
            Card card3 = new Card(PokerLibrary.Enums.CardValue.King, PokerLibrary.Enums.Suits.Diamond);
            Card card4 = new Card(PokerLibrary.Enums.CardValue.Ace, PokerLibrary.Enums.Suits.Diamond);

            player.Hand.Add(card);
            player.Hand.Add(card1);
            _game.TableCards.Add(card2);
            _game.TableCards.Add(card3);
            _game.TableCards.Add(card4);

            player.CalculateHandValue(_game.TableCards);

            Assert.AreEqual(player.HandValue, 500);
        }


        [TestMethod]
        public void CheckPoker()
        {
            Game _game = new Game();
            Player player = new Player("test@test.it", 1000);
            Card card = new Card(PokerLibrary.Enums.CardValue.Ten, PokerLibrary.Enums.Suits.Diamond);
            Card card1 = new Card(PokerLibrary.Enums.CardValue.Ten, PokerLibrary.Enums.Suits.Hearts);
            Card card2 = new Card(PokerLibrary.Enums.CardValue.Ten, PokerLibrary.Enums.Suits.Club);
            Card card3 = new Card(PokerLibrary.Enums.CardValue.Ten, PokerLibrary.Enums.Suits.Spade);
            Card card4 = new Card(PokerLibrary.Enums.CardValue.Ace, PokerLibrary.Enums.Suits.Diamond);

            player.Hand.Add(card);
            player.Hand.Add(card1);
            _game.TableCards.Add(card2);
            _game.TableCards.Add(card3);
            _game.TableCards.Add(card4);

            player.CalculateHandValue(_game.TableCards);

            Assert.AreEqual(player.HandValue, 80);
        }

        [TestMethod]
        public void CheckColorFlush()
        {
            Game _game = new Game();
            Player player = new Player("test@test.it", 1000);
            Card card = new Card(PokerLibrary.Enums.CardValue.Five, PokerLibrary.Enums.Suits.Diamond);
            Card card1 = new Card(PokerLibrary.Enums.CardValue.Six, PokerLibrary.Enums.Suits.Diamond);
            Card card2 = new Card(PokerLibrary.Enums.CardValue.Seven, PokerLibrary.Enums.Suits.Diamond);
            Card card3 = new Card(PokerLibrary.Enums.CardValue.Eight, PokerLibrary.Enums.Suits.Diamond);
            Card card4 = new Card(PokerLibrary.Enums.CardValue.Ace, PokerLibrary.Enums.Suits.Diamond);

            player.Hand.Add(card);
            player.Hand.Add(card1);
            _game.TableCards.Add(card2);
            _game.TableCards.Add(card3);
            _game.TableCards.Add(card4);

            player.CalculateHandValue(_game.TableCards);

            Assert.AreEqual(player.HandValue, 60);
        }
    }
}