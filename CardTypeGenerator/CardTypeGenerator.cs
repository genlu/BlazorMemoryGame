using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace MemoryGame.Generator
{
    [Generator]
    public class CardTypeGenerator : ISourceGenerator
    {
        private const string ICardCode = @"
using System;
namespace MemoryGame.Cards
{
    public interface ICard : IEquatable<ICard>
    {
        string Emoji { get; }
        bool IsTurned { get; set; }
        bool IsMatched { get; set; }
        string CssClass { get; }
    }
}";

        private const string CardTypeCode = @"
using System;
namespace MemoryGame.Cards
{{
    public class {0}Card : AbstractCard
    {{
        public override string Emoji => ""{1}"";
    }}
}}";

        private const string AbstractCardTypeCode = @"
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MemoryGame.Cards
{
    public abstract class AbstractCard : ICard
    {
        public abstract string Emoji { get; }

        public bool IsTurned { get; set; }
        public bool IsMatched { get; set; }

        public string CssClass
        {
            get
            {
                switch ((IsTurned, IsMatched))
                {
                    case (false, true): return ""matched"";
                    case (true, false): return ""turned"";
                    case (true, true): return ""turned matched"";
                    default: return """";
                }
            }
        }

        public bool Equals(ICard other)
            => string.CompareOrdinal(Emoji, other.Emoji) == 0;

        public override int GetHashCode()
            => HashCode.Combine(Emoji);

        public override bool Equals(object obj)
            => obj is ICard card && Equals(card);
    }
}";
        private const string CardHelpersCode1 = @"
using System;
using System.Collections.Immutable;

namespace MemoryGame.Cards
{
    public static class CardHelpers
    {
        public static ImmutableArray<string> AllEmojis { get; } = ";

        private const string CardHelpersCode2 = @"

        public static AbstractCard CreateCard(string emoji)
        {
            return emoji switch
            {";

        private const string CardHelpersCode3 = @"
                _ => throw new ArgumentException(nameof(emoji)),
            };
        }
    }
}";

        private const string SwitchExpression = @"
                {0} => new {1}Card(),";

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var emojiFile = context.AdditionalFiles.FirstOrDefault(file => string.Equals(Path.GetFileName(file.Path), "Emojis.txt", StringComparison.OrdinalIgnoreCase));
                if (emojiFile is null)
                {
                    return;
                }

                var generatorDocuments = new List<(string documentName, string source)>();
                var generatedSwitchClauses = new StringBuilder();
                var allEmojis = new StringBuilder();

                var text = emojiFile.GetText().ToString();
                using var sr = new StringReader(text);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var fields = line.Trim().Split(',');
                    var emoji = fields[0].Trim();
                    var name = fields[1].Trim();

                    var quotedEmoji = $"\"{emoji}\"";
                    generatedSwitchClauses.Append(string.Format(SwitchExpression, quotedEmoji, name));
                    allEmojis.Append($"{quotedEmoji},");
                    var generatorType = string.Format(CardTypeCode, name, emoji);
                    context.AddSource($"{name}Card", SourceText.From(generatorType, Encoding.UTF8));
                }

                // inject the created source into the users compilation
                var allEmojiList = $@"(new[]{{ {allEmojis} }}).ToImmutableArray();";
                var cardHelpers = CardHelpersCode1 + allEmojiList +
                    CardHelpersCode2 + generatedSwitchClauses.ToString() + CardHelpersCode3;

                context.AddSource("CardHelpers", SourceText.From(cardHelpers, Encoding.UTF8));
                context.AddSource("ICard", SourceText.From(ICardCode, Encoding.UTF8));
                context.AddSource("AbstractCard", SourceText.From(AbstractCardTypeCode, Encoding.UTF8));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
