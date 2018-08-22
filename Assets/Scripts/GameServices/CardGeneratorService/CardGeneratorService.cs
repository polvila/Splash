using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CardGeneratorService : ICardGeneratorService 
{
	private const int MaxRange = 12;
	private const int MinRange = 1;
	
	public int GetMaxRange => MaxRange;
	public int GetMinRange => MinRange;

	private GameObject _card;
	private Transform _cardsParent;
	private DiContainer _container;
	
	private int _lastNumGenerated = 0;

	public CardGeneratorService(GameObject card, Transform cardsParent, DiContainer container)
	{
		_card = card;
		_cardsParent = cardsParent;
		_container = container;
	}
	
	public CardView GetRandomCard()
	{
		return GetNewCardView(() => Random.Range(MinRange, MaxRange+1));
	}

	public CardView GetRandomCardExcluding(CardView[] cards)
	{
		HashSet<int> excludedNumbers = cards.Where(x => x != null && x.Num > 0).Select(x => x.Num).ToHashSet();
		return GetNewCardView(() => GiveMeANumberExcluding(excludedNumbers));
	}

	private CardView GetNewCardView(Func<int> numGenerator)
	{
		GameObject instantiatedCard = _container.InstantiatePrefab(_card, _cardsParent);
		var cardView = instantiatedCard.GetComponent<CardView>();
		cardView.Num = numGenerator.Invoke();
		return cardView;
	}
	
	private int GiveMeANumberExcluding(HashSet<int> numbers)
	{
		var range = Enumerable.Range(MinRange, MaxRange).Where(i => !numbers.Contains(i));
		var index = Random.Range(MinRange - 1, MaxRange - numbers.Count);
		return range.ElementAt(index);
	}
}
