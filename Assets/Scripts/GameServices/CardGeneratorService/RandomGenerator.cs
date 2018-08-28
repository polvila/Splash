using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class RandomGenerator
{
    protected IGameStateModel GameStateModel;
    private DiContainer _container;
    
    private GameObject _card;
    private Transform _cardsParent;
    
    public RandomGenerator(IGameStateModel gameStateModel, DiContainer container, GameObject card, Transform cardsParent)
    {
        GameStateModel = gameStateModel;
        _container = container;
        _card = card;
        _cardsParent = cardsParent;
    }
    
    public virtual CardView GenerateCard(int minRange, int maxRange)
    {
        return GetNewCardView(() => Random.Range(minRange, maxRange+1));
    }

    protected CardView GetNewCardView(Func<int> numGenerator)
    {
        GameObject instantiatedCard = _container.InstantiatePrefab(_card, _cardsParent);
        var cardView = instantiatedCard.GetComponent<CardView>();
        cardView.Num = numGenerator.Invoke();
        return cardView;
    }
}
