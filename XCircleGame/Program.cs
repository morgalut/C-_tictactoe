using System;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Tic Tac Toe!");
        Console.Write("Do you want to be X or O? ");
        string? playerChoice = Console.ReadLine();
        Player userPlayer = playerChoice?.ToUpper() == "O" ? Player.O : Player.X;

        Game game = new Game(userPlayer);
        bool gameRunning = true;

        while (gameRunning)
        {
            DisplayBoard(game);

            Console.WriteLine($"Current player: {game.CurrentPlayer}");
            Console.Write("Enter a number (1-9): ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int cellNumber) && cellNumber >= 1 && cellNumber <= 9)
            {
                int row = (cellNumber - 1) / 3;
                int column = (cellNumber - 1) % 3;
                game.PlayMove(row, column);
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 9.");
            }

            if (game.GameState != GameState.InProgress)
            {
                gameRunning = false;
                DisplayBoard(game);
                Console.WriteLine("Game Over!");
                Console.WriteLine(game.GameState == GameState.Win ? $"{game.CurrentPlayer} wins!" : "It's a draw!");
            }
        }

        Console.ReadLine(); // Keep console window open
    }

    static void DisplayBoard(Game game)
    {
        Console.WriteLine("Current board:");
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                string displayValue = game.Board[i, j] == Player.None ? ((i * 3 + j) + 1).ToString() : game.Board[i, j].ToString();
                Console.Write($"{displayValue} ");
            }
            Console.WriteLine();
        }
    }
}
