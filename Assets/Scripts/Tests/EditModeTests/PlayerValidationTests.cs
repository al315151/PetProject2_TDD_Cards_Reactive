using Data;
using NUnit.Framework;
using PlayerPresenters;

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
            var player = new PlayerPresenter();
            player.AddCardToHandFromDeck(deck);

            for (int i = 0; i < 10; i++) {
                player.AddCardToHandFromDeck(deck);
            }

            var playerData = player.GetPlayerData();

            Assert.IsTrue(playerData.PlayerHandSize == PlayerData.MaxHandSize);
        }

        [Test]
        public void PlayerValidationTestCreatePrototypePlayerFromBasePlayer()
        {
            var newPlayer = new PlayerPresenter();
            var copyOfNewPlayer = newPlayer.Clone(4) as PlayerPresenter;

            Assert.IsTrue(newPlayer.PlayerId != copyOfNewPlayer.PlayerId && newPlayer.GetHashCode() != copyOfNewPlayer.GetHashCode());
        }
    }
}
