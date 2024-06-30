using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Tic Tac Toe!");
        Console.Write("Do you want to be X or O? ");
        string? playerChoice = Console.ReadLine();

        playerChoice = playerChoice?.ToUpper() ?? "X";
        Player userPlayer = playerChoice == "O" ? Player.O : Player.X;

        Game game = new Game(userPlayer);
        QLearningAgent agent = new QLearningAgent(n_actions: 9);

        bool gameRunning = true;

        while (gameRunning)
        {
            DisplayBoard(game);

            if (game.CurrentPlayer == userPlayer)
            {
                Console.WriteLine($"Your turn, {userPlayer}");
                Console.Write("Enter a number (1-9): ");
                string? input = Console.ReadLine();

                if (input != null && int.TryParse(input, out int cellNumber) && cellNumber >= 1 && cellNumber <= 9)
                {
                    int row = (cellNumber - 1) / 3;
                    int column = (cellNumber - 1) % 3;
                    game.PlayMove(row, column);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 9.");
                }
            }
            else
            {
                // AI agent's turn
                int[] state = ConvertBoardStateToArray(game.Board);
                int action = agent.ChooseAction(state);
                int row = action / 3;
                int column = action % 3;
                game.PlayMove(row, column);
                int[] nextState = ConvertBoardStateToArray(game.Board);
                agent.UpdateQTable(state, action, CalculateReward(game.GameState), nextState);
            }

            if (game.GameState != GameState.InProgress)
            {
                gameRunning = false;
                DisplayBoard(game);
                Console.WriteLine("Game Over!");
                Console.WriteLine(game.GameState == GameState.Win ? $"{game.CurrentPlayer} wins!" : "It's a draw!");

                // Collect feedback
                Console.WriteLine("Please leave a comment about this game: ");
                string comment = Console.ReadLine();
                Console.WriteLine("Was the feedback positive, negative, or neutral? (p/n/t): ");
                string feedback = Console.ReadLine();

                double rewardAdjustment = 0.0;
                switch (feedback.ToLower())
                {
                    case "p":
                        rewardAdjustment = 0.5;
                        break;
                    case "n":
                        rewardAdjustment = -0.5;
                        break;
                    case "t":
                        rewardAdjustment = 0.1;
                        break;
                    default:
                        Console.WriteLine("Invalid feedback. No adjustment made.");
                        break;
                }

                // Adjust the reward based on feedback
                int[] finalState = ConvertBoardStateToArray(game.Board);
                agent.UpdateQTable(finalState, agent.LastAction, CalculateReward(game.GameState) + rewardAdjustment, finalState);
                
                // Save the record to the database
                agent.SaveRecordToDb(finalState, agent.LastAction, CalculateReward(game.GameState) + rewardAdjustment, finalState, comment);

                // Save records after each game
                agent.SaveRecordsToCsv();
            }
        }

        Console.ReadLine();
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

    static int[] ConvertBoardStateToArray(Player[,] board)
    {
        int[] state = new int[9];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                state[i * 3 + j] = board[i, j] == Player.X ? 1 : (board[i, j] == Player.O ? 2 : 0);
            }
        }
        return state;
    }

    static double CalculateReward(GameState gameState)
    {
        if (gameState == GameState.Win)
            return 1.0;
        else if (gameState == GameState.Draw)
            return 0.5;
        else
            return 0.0;
    }
}

internal class QLearningAgent
{
    private int n_actions;
    private double learningRate;
    private double discountFactor;
    private double explorationRate;
    private double explorationDecay;
    private Dictionary<string, double[]> qTable;
    private IMongoCollection<BsonDocument> collection;

    public int LastAction { get; private set; }

    public QLearningAgent(int n_actions, double learningRate = 0.1, double discountFactor = 0.9, double explorationRate = 1.0, double explorationDecay = 0.99)
    {
        this.n_actions = n_actions;
        this.learningRate = learningRate;
        this.discountFactor = discountFactor;
        this.explorationRate = explorationRate;
        this.explorationDecay = explorationDecay;
        this.qTable = new Dictionary<string, double[]>();

        // MongoDB setup
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("test");
        collection = database.GetCollection<BsonDocument>("tictactoe");
    }

    internal int ChooseAction(int[] state)
    {
        string stateKey = string.Join(",", state);
        LastAction = (!qTable.ContainsKey(stateKey) || new Random().NextDouble() < explorationRate) 
            ? new Random().Next(n_actions) 
            : Array.IndexOf(qTable[stateKey], qTable[stateKey].Max());

        return LastAction;
    }

    internal void SaveRecordsToCsv()
    {
        using (StreamWriter writer = new StreamWriter("game_records.csv"))
        {
            writer.WriteLine("State,Action,Reward,NextState");
            foreach (var entry in qTable)
            {
                string state = entry.Key;
                double[] qValues = entry.Value;
                for (int action = 0; action < qValues.Length; action++)
                {
                    writer.WriteLine($"{state},{action},{qValues[action]}");
                }
            }
        }
    }

    internal void UpdateQTable(int[] state, int action, double reward, int[] nextState)
    {
        string stateKey = string.Join(",", state);
        string nextStateKey = string.Join(",", nextState);

        if (!qTable.ContainsKey(stateKey))
        {
            qTable[stateKey] = new double[n_actions];
        }

        if (!qTable.ContainsKey(nextStateKey))
        {
            qTable[nextStateKey] = new double[n_actions];
        }

        double[] qValues = qTable[stateKey];
        double[] nextQValues = qTable[nextStateKey];

        double bestNextActionValue = nextQValues.Max();
        qValues[action] += learningRate * (reward + discountFactor * bestNextActionValue - qValues[action]);

        explorationRate *= explorationDecay;
    }

    public void SaveRecordToDb(int[] state, int action, double reward, int[] nextState, string comment)
    {
        var record = new BsonDocument
        {
            { "state", new BsonArray(state) },
            { "action", action },
            { "reward", reward },
            { "nextState", new BsonArray(nextState) },
            { "comment", comment }
        };

        collection.InsertOne(record);
    }
}
