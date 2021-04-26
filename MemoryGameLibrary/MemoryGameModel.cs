using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using MemoryGame.Cards;

namespace MemoryGame.Model
{
    public class MemoryGameModel
    {
        // field naming rule
        private readonly int turnDelayDuration;
        private DateTime? timerStart, timerEnd;
        private BaseCard lastCardSelected;
        private bool isTurningInProgress;

        // readonly modifier rule
        private Timer timer = new Timer(100);

        public ImmutableArray<BaseCard> ShuffledCards { get; set; }

        public int MatchesFound { get; private set; }
        public int MatchesFoundP1 { get; private set; }
        public int MatchesFoundP2 { get; private set; }

        public TimeSpan GameTimeElapsed
            => timerStart.HasValue ? timerEnd.GetValueOrDefault(DateTime.Now).Subtract(timerStart.Value) : default;

        public bool GameEnded => timerEnd.HasValue;
        public double LatestCompletionTime { get; private set; } = -1;

        public bool PlayerTurn { get; private set; }

        public event ElapsedEventHandler TimerElapsed
        {
            add { timer.Elapsed += value; }
            remove { timer.Elapsed -= value; }
        }

        public MemoryGameModel(int turnDelayDuration)
        {
            this.turnDelayDuration = turnDelayDuration;
            PlayerTurn = true;
            ResetGame();
        }

        public void ResetGame()
        {
            // Add exception handling here
            ShuffledCards = CreateShuffledCardPairs(BaseCard.AllEmojis);
            MatchesFound = 0;
            timerStart = timerEnd = null;
        }

        private ImmutableArray<BaseCard> CreateShuffledCardPairs(ImmutableArray<string> emojis)
        {
            emojis = Shuffle(emojis);
            var random = new Random();
            return emojis.Concat(emojis).OrderBy(item => random.Next()).Select(item => BaseCard.Create(item)).ToImmutableArray();
        }


        // Static modifier rule
        private ImmutableArray<string> Shuffle(ImmutableArray<string> emojis)
        {
            var random = new Random();
            return emojis.OrderBy(item => random.Next()).ToImmutableArray();
        }

        public async Task SelectCardAsync(BaseCard card)
        {
            if (!timer.Enabled)
            {
                timerStart = DateTime.Now;
                timer.Start();
            }

            // Simplify conditional expression
            if (!card.IsTurned ? !isTurningInProgress : false)
            {
                card.IsTurned = true;

                if (lastCardSelected is not null)
                {
                    if (card.Equals(lastCardSelected))
                    {
                        // Remove redundant equality
                        if (PlayerTurn == true) //Player 1 = true 
                        {
                            MatchesFoundP1++;
                        }
                        else
                        {
                            MatchesFoundP2++;
                        }

                        MatchesFound++;
                        card.IsMatched = lastCardSelected.IsMatched = true;
                    }
                    else
                    {
                        isTurningInProgress = true;
                        await Task.Delay(turnDelayDuration); // Pause before turning back
                        isTurningInProgress = false;
                        PlayerTurn = !PlayerTurn;
                        card.IsTurned = lastCardSelected.IsTurned = false;
                    }

                    lastCardSelected = null;
                }
                else
                {
                    lastCardSelected = card;
                }

                if (MatchesFound == BaseCard.AllEmojis.Length)
                {
                    timerEnd = DateTime.Now;
                    timer.Stop();
                    LatestCompletionTime = timerEnd.Value.Subtract(timerStart.Value).TotalSeconds;
                }
            }
        }
    }
}
