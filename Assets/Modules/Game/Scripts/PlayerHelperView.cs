using UnityEngine;
using Zenject;

namespace Modules.Game
{
    public class PlayerHelperView : MonoBehaviour
    {
        [SerializeField] private BoardView _boardView;

        private int _delayedCallId;
        private IGameManagerService _gameManagerService;
        private CardView _lastShiningCard;
        
        [Inject]
        private void Init(IGameManagerService gameManagerService)
        {
            _gameManagerService = gameManagerService;
        }

        private void Start()
        {
            _gameManagerService.NewGameReceived += OnNewGameReceived;
            _gameManagerService.CardUpdate += OnCardUpdate;
            _gameManagerService.GameFinished += OnGameFinished;
            _gameManagerService.Splashed += OnSplashed;
            _gameManagerService.Unblocked += OnUnblocked;
        }
        
        private void OnNewGameReceived(int[] numbers)
        {
            StartDelayedHelpIn(5f);
        }
        
        private void OnCardUpdate(int fromCardPosition, int toCardPosition, int? newNumber)
        {
            if (fromCardPosition == toCardPosition) return;
            StartDelayedHelpIn();
        }

        private void OnGameFinished(int result, bool newRecord)
        {
            StopDelayedHelp();
        }
        
        private void OnSplashed(bool wasHuman, int newLeftNumber, int newRightNumber, int points)
        {
            StopDelayedHelp();
        }

        private void OnUnblocked(int newLeftNumber, int newRightNumber)
        {
            StartDelayedHelpIn();
        }

        private void StartDelayedHelpIn(float seconds = 2f)
        {
            if (_boardView is FTUEBoardView ftueBoard && !ftueBoard.MainFtueCompleted)
            {
                return;
            }
            
            if (_lastShiningCard != null)
            {
                _lastShiningCard.Shine = false;
            }
            StopDelayedHelp();

            _delayedCallId = LeanTween.delayedCall(seconds, () =>
            {
                if (GetPlayableCard(out var card))
                {
                    _lastShiningCard = card;
                    _lastShiningCard.Shine = true;
                }
            }).id;
        }

        private void StopDelayedHelp()
        {
            if (LeanTween.isTweening(_delayedCallId))
            {
                LeanTween.cancel(_delayedCallId);
            }
        }

        private bool GetPlayableCard(out CardView card)
        {
            var cards = _boardView.Cards;
            card = null;
            for (var i = BoardView.RightStackPosition + 1; i < cards.Length; ++i)
            {
                if(GameManagerServiceMock.IsACompatibleMove(cards[i].Num, cards[BoardView.LeftStackPosition].Num) ||
                   GameManagerServiceMock.IsACompatibleMove(cards[i].Num, cards[BoardView.RightStackPosition].Num))
                {
                    card = cards[i];
                    return true;
                }
            }

            return false;
        }

        private void OnDestroy()
        {
            StopDelayedHelp();

            _gameManagerService.NewGameReceived -= OnNewGameReceived;
            _gameManagerService.CardUpdate -= OnCardUpdate;
            _gameManagerService.GameFinished -= OnGameFinished;
            _gameManagerService.Unblocked -= OnUnblocked;
        }
    }
}