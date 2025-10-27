# PetProject2_TDD_Cards_Reactive
## What is this

This project is meant to be used as means of testing different methodologies and design patterns. The game is supposed to be is called "Brisca" in spanish. 

The "Brisca" game is a round-based, point-driven multiplayer card game, in which:
* Each game, a "card suit" is chosen and shown to the players, as the "predominant one".
* Each card Suit has 12 card numbers (from 1 to 12), in which some of them have points given once they are played.
* All players start with 3 cards in hand, and will draw one at the start of each round if possible.
* The players put one of the cards within their hands onto the table in a specified order until there is a player with no cards in hand (the deck has been emptied).

* The rounds are resolved on a specific "card hierarchy".
  * If one or more players played the "predominant" suit, they will win over other players.
  * If none of the players played the "predominant" suit, the card suit of the first player of the round will count as the "predominant" one.
      * The winner of the round is decided by the card with most points of the "predominant suit", or in case they are tied for points, the highest number wins.
      * The winner of the round then gets the sum of all the cards score played that round (from all players).
* Once a player has no more cards in hand (and no ability to get more), the game is over. 
  * The player with the highest score wins.

## Project guidelines

This project is the means of practicing specific code patterns and development methodologies.
* The code methodology used to develop this project is [TDD (Test Driven Development)](https://www.geeksforgeeks.org/software-engineering/test-driven-development-tdd/).
* The code patterns used to develop this project are the following:
  * The creational pattern to create the different objects is the [Prototype pattern](https://refactoring.guru/design-patterns/prototype).
  * The behavioral pattern chosen for this project is the [Observer pattern](https://refactoring.guru/design-patterns/observer).
* The plug-ins that were encouraged to be used and practiced upon for developing this game is the [R3 package](https://github.com/Cysharp/R3).
    

## Where to look into

There are different places to look into.
* To see where the Tests have been implemented, there is the `Assets/Scripts/Tests` , which includes both Edit Mode and Play Mode tests.
  * Edit mode tests validate every step of the game from the data perspective.
  * Play mode tests validate screen loading, an user input interaction to an extent (start the game, starting rounds, for example).
* (TO BE COMPLETED)

