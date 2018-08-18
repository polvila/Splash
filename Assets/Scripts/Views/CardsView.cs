using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

public class CardsView : MonoBehaviour {

	[HideInInspector] public CardView[] Cards;
	[HideInInspector] public Transform[] Slots;

	protected ICardGeneratorService _cardGeneratorService;
	
	[Inject]
	void Init(ICardGeneratorService cardGeneratorService)
	{
		_cardGeneratorService = cardGeneratorService;
	}
	
	void Awake()
	{
		Cards = new CardView[transform.childCount];
		Slots = new Transform[transform.childCount];
		
		for (int i = 0; i < Slots.Length; ++i)
		{
			Slots[i] = transform.GetChild(i);
		}
		
		FillSlotsWithCards();
	}

	protected virtual void FillSlotsWithCards()
	{
		for (int i = 0; i < Cards.Length; ++i)
		{
			if (Cards[i] != null) continue;
			Cards[i] = GetNewCard(i);
		}
	}

	protected CardView GetNewCard(int index)
	{
		var card = _cardGeneratorService.GetRandomCardExcluding(Cards);
		card.transform.position = Slots[index].position;
		card.Index = index;
		return card;
	}
}
