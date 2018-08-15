using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

public class CardGeneratorService : ICardGeneratorService 
{
	private const int MaxRange = 12;
	private const int MinRange = 1;
	
	private GameObject _card;
	private Transform _gameCanvas;

	public CardGeneratorService(GameObject card, Transform gameCanvas)
	{
		_card = card;
		_gameCanvas = gameCanvas;
	}
	
	public void GetRandomCard()
	{
		GameObject instantiatedCard = Object.Instantiate(_card, _gameCanvas);
		instantiatedCard.GetComponent<CardView>().Num = Random.Range(MinRange, MaxRange);
	}

	public void GetPseudoRandomCard(int[] excludedNumbers)
	{
		GameObject instantiatedCard = Object.Instantiate(_card, _gameCanvas);
		instantiatedCard.GetComponent<CardView>().Num = GiveMeANumberExcluding(excludedNumbers.ToHashSet());
	}
	
	private int GiveMeANumberExcluding(HashSet<int> numbers)
	{
		var range = Enumerable.Range(MinRange, MaxRange).Where(i => !numbers.Contains(i));
		int index = Random.Range(MinRange, MaxRange - numbers.Count);
		return range.ElementAt(index);
	}
}
