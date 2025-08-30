using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaBlackJack.Data.States
{
    /// <summary>
    /// Game state manager with toggle functionality
    /// </summary>
    public class GameState
    {
        private GameStates _currentState = GameStates.NONE;
        private readonly Dictionary<GameStates, List<GameStates>> _validTransitions;

        public GameState() 
        {
            // Define valid state transitions
            _validTransitions = new Dictionary<GameStates, List<GameStates>>
            {
                { GameStates.NONE, new List<GameStates> { GameStates.GAME_STARTED } },
                { GameStates.GAME_STARTED, new List<GameStates> { GameStates.VICTORY, GameStates.DEFEAT, GameStates.TIE, GameStates.BLACKJACK_WIN, GameStates.BLACKJACK_DEFEAT, GameStates.GAME_STOPPED } },
                { GameStates.VICTORY, new List<GameStates> { GameStates.NONE, GameStates.GAME_STARTED } },
                { GameStates.DEFEAT, new List<GameStates> { GameStates.NONE, GameStates.GAME_STARTED } },
                { GameStates.TIE, new List<GameStates> { GameStates.NONE, GameStates.GAME_STARTED } },
                { GameStates.BLACKJACK_WIN, new List<GameStates> { GameStates.NONE, GameStates.GAME_STARTED } },
                { GameStates.BLACKJACK_DEFEAT, new List<GameStates> { GameStates.NONE, GameStates.GAME_STARTED } },
                { GameStates.GAME_STOPPED, new List<GameStates> { GameStates.NONE, GameStates.GAME_STARTED } }
            };
        }

        public GameState(GameStates initialState) : this()
        {
            CurrentState = initialState;
        }

        public GameStates CurrentState 
        { 
            get => _currentState;
            private set => _currentState = value;
        }

        public string CurrentStateName => CurrentState.ToString();

        /// <summary>
        /// Check if a state transition is valid
        /// </summary>
        public bool CanTransitionTo(GameStates newState)
        {
            return _validTransitions.ContainsKey(CurrentState) && 
                   _validTransitions[CurrentState].Contains(newState);
        }

        /// <summary>
        /// Get all valid next states from current state
        /// </summary>
        public List<GameStates> GetValidTransitions()
        {
            return _validTransitions.ContainsKey(CurrentState) 
                ? _validTransitions[CurrentState] 
                : new List<GameStates>();
        }

        /// <summary>
        /// Change to a new state if valid
        /// </summary>
        public bool ChangeState(GameStates newState)
        {
            if (CanTransitionTo(newState))
            {
                var previousState = CurrentState;
                CurrentState = newState;
                OnStateChanged?.Invoke(previousState, newState);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Toggle between two states
        /// </summary>
        public bool ToggleState(GameStates state1, GameStates state2)
        {
            if (CurrentState == state1 && CanTransitionTo(state2))
            {
                return ChangeState(state2);
            }
            else if (CurrentState == state2 && CanTransitionTo(state1))
            {
                return ChangeState(state1);
            }
            return false;
        }

        /// <summary>
        /// Toggle between game started and stopped
        /// </summary>
        public bool ToggleGameRunning()
        {
            return ToggleState(GameStates.GAME_STARTED, GameStates.GAME_STOPPED);
        }

        /// <summary>
        /// Reset to initial state
        /// </summary>
        public void Reset()
        {
            var previousState = CurrentState;
            CurrentState = GameStates.NONE;
            OnStateChanged?.Invoke(previousState, CurrentState);
        }

        /// <summary>
        /// Check if game is currently running
        /// </summary>
        public bool IsGameRunning => CurrentState == GameStates.GAME_STARTED;

        /// <summary>
        /// Check if game is in a final state (win/lose/tie)
        /// </summary>
        public bool IsGameFinished => CurrentState == GameStates.VICTORY || 
                                     CurrentState == GameStates.DEFEAT || 
                                     CurrentState == GameStates.TIE ||
                                     CurrentState == GameStates.BLACKJACK_WIN ||
                                     CurrentState == GameStates.BLACKJACK_DEFEAT;

        /// <summary>
        /// Event fired when state changes
        /// </summary>
        public event Action<GameStates, GameStates>? OnStateChanged;

        /// <summary>
        /// Display state information (legacy method for compatibility)
        /// </summary>
        public async Task DisplayState(GameStates gameStates)
        {
            ChangeState(gameStates);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get state description
        /// </summary>
        public string GetStateDescription()
        {
            return CurrentState switch
            {
                GameStates.NONE => "No game in progress",
                GameStates.GAME_STARTED => "Game is running",
                GameStates.GAME_STOPPED => "Game is paused",
                GameStates.VICTORY => "Player wins!",
                GameStates.DEFEAT => "Player loses!",
                GameStates.TIE => "It's a tie!",
                GameStates.BLACKJACK_WIN => "Blackjack! Player wins!",
                GameStates.BLACKJACK_DEFEAT => "Blackjack! Dealer wins!",
                _ => "Unknown state"
            };
        }
    }
}
