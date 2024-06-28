using System;

public class Game
{
    private Player[,] board;
    private Player currentPlayer;
    private GameState gameState;
    private Player userPlayer;

    public Player CurrentPlayer => currentPlayer;
    public GameState GameState => gameState;
    public Player[,] Board => board;

    public Game(Player userPlayer)
    {
        board = new Player[3, 3]; // Initialize 3x3 board
        currentPlayer = Player.X; // Start with player X
        gameState = GameState.InProgress;
        this.userPlayer = userPlayer;
    }

    public void PlayMove(int row, int column)
    {
        if (gameState != GameState.InProgress)
        {
            Console.WriteLine("Game over. Cannot make a move.");
            return;
        }

        if (board[row, column] == Player.None)
        {
            board[row, column] = currentPlayer;

            // Check for win or draw
            if (CheckForWin(currentPlayer))
            {
                gameState = GameState.Win;
                return;
            }
            else if (IsBoardFull())
            {
                gameState = GameState.Draw;
                return;
            }

            // Switch turn to the other player
            currentPlayer = (currentPlayer == Player.X) ? Player.O : Player.X;
        }
        else
        {
            Console.WriteLine("Invalid move. Cell already taken.");
        }
    }

    private bool CheckForWin(Player player)
    {
        // Logic to check rows, columns, and diagonals for a win
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] == player && board[i, 1] == player && board[i, 2] == player)
                return true;
            if (board[0, i] == player && board[1, i] == player && board[2, i] == player)
                return true;
        }

        if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
            return true;
        if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
            return true;

        return false;
    }

    private bool IsBoardFull()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == Player.None)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
