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
	protected const int MaxRange = 12;
	protected const int MinRange = 1;

	protected IGameStateModel GameStateModel;
	
	public int GetMaxRange => MaxRange;
	public int GetMinRange => MinRange;

	private GameObject _card;
	private Transform _cardsParent;
	private DiContainer _container;

	protected CardGeneratorService(GameObject card, Transform cardsParent, DiContainer container)
	{
		_card = card;
		_cardsParent = cardsParent;
		_container = container;
	}
	
	[Inject]
	void Init(IGameStateModel gameStateModel)
	{
		GameStateModel = gameStateModel;
	}
	
	public virtual CardView GenerateCard()
	{
		return GetNewCardView(() => Random.Range(MinRange, MaxRange+1));
	}

	protected CardView GetNewCardView(Func<int> numGenerator)
	{
		GameObject instantiatedCard = _container.InstantiatePrefab(_card, _cardsParent);
		var cardView = instantiatedCard.GetComponent<CardView>();
		cardView.Num = numGenerator.Invoke();
		return cardView;
	}
}
