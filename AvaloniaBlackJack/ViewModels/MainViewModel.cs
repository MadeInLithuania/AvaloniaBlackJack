using AvaloniaBlackJack.Data;
using AvaloniaBlackJack.Data.Cards;
using AvaloniaBlackJack.Data.States;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq; // Added for FirstOrDefault
using Avalonia.Media.Imaging;
using AvaloniaBlackJack.Data.Actions;
using EnvDTE;

namespace AvaloniaBlackJack.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _greeting = "Welcome to Avalonia!";

        [ObservableProperty]
        private int _credits = 1000;

        [ObservableProperty]
        private string _betAmount = "0";

        [ObservableProperty]
        private string _toastMessage = "";

        [ObservableProperty]
        private bool _isToastVisible = false;

        [ObservableProperty]
        private string _toastType = "info"; // info, success, warning, error

        [ObservableProperty]
        private string _toastBackgroundColor = "LightBlue";

        //Card list
        public List<Card> Cards { get; set; } = [];
        public List<Card> UsedCards { get; set; } = [];

        // Game state management
        private GameState _gameState;

        // Player card images
        [ObservableProperty]
        // C:\Users\tadul\Desktop\Dev\C#\AvaloniaBlackJack\AvaloniaBlackJack\Assets\Cards\Card backs\red_back.png
        private string _playerCard1Source = $"avares://{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName}\\AvaloniaBlackJack\\Assets\\Cards\\Card backs\\red_back.png";

        [ObservableProperty]
        private string _playerCard2Source = $"avares://Assets/Cards/Card backs/red_back.png";

        // Casino card images
        [ObservableProperty]
        private string _casinoCard1Source = $"Assets/Cards/Card backs/red_back.png";

        [ObservableProperty]
        private string _casinoCard2Source = $"Assets/Cards/Card backs/red_back.png";

        public MainViewModel()
        {
            _gameState = new GameState();
            _gameState.OnStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameStates previousState, GameStates newState)
        {
            Console.WriteLine($"Game state changed from {previousState} to {newState}");
        }

        // Simple toast notification method
        public async Task ShowToast(string message, string type = "info")
        {
            ToastMessage = message;
            ToastType = type;
            
            // Set background color based on type
            ToastBackgroundColor = type switch
            {
                "success" => "LightGreen",
                "error" => "LightCoral", 
                "warning" => "Orange",
                _ => "LightBlue" // default for info
            };
            
            IsToastVisible = true;
            
            // Auto-hide after 3 seconds
            await Task.Delay(3000);
            IsToastVisible = false;
        }

        // Commands for button actions
        [RelayCommand]
        private async Task NewGame()
        {
            try
            {
                if (int.TryParse(BetAmount, out int betAmount) && betAmount <= 0)
                {
                    await ShowToast("Insufficient credits! Please add more credits to play.", "error");
                }
                else if(betAmount > Credits)
                {
                    await ShowToast("Bet amount exceeds available credits!", "error");
                }
                else {
                    // Start new game
                    if (_gameState.CurrentState == GameStates.GAME_STARTED)
                    {
                        await ShowToast("New game started! Good luck!", "success");

                        var json = File.ReadAllText("Assets/Cards/cards.json");
                        Cards = JsonSerializer.Deserialize<List<Card>>(json);

                        _gameState = await GameActions.InitializeGame(UsedCards, Cards, [PlayerCard1Source, PlayerCard2Source]);
                    }
                    else
                    {
                        await ShowToast("Cannot start new game in current state!", "error");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in NewGame: {ex.Message}");
                await ShowToast($"Error starting new game: {ex.Message}", "error");
            }
        }

        [RelayCommand]
        private async Task Shuffle()
        {
            if (!_gameState.IsGameRunning)
            {
                await ShowToast("No game in progress to shuffle!", "warning");
                return;
            }

            Console.WriteLine("=== SHUFFLE COMMAND EXECUTED ===");
            
            // Shuffle the remaining cards
            var random = new Random();
            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
            }
            
            await ShowToast("Cards shuffled!", "info");
        }

        [RelayCommand]
        private async Task DealCard()
        {
            if (!_gameState.IsGameRunning)
            {
                await ShowToast("No game in progress to deal cards!", "warning");
                return;
            }

            Console.WriteLine("=== DEAL CARD COMMAND EXECUTED ===");
            
            // Deal cards logic here
            if (Cards.Count > 0)
            {
                var card = Cards[0];
                Cards.RemoveAt(0);
                UsedCards.Add(card);
                
                // Deal to player or casino based on game logic
                DealCardById(card.ID);
                
                await ShowToast($"Card dealt: {card.ID}!", "info");
            }
            else
            {
                await ShowToast("No cards left to deal!", "warning");
            }
        }

        [RelayCommand]
        private async Task EndGame()
        {
            Console.WriteLine("=== END GAME COMMAND EXECUTED ===");
            
            if (_gameState.IsGameRunning)
            {
                // Determine game result (simplified logic)
                var playerScore = CalculatePlayerScore();
                var dealerScore = CalculateDealerScore();
                
                if (playerScore > 21)
                {
                    _gameState.ChangeState(GameStates.DEFEAT);
                    await ShowToast("Bust! You lose!", "error");
                }
                else if (dealerScore > 21)
                {
                    _gameState.ChangeState(GameStates.VICTORY);
                    await ShowToast("Dealer bust! You win!", "success");
                }
                else if (playerScore > dealerScore)
                {
                    _gameState.ChangeState(GameStates.VICTORY);
                    await ShowToast("You win!", "success");
                }
                else if (playerScore < dealerScore)
                {
                    _gameState.ChangeState(GameStates.DEFEAT);
                    await ShowToast("Dealer wins!", "error");
                }
                else
                {
                    _gameState.ChangeState(GameStates.TIE);
                    await ShowToast("It's a tie!", "warning");
                }
            }
            else
            {
                await ShowToast("No game in progress!", "warning");
            }
        }

        [RelayCommand]
        private async Task OpenSettings()
        {
            Console.WriteLine("=== OPEN SETTINGS COMMAND EXECUTED ===");
            await ShowToast("Settings opened!", "info");
        }

        [RelayCommand]
        private async Task ToggleGame()
        {
            await ToggleGameRunning();
        }

        // Helper methods for game logic
        private int CalculatePlayerScore()
        {
            // Simplified score calculation
            return UsedCards.Take(2).Sum(c => c.Value);
        }

        private int CalculateDealerScore()
        {
            // Simplified dealer score calculation
            return UsedCards.Skip(2).Take(2).Sum(c => c.Value);
        }

        // Toggle game state methods
        public async Task ToggleGameRunning()
        {
            if (_gameState.ToggleGameRunning())
            {
                var state = _gameState.IsGameRunning ? "started" : "paused";
                await ShowToast($"Game {state}!", "info");
            }
            else
            {
                await ShowToast("Cannot toggle game state!", "error");
            }
        }

        public async Task ResetGame()
        {
            _gameState.Reset();
            UsedCards.Clear();
            
            // Reset card images
            PlayerCard1Source = "Assets/Cards/Card backs/red_back.png";
            PlayerCard2Source = "Assets/Cards/Card backs/red_back.png";
            CasinoCard1Source = "Assets/Cards/Card backs/red_back.png";
            CasinoCard2Source = "Assets/Cards/Card backs/red_back.png";
            
            await ShowToast("Game reset!", "info");
        }

        // Method to deal a card by ID
        public void DealCardById(string cardId)
        {
            var card = Cards.FirstOrDefault(c => c.ID == cardId);
            if (card != null)
            {
                // Find the next empty player card slot
                if (string.IsNullOrEmpty(PlayerCard1Source) || PlayerCard1Source.Contains("red_back"))
                {
                    PlayerCard1Source = card.ImageUrl;
                }
                else if (string.IsNullOrEmpty(PlayerCard2Source) || PlayerCard2Source.Contains("red_back"))
                {
                    PlayerCard2Source = card.ImageUrl;
                }
                // You can add more card slots as needed
            }
        }

        // Method to deal a card to casino by ID
        public void DealCardToCasinoById(string cardId)
        {
            var card = Cards.FirstOrDefault(c => c.ID == cardId);
            if (card != null)
            {
                // Find the next empty casino card slot
                if (string.IsNullOrEmpty(CasinoCard1Source) || CasinoCard1Source.Contains("red_back"))
                {
                    CasinoCard1Source = card.ImageUrl;
                }
                else if (string.IsNullOrEmpty(CasinoCard2Source) || CasinoCard2Source.Contains("red_back"))
                {
                    CasinoCard2Source = card.ImageUrl;
                }
                // You can add more card slots as needed
            }
        }
    }
}
