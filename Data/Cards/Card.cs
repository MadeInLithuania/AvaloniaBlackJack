using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaBlackJack.Data.Cards
{
    public class Card
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public int Value { get; set; }
        public List<Card> Cards { get; set; } = [];

        public List<Card> GenerateDeck()
        {
            var jsonDir = Path.Combine(AppContext.BaseDirectory, "Data", "Cards", "cards.json");
            Cards = System.Text.Json.JsonSerializer.Deserialize<List<Card>>(File.ReadAllText(jsonDir)) ?? [];
            return Cards;
        }
    }
}
