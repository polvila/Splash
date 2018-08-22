using UnityEngine;
using Zenject;

public class CardsZone {

	public CardView[] Cards;
	public Transform[] Slots;

	private ICardGeneratorService _cardGeneratorService;
	
	[Inject]
	void Init(ICardGeneratorService cardGeneratorService)
	{
		_cardGeneratorService = cardGeneratorService;
	}
	
	public void FillSlotsWithCards()
	{
		Cards = new CardView[Slots.Length];
		
		for (int i = 0; i < Cards.Length; ++i)
		{
			if (Cards[i] != null) continue;
			GetNewCard(i);
		}
	}

	protected virtual void GetNewCard(int index)
	{
		var card = _cardGeneratorService.GetRandomCardExcluding(Cards);
		//var card = _cardGeneratorService.GetRandomCard();
		card.transform.SetAsFirstSibling();
		card.transform.position = Slots[index].position;
		card.Index = index;
		Cards[index] = card;
	}
}
