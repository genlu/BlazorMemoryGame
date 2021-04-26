﻿using System.Text.Json;
using MemoryGame.Cards;
using MemoryGame.Model;
using Xunit;

namespace MemoryGame.Test
{
    public class MemoryGameModelTest
    {
        [Fact]
        public void TestSerialization()
        {
            var card = CatCard.Create("🐱");
            var jsonString = JsonSerializer.Serialize<CatCard>((CatCard)card);  //Simplification fixes 
            Assert.Equal($"{{\"{nameof(CatCard.Emoji)}\":\"\\uD83D\\uDC31\",\"IsTurned\":false,\"IsMatched\":false,\"CssClass\":\"\"}}", jsonString);

            // Convert regular string literal to verbatim string literal
            string toSerialize = $"\r\n{{ \r\n\"{nameof(CatCard.Emoji)}\": \r\n\"\\uD83D\\uDC31\", \r\n\"IsTurned\": false, \r\n\"IsMatched\": false, \r\n\"CssClass\": \"\" \r\n}}";
            var newCard = JsonSerializer.Deserialize<CatCard>(toSerialize);
            Assert.Equal("🐱", newCard.Emoji);
        }

        // Add a test for matching cards

        //[Fact]
        //public void HaveMatchingPairs()
        //{
        //    var model = new MemoryGameModel(0);
        //    for (var i = 0; i < model.ShuffledCards.Length; ++i)
        //    {
        //        var currentCard = model.ShuffledCards[i];
        //        var remainingCards = model.ShuffledCards.RemoveAt(i);
        //        Assert.Contains(currentCard, remainingCards);
        //    }
        //}

        // #7. Add more tests to increase test coverage

        //[Fact]
        //public async Task WhenUserSelectsMatchingPair_StaysMatched()
        //{
        //    // Find a matching pair
        //    var model = new MemoryGameModel(0);
        //    var firstSelection = model.ShuffledCards[0];
        //    var secondSelection = model.ShuffledCards.Skip(1)
        //        .Single(c => c.Emoji == firstSelection.Emoji);

        //    // Select first one
        //    await model.SelectCardAsync(firstSelection);
        //    Assert.True(firstSelection.IsTurned);
        //    Assert.Equal(0, model.MatchesFound);

        //    // Select second one - everything resets
        //    await model.SelectCardAsync(secondSelection);

        //    Assert.True(firstSelection.IsTurned);
        //    Assert.True(secondSelection.IsTurned);
        //    Assert.True(firstSelection.IsMatched);
        //    Assert.True(secondSelection.IsMatched);
        //    Assert.Equal(1, model.MatchesFound);
        //}

        //[Fact]
        //public async Task WhenUserSelectsNonMatchingPair_TurnsBothBack()
        //{
        //    // Find a non-matching pair
        //    var model = new MemoryGameModel(0);
        //    var firstSelection = model.ShuffledCards[0];
        //    var secondSelection = model.ShuffledCards   // Uh-oh, can't find a mismatch since we only have 1 kind of card!
        //        .First(c => c.Emoji == firstSelection.Emoji);

        //    await model.SelectCardAsync(firstSelection);
        //    Assert.True(firstSelection.IsTurned);

        //    await model.SelectCardAsync(secondSelection);
        //    Assert.False(firstSelection.IsTurned);
        //    Assert.False(secondSelection.IsTurned);
        //    Assert.False(firstSelection.IsMatched);
        //    Assert.False(firstSelection.IsMatched);
        //    Assert.Equal(0, model.MatchesFound);
        //}


        //[Fact]
        //public async Task WhenAllMatchesFound_GameEnds()
        //{
        //    var model = new MemoryGameModel(0);
        //    var distinctAnimals = model.ShuffledCards.Select(c => c).Distinct().ToList();

        //    // Select each pair in turn
        //    foreach (var animal in distinctAnimals)
        //    {
        //        Assert.False(model.GameEnded);

        //        var matchingCards = model.ShuffledCards.Where(c => c.Equals(animal)).ToList();
        //        Assert.Equal(2, matchingCards.Count);
        //        await model.SelectCardAsync(matchingCards[0]);
        //        await model.SelectCardAsync(matchingCards[1]);
        //        Assert.True(matchingCards[0].IsMatched);
        //        Assert.True(matchingCards[1].IsMatched);
        //    }

        //    // Finally, the game should be completed
        //    Assert.True(model.GameEnded);
        //    Assert.True(model.LatestCompletionTime != default);
        //}
    }
}
