using System;
using System.Collections;
using Core.AssetManagement.APIs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Modules.Game
{
    public class BoardView : MonoBehaviour, IBoardView
    {
        protected const int FirstPositionHumanCards = 6;
        protected const int LeftMiddlePositionCard = 4;
        protected const int RightMiddlePositionCard = 5;

        private Presenter<IBoardView> _presenter;
        [HideInInspector] public CardView[] Cards;
        private Transform[] _slots;
        private IAssetManager _assetManager;

        [SerializeField] private Mode _gameMode;
        [SerializeField] protected CountdownView _countdownView;
        [SerializeField] private Transform[] _slotContainers;

        [Header("Splash")] [SerializeField] private Button _splashZone;
        [SerializeField] private SplashView _humanSplash;
        [SerializeField] private SplashView _enemySplash;

        [Header("Cards")] [SerializeField] private string _cardPrefabName;
        [SerializeField] private string _enemyCardPrefabName;
        [SerializeField] private Transform _cardsParent;

        public event Action SplashZoneSelected;
        public event Action<int> CardSelected;
        public event Action StartGameEvent;

        public Mode GameMode => _gameMode;

        [Inject]
        private void Init(
            Presenter<IBoardView> presenter,
            IAssetManager assetManager)
        {
            _presenter = presenter;
            _assetManager = assetManager;
            Cards = new CardView[10];
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
        }

        private void Awake()
        {
            _splashZone.onClick.AddListener(() =>
            {
                SplashZoneSelected?.Invoke();
            });
            
            _presenter.RegisterView(this);
        }

        public virtual void MoveCard(int from, int to, Action onComplete = null)
        {
            CardView oldPileCard = Cards[to];
            Cards[to] = Cards[from];
            Cards[from].MoveFrom(_slots[from], _slots[to], () =>
            {
                Destroy(oldPileCard.gameObject);
                onComplete?.Invoke();
            });
        }

        public virtual void MissCardMove(int from, Action onComplete = null)
        {
            Cards[from].TriggerMissAnimationFrom(_slots[from], onComplete);
        }

        public void DestroyCard(int position, float delay = 0)
        {
            if (Cards[position] == null) return;

            Destroy(Cards[position].gameObject, delay);
            Cards[position] = null;
        }

        public void AddNewCardTo(int cardPosition, int number, Action onComplete = null)
        {
            var card = GetNewCardView(cardPosition <= LeftMiddlePositionCard, number);

            Cards[cardPosition] = card;

            Vector2 edgeVector = GetComponent<Canvas>().worldCamera.ViewportToWorldPoint(new Vector2(1, 0.5f));

            if (cardPosition == LeftMiddlePositionCard || cardPosition == RightMiddlePositionCard)
            {
                var offset = edgeVector.x * (cardPosition == LeftMiddlePositionCard ? -1 : 1);
                card.transform.position =
                    new Vector2(_slots[cardPosition].position.x + offset, _slots[cardPosition].position.y);
                LeanTween.move(card.gameObject, _slots[cardPosition], 0.2f).setOnComplete(onComplete);
                card.transform.SetAsLastSibling();
            }
            else
            {
                card.transform.position = _slots[cardPosition].position;
                card.transform.SetAsFirstSibling();
                onComplete?.Invoke();
            }

            if (cardPosition >= FirstPositionHumanCards)
            {
                card.Button.onClick.AddListener(() => OnCardClicked(cardPosition));
            }
        }

        protected virtual void OnCardClicked(int cardPosition)
        {
            CardSelected?.Invoke(cardPosition);
        }

        public virtual void FinishGame(Action onComplete)
        {
            _countdownView.gameObject.SetActive(true);
            StartCoroutine(WaitUntilAnimationsEnd(onComplete));
        }

        protected virtual IEnumerator WaitUntilAnimationsEnd(Action onComplete)
        {
            foreach (var cardView in Cards)
            {
                if (cardView != null)
                {
                    yield return new WaitUntil(() => !LeanTween.isTweening(cardView.gameObject));
                }
            }

            yield return new WaitUntil(() => !LeanTween.isTweening(_humanSplash.gameObject));
            yield return new WaitUntil(() => !LeanTween.isTweening(_enemySplash.gameObject));
            onComplete?.Invoke();
        }

        public virtual void StartCountdown(Action onComplete)
        {
            _countdownView.StartCountdown(() =>
            {
                StartGameEvent?.Invoke();
                onComplete?.Invoke();
            });
        }

        private CardView GetNewCardView(bool enemyCard, int number)
        {
            var instantiatedCard =
                _assetManager.InstantiatePrefab(enemyCard ? _enemyCardPrefabName : _cardPrefabName, _cardsParent);
            var cardView = instantiatedCard.GetComponent<CardView>();
            cardView.Num = number;
            return cardView;
        }

        public virtual void ShowSplash(bool fromHumanPlayer, int totalPoints)
        {
            var splash = fromHumanPlayer ? _humanSplash : _enemySplash;
            splash.Show(totalPoints);
        }

        public virtual void UnblockMiddleCards(int newLeftNumber, int newRightNumber)
        {
            UpdatePiles(newLeftNumber, newRightNumber);
        }

        protected virtual void UpdatePiles(int newLeftNumber, int newRightNumber, Action onComplete = null)
        {
            DestroyCard(LeftMiddlePositionCard, 0.3f);
            DestroyCard(RightMiddlePositionCard, 0.3f);
            AddNewCardTo(LeftMiddlePositionCard, newLeftNumber);
            AddNewCardTo(RightMiddlePositionCard, newRightNumber, onComplete);
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
            foreach (var card in Cards)
            {
                if (card == null) continue;
                card.Button.onClick.RemoveAllListeners();
            }

            _splashZone.onClick.RemoveAllListeners();
        }
    }
}