using AvaloniaBlackJack.Data.Actions;
using AvaloniaBlackJack.Data.States;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace AvaloniaBlackjack.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public int _BetAmount = 0;
    [ObservableProperty]
    public int _Balance = 1000;

    [ObservableProperty]
    public int _PlayerScore = 0;
    [ObservableProperty]
    public int _DealerScore = 0;



    public GameState GameState { get; set; } = new GameState();
    public GameAction GameAction { get; set; } = new GameAction();

    public string PlayerScoreText
    {
        get
        {
            return $"Player : {PlayerScore}";
        }
    }
    public string DealerScoreText
    {
        get
        {
            return $"Dealer : {DealerScore}";
        }
    }
    public string BalanceText
    {
        get
        {
            return $"Balance : {Balance}";
        }
    }

    private void ShowToast(string title, string message)
    {
        // Simple console output for notifications
        System.Console.WriteLine($"🎯 [{title}] {message}");
    }

    [RelayCommand]
    public async Task StartNewGame()
    {
        try
        {
            //if (BetAmount <= 0)
            //{
            //    ShowToast("Invalid Bet", "Bet amount cannot be zero or negative.");
            //    return;
            //}
            //if (BetAmount > Balance)
            //{
            //    ShowToast("Insufficient funds", "Bet amount cannot be greater than balance.");
            //    return;
            //}

            Balance -= BetAmount;
            PlayerScore = 0;
            DealerScore = 0;

            OnPropertyChanged(nameof(PlayerScoreText));
            OnPropertyChanged(nameof(DealerScoreText));
            OnPropertyChanged(nameof(Balance));

            ShowToast("New Game", $"Game started! Bet: ${BetAmount}, Remaining Balance: ${Balance}");
        
            GameAction.Action = GameActions.Start;
            await GameAction.StartGameAsync();
        }
        catch (Exception ex)
        {
            ShowToast("Error", ex.Message);
        }
    }
}
