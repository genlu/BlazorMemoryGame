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
        private const string CardTypeCode = @"
using System;
namespace MemoryGame.Cards
{{
    public class {0}Card : BaseCard
    {{
        public override string Emoji => ""{1}"";
    }}
}}";

        private const string AbstractCardTypeCode1 = @"
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MemoryGame.Cards
{
    public abstract class BaseCard
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

        public bool Equals(BaseCard other)
            => string.CompareOrdinal(Emoji, other.Emoji) == 0;

        public override int GetHashCode()
            => HashCode.Combine(Emoji);

        public static bool operator ==(BaseCard left, BaseCard right)
            => EqualityComparer<BaseCard>.Default.Equals(left, right);

        public static bool operator !=(BaseCard left, BaseCard right)
            => !(left == right);

        public override bool Equals(object obj)
            => obj is BaseCard card && this == card;

        public static ImmutableArray<string> AllEmojis { get; } = ";

        private const string AbstractCardTypeCode2 = @"

        public static BaseCard Create(string emoji)
        {
            return emoji switch
            {";

        private const string AbstractCardTypeCode3 = @"
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
                var abstractCode = AbstractCardTypeCode1 + allEmojiList +
                    AbstractCardTypeCode2 + generatedSwitchClauses.ToString() + AbstractCardTypeCode3;

                context.AddSource("BaseCard", SourceText.From(abstractCode, Encoding.UTF8));
            }
            catch (Exception)
            {
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
