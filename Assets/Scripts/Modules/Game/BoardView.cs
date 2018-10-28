using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BoardView : MonoBehaviour
{
    private const int FirstPositionHumanCards = 6;

    private Presenter<BoardView> _presenter;
    private CardView[] _cards;
    private DiContainer _container;

    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private Transform[] _slots;

    [Header("Cards")] [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Transform _cardsParent;
    
    public Action<int> CardSelected;

    [Inject]
    void Init(Presenter<BoardView> presenter, DiContainer container)
    {
        SetInfo("Loading...");
        _presenter = presenter;
        _container = container;
        _cards = new CardView[10];
    
        _presenter.RegisterView(this);
    }

    public void SetInfo(string text)
    {
        _infoText.text = text;
    }

    public void MoveCard(int from, int to)
    {
        _cards[from].transform.SetAsLastSibling();
        CardView oldPileCard = _cards[to];
        _cards[to] = _cards[from];
        LeanTween.move(_cards[from].gameObject,
            _slots[to], 0.2f).setOnComplete(() =>
        {
            Destroy(oldPileCard.gameObject);
        });
    }

    public virtual void AddNewCardTo(int cardPosition, int number)
    {
        var card = GetNewCardView(number);
        card.transform.position = _slots[cardPosition].position;
        card.transform.SetAsFirstSibling();
        card.Index = cardPosition;
        _cards[cardPosition] = card;
        if (cardPosition >= FirstPositionHumanCards)
        {
            card.gameObject.AddComponent<Button>().onClick.AddListener(() =>
            {
                CardSelected?.Invoke(cardPosition);
            });
        }
    }

    private CardView GetNewCardView(int number)
    {
        GameObject instantiatedCard = _container.InstantiatePrefab(_cardPrefab, _cardsParent);
        var cardView = instantiatedCard.GetComponent<CardView>();
        cardView.Num = number;
        return cardView;
    }

    private void OnDestroy()
    {
        _presenter?.Dispose();
    }
}