class TicTacToeGame:
    def __init__(self):
        self.board = [0] * 9  # 0 for empty, 1 for X, 2 for O
        self.current_player = 1  # 1 for X, 2 for O
    
    def is_valid_action(self, action):
        return self.board[action] == 0
    
    def get_feedback(self, state):
        # Check rows, columns, and diagonals for a win
        winning_combinations = [
            [0, 1, 2], [3, 4, 5], [6, 7, 8],  # rows
            [0, 3, 6], [1, 4, 7], [2, 5, 8],  # columns
            [0, 4, 8], [2, 4, 6]              # diagonals
        ]
        
        for combination in winning_combinations:
            if state[combination[0]] == state[combination[1]] == state[combination[2]] != 0:
                return 'win'
        
        if all(cell != 0 for cell in state):
            return 'draw'
        
        return 'in_progress'
