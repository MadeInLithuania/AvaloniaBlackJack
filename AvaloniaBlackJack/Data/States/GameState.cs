using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaBlackJack.Data.States
{
    /// <summary>
    /// Game state (ex. win, defeat)
    /// </summary>
    public class GameState
    {
        public GameState() { }
        public GameState(GameStates gameStates) { }

        public async Task DisplayState(GameStates gameStates)
        {
            switch (gameStates)
            {
                case GameStates.NONE:
                    break;
                case GameStates.VICTORY:
                    break;
                case GameStates.TIE:
                    break;
                case GameStates.DEFEAT:
                    break;
                case GameStates.BLACKJACK_WIN:
                    break;
                case GameStates.BLACKJACK_DEFEAT:
                    break;
                default:
                    break;
            }
        }
    }
}
