using AvaloniaBlackJack.Data.Cards;
using AvaloniaBlackJack.Data.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvaloniaBlackJack.Data.Actions
{
    public static class GameActions
    {
        /// <summary>
        /// Initialize game
        /// </summary>
        /// <param name="usedCards">Used cards</param>
        /// <param name="cards">Cards</param>
        /// <param name="playerCardSources">Player card sources</param>
        /// <returns>Task</returns>
        public static async Task<GameState> InitializeGame(List<Card> usedCards, List<Card> cards, List<string> playerCardSources)
        {
            try
            {                      
                // Shuffle cards (Fisher-Yates algorithm) https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
                var random = new Random();
                for (int i = cards.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    (cards[i], cards[j]) = (cards[j], cards[i]);                    
                }
                usedCards.Add(cards[0]);
                usedCards.Add(cards[1]);
                
                // Reset card images to back of cards
                playerCardSources[0] = "Assets/Cards/Card backs/red_back.png";
                playerCardSources[1] = "Assets/Cards/Card backs/red_back.png";            

                return new GameState(GameStates.GAME_STARTED);
            }
            catch(Exception ex) 
            {
                throw new Exception($"Error initializing game: {ex.Message}");
            }
        }
    }
}
