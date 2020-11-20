using System;
using System.Collections;
using Core.AssetManagement.APIs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Modules.Game
{
    public class BoardView : MonoBehaviour
    {
        private const int FirstPositionHumanCards = 6;
        private const int LeftMiddlePositionCard = 4;
        private const int RightMiddlePositionCard = 5;

        private Presenter<BoardView> _presenter;
        private CardView[] _cards;
        private bool _cardsArePlayable;
        private Transform[] _slots;
        private IAssetManager _assetManager;
        private FTUEView _ftueView;

        [SerializeField] private CountdownView _countdownView;
        [SerializeField] private Transform[] _slotContainers;

        [Header("Splash")] [SerializeField] private Button _splashZone;
        [SerializeField] private SplashView _humanSplash;
        [SerializeField] private SplashView _enemySplash;

        [Header("Cards")] [SerializeField] private string _cardPrefabName;
        [SerializeField] private string _enemyCardPrefabName;
        [SerializeField] private Transform _cardsParent;

        public Action SplashZoneSelected;
        public Action<int> CardSelected;

        [Inject]
        void Init(Presenter<BoardView> presenter,
            IAssetManager assetManager)
        {
            _presenter = presenter;
            _assetManager = assetManager;
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

        public void MoveCard(int from, int to)
        {
            CardView oldPileCard = _cards[to];
            _cards[to] = _cards[from];
            _cards[to].Index = to;
            _cards[from].MoveFrom(_slots[from], _slots[to], () =>
            {
                TriggerFTUEs(from);
                Destroy(oldPileCard.gameObject);
            });
        }

        public void MissCardMove(int from, Action onComplete)
        {
            _cards[from].TriggerMissAnimationFrom(_slots[from], onComplete);
        }

        public void DestroyCard(int position, float delay = 0)
        {
            if (_cards[position] == null) return;

            Destroy(_cards[position].gameObject, delay);
            _cards[position] = null;
        }

        public virtual void AddNewCardTo(int cardPosition, int number, Action onComplete = null)
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
                card.Button.onClick.AddListener(() =>
                {
                    if (!_cardsArePlayable && !SROptions.Current.GodMode) return;

                    CardSelected?.Invoke(cardPosition);
                });
            }
        }

        public void SetCardsArePlayable(bool active)
        {
            _cardsArePlayable = active;
        }

        public void FinishGame(Action onComplete)
        {
            _cardsArePlayable = false;
            StartCoroutine(WaitUntilAnimationsEnd(onComplete));
        }

        private IEnumerator WaitUntilAnimationsEnd(Action onComplete)
        {
            foreach (var cardView in _cards)
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

        public void StartCountdown(Action onComplete)
        {
            _countdownView.StartCountdown(onComplete);
        }

        private CardView GetNewCardView(bool enemyCard, int number)
        {
            var instantiatedCard =
                _assetManager.InstantiatePrefab(enemyCard ? _enemyCardPrefabName : _cardPrefabName, _cardsParent);
            var cardView = instantiatedCard.GetComponent<CardView>();
            cardView.Num = number;
            return cardView;
        }

        public void ShowSplash(bool fromHumanPlayer, int totalPoints)
        {
            var splash = fromHumanPlayer ? _humanSplash : _enemySplash;
            splash.Show(totalPoints);
        }

        public void UnblockMiddleCards(int newLeftNumber, int newRightNumber)
        {
            if (_ftueView == null)
            {
                UpdatePiles(newLeftNumber, newRightNumber);
            }
            else
            {
                _ftueView.UpdatePiles += () => UpdatePiles(newLeftNumber, newRightNumber);
                _ftueView.Trigger(FTUETrigger.BeforeUnblocked);
            }
        }

        private void UpdatePiles(int newLeftNumber, int newRightNumber)
        {
            DestroyCard(LeftMiddlePositionCard, 0.3f);
            DestroyCard(RightMiddlePositionCard, 0.3f);
            AddNewCardTo(LeftMiddlePositionCard, newLeftNumber);
            AddNewCardTo(RightMiddlePositionCard, newRightNumber, TriggerFTUEAfterUnblocked);
        }

        public void OpenFTUE(bool miss = false)
        {
            if (miss)
            {
                _assetManager.InstantiatePrefab("MissFTUE", transform);
            }
            else
            {
                _ftueView = _assetManager.InstantiatePrefab("FTUE", transform).GetComponent<FTUEView>();
            }
        }

        private void TriggerFTUEs(int cardMovedPosition)
        {
            if (_ftueView == null) return;

            if (cardMovedPosition < LeftMiddlePositionCard)
            {
                _ftueView.Trigger(FTUETrigger.EnemyPlayed);
            }
            else if (cardMovedPosition > RightMiddlePositionCard)
            {
                _ftueView.Trigger(FTUETrigger.PlayerPlayed);
            }

            if (_cards[LeftMiddlePositionCard].Num == _cards[RightMiddlePositionCard].Num)
            {
                _ftueView.Trigger(FTUETrigger.SplashReady);
            }
        }

        private void TriggerFTUEAfterUnblocked()
        {
            if (_ftueView == null) return;
            _ftueView.Trigger(FTUETrigger.AfterUnblocked);
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
            foreach (var card in _cards)
            {
                if (card == null) continue;
                card.Button.onClick.RemoveAllListeners();
            }

            _splashZone.onClick.RemoveAllListeners();
        }
    }
}