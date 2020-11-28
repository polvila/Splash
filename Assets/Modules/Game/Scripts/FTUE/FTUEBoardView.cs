using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Modules.Game
{
    public class FTUEBoardView : BoardView, IBoardView
    {
        [SerializeField] private FTUEView _ftueView;

        [Inject] private IPlayerModel _playerModel;
        private bool _mainFtueCompleted;
        
        public event Action StartGameEvent;

        public override void StartCountdown(Action onComplete)
        {
            _ftueView.SequenceEnded += isMissFtue =>
            {
                if (!isMissFtue)
                {
                    StartGameEvent?.Invoke();
                    _mainFtueCompleted = true;
                }
            };
            _countdownView.StartCountdown(onComplete);
        }

        public override void ShowSplash(bool fromHumanPlayer, int totalPoints)
        {
            _ftueView.Trigger(FTUETrigger.Splash);
            base.ShowSplash(fromHumanPlayer, totalPoints);
        }

        public override void MoveCard(int from, int to, Action onComplete = null)
        {
            if (from > RightMiddlePositionCard)
            {
                _ftueView.Trigger(FTUETrigger.PlayerValidPlay);
            }
            else if(from < LeftMiddlePositionCard)
            {
                _ftueView.Trigger(FTUETrigger.EnemyPlay);
            }
            
            base.MoveCard(from, to, () =>
            {
                if (from > RightMiddlePositionCard)
                {
                    _ftueView.Trigger(FTUETrigger.PlayerPlayed);
                }
                else if(from < LeftMiddlePositionCard)
                {
                    _ftueView.Trigger(FTUETrigger.EnemyPlayed);
                }
                if (Cards[LeftMiddlePositionCard].Num == Cards[RightMiddlePositionCard].Num)
                {
                    _ftueView.Trigger(FTUETrigger.SplashReady);
                }
                
                if (!_mainFtueCompleted && _ftueView.GetNextPositionToMove(out int position) &&
                    position >= 0 && 
                    position < LeftMiddlePositionCard)
                {
                    OnCardClicked(_ftueView.NextPositionToMove);
                }
                onComplete?.Invoke();
            });
        }

        public override void UnblockMiddleCards(int newLeftNumber, int newRightNumber)
        {
            _ftueView.UnblockAction = () => { base.UnblockMiddleCards(newLeftNumber, newRightNumber); };
            _ftueView.Trigger(FTUETrigger.Unblock);
        }

        protected override void UpdatePiles(int newLeftNumber, int newRightNumber, Action onComplete = null)
        {
            base.UpdatePiles(newLeftNumber, newRightNumber, () =>
            {
                _ftueView.Trigger(FTUETrigger.Unblocked);
                onComplete?.Invoke();
            });
        }
        
        protected override void OnCardClicked(int cardPosition)
        {
            if (_mainFtueCompleted || !_mainFtueCompleted && cardPosition == _ftueView.NextPositionToMove)
            {
                base.OnCardClicked(cardPosition);
            }
        }

        public override void MissCardMove(int from, Action onComplete = null)
        {
            base.MissCardMove(from, () =>
            {
                _ftueView.Trigger(FTUETrigger.PlayerInvalidPlay);
                onComplete?.Invoke();
            });
        }

        public override void FinishGame(Action onComplete)
        {
            _playerModel.FTUECompleted = true;
            base.FinishGame(onComplete);
        }

        protected override IEnumerator WaitUntilAnimationsEnd(Action onComplete)
        {
            yield return base.WaitUntilAnimationsEnd(() =>
            {
                StartCoroutine(WaitUntilFtueEnd(onComplete));
            });
        }

        private IEnumerator WaitUntilFtueEnd(Action onComplete)
        {
            yield return new WaitForSeconds(0.1f); //Wait for the possible miss FTUE screen
            yield return new WaitUntil(() => _ftueView.gameObject.activeSelf == false);
            onComplete?.Invoke();
        }
    }
}