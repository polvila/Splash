using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CardGeneratorService : ICardGeneratorService 
{
	private const int MaxRange = 12;
	private const int MinRange = 1;
	
	public int GetMaxRange { get { return MaxRange; } }
	public int GetMinRange { get { return MinRange; } }

	private GameObject _card;
	private Transform _gameCanvas;

	public CardGeneratorService(GameObject card, Transform gameCanvas)
	{
		_card = card;
		_gameCanvas = gameCanvas;
	}
	
	public CardView GetRandomCard()
	{
		return GetNewCardView(() => Random.Range(MinRange, MaxRange));
	}

	public CardView GetRandomCardExcluding(CardView[] cards)
	{
		HashSet<int> excludedNumbers = cards.Where(x => x != null && x.Num > 0).Select(x => x.Num).ToHashSet();
		return GetNewCardView(() => GiveMeANumberExcluding(excludedNumbers));
	}

	private CardView GetNewCardView(Func<int> randomGenerator)
	{
		GameObject instantiatedCard = Object.Instantiate(_card, _gameCanvas);
		var cardView = instantiatedCard.GetComponent<CardView>();
		cardView.Num = randomGenerator.Invoke();
		return cardView;
	}
	
	private int GiveMeANumberExcluding(HashSet<int> numbers)
	{
		var range = Enumerable.Range(MinRange, MaxRange).Where(i => !numbers.Contains(i));
		int index = Random.Range(MinRange, MaxRange - numbers.Count);
		return range.ElementAt(index);
	}
}
