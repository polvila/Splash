using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BoardView : MonoBehaviour
{
    private const int FirstPositionHumanCards = 6;

    private Presenter<BoardView> _presenter;
    private CardView[] _cards;
    private DiContainer _container;
    private bool _cardsArePlayable = true;
    private IDisposable _setInfoTimer;

    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private Transform[] _slots;
    [SerializeField] private Button SplashZone;

    [Header("Cards")] [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Transform _cardsParent;

    public Action SplashZoneSelected;
    public Action<int> CardSelected;

    [Inject]
    void Init(Presenter<BoardView> presenter, DiContainer container)
    {
        SetInfo("Loading...", false);
        _presenter = presenter;
        _container = container;
        _cards = new CardView[10];
    
        _presenter.RegisterView(this);
    }

    private void Awake()
    {
        SplashZone.onClick.AddListener(() =>
        {
            if (!_cardsArePlayable && !SROptions.Current.GodMode) return;
            
            SplashZoneSelected?.Invoke();
        });
    }

    public void SetInfo(string text, bool timed = true)
    {
        _infoText.text = text;
        
        if(text == "" || !timed) return;
        
        _setInfoTimer?.Dispose();
        _setInfoTimer = Observable
            .Timer(TimeSpan.FromSeconds(1))
            .Subscribe(x =>
            {
                SetInfo("");
            });
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
    
    public void DestroyCard(int position)
    {
        Destroy(_cards[position]?.gameObject);        
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
                if (!_cardsArePlayable && !SROptions.Current.GodMode) return;
                    
                CardSelected?.Invoke(cardPosition);
            });
        }
    }

    public void StopPlayableCards()
    {
        _cardsArePlayable = false;
    }

    private CardView GetNewCardView(int number)
    {
        var instantiatedCard = _container.InstantiatePrefab(_cardPrefab, _cardsParent);
        var cardView = instantiatedCard.GetComponent<CardView>();
        cardView.Num = number;
        return cardView;
    }

    private void OnDestroy()
    {
        _setInfoTimer?.Dispose();
        _presenter?.Dispose();
        foreach (var card in _cards)
        {
            if(card == null) continue;
            card.GetComponent<Button>()?.onClick.RemoveAllListeners();
        }
        SplashZone.onClick.RemoveAllListeners();
    }
}