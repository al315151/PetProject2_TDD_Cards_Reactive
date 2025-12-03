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

## How does the game actually works 

For further information about the base project, game rules, etc. Change to "main" branch and check the README there.
