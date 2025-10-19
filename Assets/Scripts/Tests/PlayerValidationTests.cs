using Data;
using NUnit.Framework;

namespace Tests
{
    public class PlayerValidationTests
    {
        [Test]
        public void PlayerValidationTestGetCardsUpToCardLimit()
        {
            var deck = new DeckData();
            deck.CreateDeck();

            //Create player
            var player = new PlayerData();
            player.AddCardToHandFromDeck(deck);

            for (int i = 0; i < 10; i++) {
                player.AddCardToHandFromDeck(deck);
            }

            Assert.IsTrue(player.PlayerHandSize == PlayerData.MaxHandSize);
        }

        [Test]
        public void PlayerValidationTestCreatePrototypePlayerFromBasePlayer()
        {
            var newPlayer = new PlayerData();
            var copyOfNewPlayer = newPlayer.Clone(4) as PlayerData;

            Assert.IsTrue(newPlayer.PlayerId != copyOfNewPlayer.PlayerId && newPlayer.GetHashCode() != copyOfNewPlayer.GetHashCode());
        }
    }
}
