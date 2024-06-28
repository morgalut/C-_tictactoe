"# C-_tictactoe" 
# Tic Tac Toe Game

Welcome to the Tic Tac Toe Game! This is a simple console-based Tic Tac Toe game written in C#. Players can choose between 'X' and 'O' and take turns to mark the cells in a 3x3 grid. The first player to get three of their marks in a row (horizontally, vertically, or diagonally) wins the game.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
- [Game Rules](#game-rules)
- [Project Structure](#project-structure)

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download): Make sure you have the .NET SDK installed on your machine.

You can check if .NET is installed by running the following command in your CMD:

```sh
dotnet --version
Installation
Follow these steps to set up and run the Tic Tac Toe game:

Clone the repository:
sh
Copy code
git clone <repository_url>
cd <repository_directory>
Replace <repository_url> with the URL of your repository and <repository_directory> with the directory where the project should be cloned.

Navigate to the project directory:
sh
Copy code
cd P11/XCircleGame
Restore the dependencies:
sh
Copy code
dotnet restore
Usage
To run the game, execute the following command in the project directory:

sh
Copy code
dotnet run
Follow the on-screen instructions to play the game.

Game Rules
The game is played on a 3x3 grid.
Players take turns to place their marks (X or O) in an empty cell.
The first player to get three of their marks in a row (horizontally, vertically, or diagonally) wins.
If all cells are filled and no player has three marks in a row, the game is a draw.
Project Structure
The project structure is as follows:

objectivec
Copy code
P11/
│
├── XCircleGame/
│   ├── bin/
│   ├── obj/
│   ├── Game.cs
│   ├── GameInputHandler.cs
│   ├── GameState.cs
│   ├── Player.cs
│   ├── Program.cs
│   ├── XCircleGame.csproj
│
├── P11.sln
Program.cs: Contains the main logic for running the game.
Game.cs: Contains the game logic, including handling moves, checking for a win or draw, and switching players.
GameInputHandler.cs: Handles user input.
GameState.cs: Defines the different states of the game (InProgress, Win, Draw).
Player.cs: Defines the players (X, O) and cell status.
Notes
Ensure you run the commands from the root directory of the project (P11/XCircleGame).
The game requires a valid .NET SDK installation to build and run.
License
This project is licensed under the MIT License.