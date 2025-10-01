using AvaloniaBlackJack.Data.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaBlackJack.Data.Actions
{
    public class GameAction
    {
        private GameActions _action;
        private Card Card = new();
        public GameActions Action
        {
            get { return _action; } set { _action = value; }
        }

        public GameAction()
        {
            _action = GameActions.Default;  
        }

        public async Task StartGameAsync()
        {
            Card.GenerateDeck();
        }
    }
}
