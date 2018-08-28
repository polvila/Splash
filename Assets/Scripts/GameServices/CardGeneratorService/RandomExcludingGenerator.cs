using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

public class RandomExcludingGenerator : RandomGenerator
{
    public RandomExcludingGenerator(IGameStateModel gameStateModel, DiContainer container, GameObject card, Transform cardsParent) : base(gameStateModel, container, card, cardsParent)
    {
    }

    public override CardView GenerateCard(int minRange, int maxRange)
    {
        List<CardView> cards = new List<CardView>();
        if (GameStateModel.EnemyPlayer.Cards != null)
        {
            cards.AddRange(GameStateModel.EnemyPlayer.Cards);
        }

        if (GameStateModel.HumanPlayer.Cards != null)
        {
            cards.AddRange(GameStateModel.HumanPlayer.Cards);
        }
       
        HashSet<int> excludedNumbers = cards.Where(x => x != null && x.Num > 0).Select(x => x.Num).ToHashSet();
        return GetNewCardView(() => GiveMeANumberExcluding(excludedNumbers, minRange, maxRange));
    }
    
    private int GiveMeANumberExcluding(HashSet<int> numbers, int minRange, int maxRange)
    {
        var range = Enumerable.Range(minRange, maxRange).Where(i => !numbers.Contains(i));
        var index = Random.Range(minRange - 1, maxRange - numbers.Count);
        return range.ElementAt(index);
    }
}
