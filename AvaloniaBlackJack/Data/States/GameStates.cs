using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaBlackJack.Data.States
{
    /// <summary>
    /// Different enum game states
    /// </summary>
    public enum GameStates
    {
        NONE,
        VICTORY,
        TIE,
        DEFEAT,
        BLACKJACK_WIN,
        BLACKJACK_DEFEAT,
        GAME_STARTED, 
        GAME_STOPPED,
    }
}
