using System;
using System.Collections.Immutable;

namespace MemoryGame.Cards
{
    public static class CardHelpers
    {
        public static ImmutableArray<string> AllAnimals { get; } = (new[] { "🐱" }).ToImmutableArray();

        public static ICard CreateCard(string animal)
        {
            // use Switch Expression
            switch (animal)
            {
                case "🐱":
                    return new CatCard();
            }

            throw new ArgumentException();
        }
    }
}
