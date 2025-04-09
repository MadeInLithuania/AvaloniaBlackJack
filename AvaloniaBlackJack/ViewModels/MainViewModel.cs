using AvaloniaBlackJack.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic;

namespace AvaloniaBlackJack.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _greeting = "Welcome to Avalonia!";
    }
}
