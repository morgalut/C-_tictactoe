from game import TicTacToeGame
from train_model import QLearningAgent

def play_game(agent):
    state = [0] * 9  # Initial state of the board
    done = False

    while not done:
        action = agent.choose_action(state)
        # Execute action in the environment
        next_state, reward, done = execute_action(state, action)

        if done:
            print(f"Game over! Reward: {reward}")
            comment = input("Please leave a comment about this game: ")
            feedback = input("Was the feedback positive or negative? (p/n): ")
            if feedback.lower() == 'p':
                reward += 0.5  # Adjust reward for positive feedback
            else:
                reward -= 0.5  # Adjust reward for negative feedback

            agent.update_q_table(state, action, reward, next_state)
            agent.save_record_to_db(state, action, reward, next_state, comment)
        else:
            agent.update_q_table(state, action, reward, next_state)
            state = next_state

def execute_action(state, action):
    # Implement the logic to execute the action and return the next state, reward, and whether the game is done
    # For now, this is a placeholder
    next_state = state[:]
    next_state[action] = 1  # Assume the agent is '1'
    reward = 0
    done = False
    return next_state, reward, done

if __name__ == "__main__":
    agent = QLearningAgent()
    play_game(agent)
