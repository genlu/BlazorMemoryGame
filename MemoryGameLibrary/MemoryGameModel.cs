using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Text.Json;
using MemoryGame.Cards;

namespace MemoryGame.Model
{
    public class MemoryGameModel
    {
        // field naming rule
        private readonly int turnDelayDuration;
        private DateTime? timerStart, timerEnd;
        private ICard lastCardSelected;
        private bool isTurningInProgress;

        // readonly modifier rule
        private Timer timer = new Timer(100);

        public ImmutableArray<ICard> ShuffledCards { get; set; }

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
            try
            {
                // Extract method
                ImmutableArray<string> emojis = CardHelpers.AllAnimals;
                var random = new Random();
                emojis = emojis.OrderBy(item => random.Next()).ToImmutableArray();
                // Wrap call chain
                var shuffledCards = emojis.Concat(emojis).OrderBy(item => random.Next()).Select(item => CardHelpers.CreateCard(item)).ToImmutableArray();

                ShuffledCards = shuffledCards.CastArray<ICard>();
                MatchesFound = 0;
                timerStart = timerEnd = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SelectCardAsync(ICard card)
        {
            try
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

                    // Invert condition
                    if (lastCardSelected is not null)
                    {
                        // extract method `SelectSecondCard`
                        if (card.Equals(lastCardSelected))
                        {
                            // Remove redundant equality
                            if (PlayerTurn == true)
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
                            await Task.Delay(turnDelayDuration);
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

                    if (MatchesFound == CardHelpers.AllAnimals.Length)
                    {
                        timerEnd = DateTime.Now;
                        timer.Stop();
                        LatestCompletionTime = timerEnd.Value.Subtract(timerStart.Value).TotalSeconds;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
