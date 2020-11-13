using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BoardView : MonoBehaviour
{
    private const int FirstPositionHumanCards = 6;
    private const int LeftMiddlePositionCard = 4;
    private const int RightMiddlePositionCard = 5;

    private Presenter<BoardView> _presenter;
    private CardView[] _cards;
    private DiContainer _container;
    private bool _cardsArePlayable = true;
    private IDisposable _setInfoTimer;
    private Transform[] _slots;

    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private TMP_Text _countdownText;
    [SerializeField] private Transform[] _slotContainers;

    [Header("Splash")] [SerializeField] private Button _splashZone;
    [SerializeField] private SplashView _humanSplash;
    [SerializeField] private SplashView _enemySplash;


    [Header("Cards")] [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _enemyCardPrefab;
    [SerializeField] private Transform _cardsParent;

    public Action SplashZoneSelected;
    public Action<int> CardSelected;

    [Inject]
    void Init(Presenter<BoardView> presenter, DiContainer container)
    {
        _presenter = presenter;
        _container = container;
        _cards = new CardView[10];
        _slots = new Transform[10];

        var i = 0;
        foreach (var _slotContainer in _slotContainers)
        {
            foreach (Transform slot in _slotContainer)
            {
                _slots[i] = slot;
                ++i;
            }
        }

        _presenter.RegisterView(this);
    }

    private void Awake()
    {
        _splashZone.onClick.AddListener(() =>
        {
            if (!_cardsArePlayable && !SROptions.Current.GodMode) return;

            SplashZoneSelected?.Invoke();
        });
    }

    public void SetInfo(string text, bool timed = true)
    {
        _infoText.text = text;

        if (text == "" || !timed) return;

        _setInfoTimer?.Dispose();
        _setInfoTimer = Observable
            .Timer(TimeSpan.FromSeconds(1))
            .Subscribe(x => { SetInfo(""); });
    }

    public void MoveCard(int from, int to)
    {
        _cards[from].transform.SetAsLastSibling();
        CardView oldPileCard = _cards[to];
        _cards[to] = _cards[from];
        _cards[to].Index = to;
        LeanTween.move(_cards[from].gameObject,
            _slots[to], 0.2f).setOnComplete(() => { Destroy(oldPileCard.gameObject); });
    }

    public void ShowCardMiss(int position)
    {
        _cards[position].TriggerMissAnimationFrom(_slots[position].position);
    }

    public void DestroyCard(int position, float delay = 0)
    {
        if (_cards[position] == null) return;
        
        Destroy(_cards[position].gameObject, delay);
        _cards[position] = null;
    }

    public virtual void AddNewCardTo(int cardPosition, int number)
    {
        var card = GetNewCardView(cardPosition <= LeftMiddlePositionCard, number);

        card.Index = cardPosition;
        _cards[cardPosition] = card;

        Vector2 edgeVector = GetComponent<Canvas>().worldCamera.ViewportToWorldPoint(new Vector2(1, 0.5f));

        if (cardPosition == LeftMiddlePositionCard || cardPosition == RightMiddlePositionCard)
        {
            var offset = edgeVector.x * (cardPosition == LeftMiddlePositionCard ? -1 : 1);
            card.transform.position =
                new Vector2(_slots[cardPosition].position.x + offset, _slots[cardPosition].position.y);
            LeanTween.move(card.gameObject, _slots[cardPosition], 0.2f);
            card.transform.SetAsLastSibling();
        }
        else
        {
            card.transform.position = _slots[cardPosition].position;
            card.transform.SetAsFirstSibling();
        }

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

    public void StartCountdown(Action onComplete)
    {
        var seq = LeanTween.sequence();
        seq.append(() => { _countdownText.text = "3"; });
        seq.append(1f);
        seq.append(() => { _countdownText.text = "2"; });
        seq.append(1f);
        seq.append(() => { _countdownText.text = "1"; });
        seq.append(1f);
        seq.append(() =>
        {
            _countdownText.text = "";
            onComplete?.Invoke();
        });
    }

    private CardView GetNewCardView(bool enemyCard, int number)
    {
        var instantiatedCard = _container.InstantiatePrefab(enemyCard ? _enemyCardPrefab : _cardPrefab, _cardsParent);
        var cardView = instantiatedCard.GetComponent<CardView>();
        cardView.Num = number;
        return cardView;
    }

    public void ShowSplash(bool fromHumanPlayer, int totalPoints)
    {
        var splash = fromHumanPlayer ? _humanSplash : _enemySplash;
        splash.Show(totalPoints);
    }

    private void OnDestroy()
    {
        _setInfoTimer?.Dispose();
        _presenter?.Dispose();
        foreach (var card in _cards)
        {
            if (card == null) continue;
            card.GetComponent<Button>()?.onClick.RemoveAllListeners();
        }

        _splashZone.onClick.RemoveAllListeners();
    }
}