namespace MemoryGame.Cards
{
    public interface ICard
    {
        string Animal { get; }
        bool IsTurned { get; set; }
        bool IsMatched { get; set; }
        string CssClass { get; }
    }

    // #6. Implement IEquatable and fix diagnostics
    // #9. Extract a common base calss for more card types
    public class CatCard : ICard
    {
        public string Animal { get; } = "🐱";
        public bool IsTurned { get; set; }
        public bool IsMatched { get; set; }

        public string CssClass
        {
            get
            {
                // Convert to switch expression
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
