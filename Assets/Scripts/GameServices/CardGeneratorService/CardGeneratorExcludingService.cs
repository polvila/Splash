using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

public class CardGeneratorExcludingService : CardGeneratorService
{
    public CardGeneratorExcludingService(GameObject card, Transform cardsParent, DiContainer container) : base(card, cardsParent, container)
    {
    }

    public override CardView GenerateCard()
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
        return GetNewCardView(() => GiveMeANumberExcluding(excludedNumbers));
    }
    
    private int GiveMeANumberExcluding(HashSet<int> numbers)
    {
        var range = Enumerable.Range(MinRange, MaxRange).Where(i => !numbers.Contains(i));
        var index = Random.Range(MinRange - 1, MaxRange - numbers.Count);
        return range.ElementAt(index);
    }
}
