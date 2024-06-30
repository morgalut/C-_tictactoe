import numpy as np
import csv
import os
from pymongo import MongoClient

class QLearningAgent:
    def __init__(self, n_actions=9, learning_rate=0.1, discount_factor=0.9, exploration_rate=1.0, exploration_decay=0.99):
        self.n_actions = n_actions
        self.learning_rate = learning_rate
        self.discount_factor = discount_factor
        self.exploration_rate = exploration_rate
        self.exploration_decay = exploration_decay
        self.q_table = np.zeros((3**9, n_actions))
        self.records = []

        # File paths
        self.board_positions_file = 'board_positions.csv'
        self.game_records_file = 'game_records.csv'

        # Load or create board positions definitions
        self.load_or_create_board_positions()

        # MongoDB client setup
        try:
            self.client = MongoClient('localhost', 27017)
            self.db = self.client.test
            self.collection = self.db.tictactoe
        except Exception as e:
            print(f"Error connecting to MongoDB: {e}")

    def load_or_create_board_positions(self):
        if not os.path.exists(self.board_positions_file):
            self.create_board_positions_file()
        self.board_positions = {}
        with open(self.board_positions_file, 'r', newline='') as csvfile:
            reader = csv.DictReader(csvfile)
            for row in reader:
                self.board_positions[int(row['Position'])] = row['Definition']

    def create_board_positions_file(self):
        positions = [
            {'Position': 1, 'Definition': 'Top-left corner in the first row'},
            {'Position': 2, 'Definition': 'Top row in the middle'},
            {'Position': 3, 'Definition': 'Top-right corner in the first row'},
            {'Position': 4, 'Definition': 'Left corner in the middle row'},
            {'Position': 5, 'Definition': 'Center of the board in the middle row'},
            {'Position': 6, 'Definition': 'Right corner in the middle row'},
            {'Position': 7, 'Definition': 'Bottom-left square in the bottom row'},
            {'Position': 8, 'Definition': 'Middle square in the bottom row'},
            {'Position': 9, 'Definition': 'Bottom-right square in the bottom row'}
        ]
        with open(self.board_positions_file, 'w', newline='') as csvfile:
            writer = csv.DictWriter(csvfile, fieldnames=['Position', 'Definition'])
            writer.writeheader()
            writer.writerows(positions)

    def state_to_index(self, state):
        index = 0
        for i in range(9):
            index = index * 3 + state[i]
        return index

    def choose_action(self, state):
        state_index = self.state_to_index(state)
        if np.random.rand() < self.exploration_rate:
            return np.random.randint(self.n_actions)
        else:
            return np.argmax(self.q_table[state_index])

    def update_q_table(self, state, action, reward, next_state):
        state_index = self.state_to_index(state)
        next_state_index = self.state_to_index(next_state)
        best_next_action = np.argmax(self.q_table[next_state_index])
        td_target = reward + self.discount_factor * self.q_table[next_state_index, best_next_action]
        self.q_table[state_index, action] += self.learning_rate * (td_target - self.q_table[state_index, action])
        self.exploration_rate *= self.exploration_decay

    def save_record_to_db(self, state, action, reward, next_state, comment):
        record = {
            'state': state,
            'action': action,
            'reward': reward,
            'next_state': next_state,
            'comment': comment
        }
        self.collection.insert_one(record)

    def load_records_from_db(self):
        records = self.collection.find()
        self.records = [(record['state'], record['action'], record['reward'], record['next_state'], record['comment']) for record in records]

    def save_records_to_csv(self):
        with open(self.game_records_file, 'w', newline='') as csvfile:
            writer = csv.writer(csvfile)
            writer.writerow(['State', 'Action', 'Reward', 'Next State', 'Comment'])
            for record in self.records:
                state, action, reward, next_state, comment = record
                writer.writerow([state, action, reward, next_state, comment])

    def load_records_from_csv(self):
        with open(self.game_records_file, 'r', newline='') as csvfile:
            reader = csv.reader(csvfile)
            next(reader)  # Skip header row
            self.records = [(eval(state), int(action), float(reward), eval(next_state), comment) for state, action, reward, next_state, comment in reader]
            