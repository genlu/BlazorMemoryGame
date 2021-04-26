using System;
using System.Collections.Immutable;

namespace MemoryGame.Cards
{
    public class CatCard
    {
        public static ImmutableArray<string> AllEmojis { get; } = (new[] { "🐱" }).ToImmutableArray();

        public static CatCard Create(string animal)
        {
            if (animal == "🐱")
            {
                return new CatCard();
            }

            // Use another constrcutor
            throw new ArgumentException();
        }

        public string Emoji => "🐱";
        public bool IsTurned { get; set; }
        public bool IsMatched { get; set; }

        public string CssClass
        {
            get
            {
                switch ((IsTurned, IsMatched))
                {
                    case (false, true): return "matched";
                    case (true, false): return "turned";
                    case (true, true): return "turned matched";
                    default: return "";
                }
            }
        }
    }
}
