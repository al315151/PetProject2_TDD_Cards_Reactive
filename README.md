# PetProject2_TDD_Cards_Reactive

## Branch: ADD-ML-AI
## Scenes to Check

In this branch, two new scenes have been created with different intents.

* TrainingScene -- Scene used for the training of the Model. 
* PlayWithTrainedPlayerScene -- Scene used for playing the game and the necessary adjustments to play with a trained model.

## Classes to focus

* CardGameAcademy -- Class used on the training of the Model. It is needed so that initialization of the game logic , and the Agent is able to connect to receive the game data when needed (on the training environment).
* PlayerTrainingAgent -- Class used to manage the Agent's actions. It is needed for sending data to the Training workflow, and act upon received Actions (both on training and on gameplay environments).
* PlayerTableReadingStrategiesSolver -- Class used to execute the requested strategies (depending on input enum value received). Necessary for ensuring deterministic behaviour on Heuristics and for traditional AI usages by players.

## Target of new logic

In this branch, the main focus was to implement the training and usage of a player within the game using the ML-Agents package.
The Agent is trained to replicate the strategy specified on the Agent (can be configured on the Inspector value), which is used as the Heuristic in which to compare the Model while training.

## How to train the Model

* Training configuration can be found on config/cardgame_config.yaml. 
* For information on training the model itself, please look into installing the Python ML-Agents package guide from Unity: (https://docs.unity3d.com/Packages/com.unity.ml-agents@4.0/manual/Installation.html)
* To trigger the training of the model, make sure you are on the TrainingScene within Unity, and trigger the command mentioned here: (https://docs.unity3d.com/Packages/com.unity.ml-agents@4.0/manual/Training-ML-Agents.html)
* When using the ML-Agents 4.0.0 version, make sure the PyTorch version is <= 2.8.0 , to avoid a problem when trying to save the Model.

## How to use the trained model

* Open the PlayWithTrainedPlayerScene, setup the model within the TrainingAgent instance within the scene on the BehaviorParameters --> Model.
* Start Play mode, as the Agent is already programmed to use the trained model to work.

## How to play

Once application starts, a menu with many empty objects will appear.

* To start the game, press the button "Start Game".
* Once that happens, cards will be dealt to your hand (bottom part of the UI).
* To start playing, press the button "Start next Round".


* Depending on player order (middle part of the UI, above the "empty cards", the text will tell you which is your order), the NPCs will play before or after you.
* Click on a card on your hand to play it. Once that happens, the rest of the NPCs will also play their cards.
* Depending on the cards played, a winner will be announced (rules for it are placed below).
* If the player won the round, its score will increase depending on the value the cards on this round have.

* To continue playing, press the button "Start Next Round" again, until no cards in any of the players (user or NPCs) are available.
* To start a new game, press the button "Start Game".

Card score works the following way:
* 1 -- 11 points
* 3 -- 10 points
* 10 -- 2 points
* 11 -- 3 points
* 12 -- 4 points

## How does the game actually works 

For further information about the base project, game rules, etc. Change to "main" branch and check the README there.
