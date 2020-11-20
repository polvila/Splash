using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Modules.Game
{
    public enum HighlightedPosition
    {
        None,
        Enemy,
        Middle,
        Player,
        HealthBar,
        OnePlayerCard,
        OnePlayerToMiddle,
        Points
    }

    public enum FTUETrigger
    {
        None,
        EnemyPlayed,
        PlayerPlayed,
        SplashReady,
        AfterUnblocked,
        BeforeUnblocked
    }

    public enum HideScreenCondition
    {
        None,
        CardMovement,
        Splash
    }

    public class FTUEView : MonoBehaviour
    {
        [SerializeField] private FTUESequence _ftueSequence;
        [SerializeField] private Transform _darkBackground;
        [SerializeField] private Image _screenTouchBlocker;
        [SerializeField] private bool _missFTUE;

        [Header("Masks")] [SerializeField] private Transform _enemyMask;
        [SerializeField] private Transform _middleMask;
        [SerializeField] private Transform _playerMask;
        [SerializeField] private Transform _healthBarMask;
        [SerializeField] private Transform _onePlayerCardMask;
        [SerializeField] private Transform _playerToMiddleCardMask;
        [SerializeField] private Transform _pointsMask;

        [Header("Texts")] [SerializeField] private TMP_Text _topText;
        [SerializeField] private TMP_Text _bottomText;
        [SerializeField] private TMP_Text _continueText;

        private Dictionary<HighlightedPosition, Transform> _masks;
        private int _currentScreenIndex = -1;
        private IGameManagerService _gameManagerService;
        private IPlayerModel _playerModel;
        private IAIManagerService _aiManagerService;

        public event Action UpdatePiles;
        public event Action OnClose;

        [Inject]
        private void Init(
            IGameManagerService gameManagerService, 
            IPlayerModel playerModel,
            IAIManagerService aiManagerService)
        {
            _masks = new Dictionary<HighlightedPosition, Transform>
            {
                {HighlightedPosition.None, transform},
                {HighlightedPosition.Enemy, _enemyMask},
                {HighlightedPosition.Middle, _middleMask},
                {HighlightedPosition.Player, _playerMask},
                {HighlightedPosition.HealthBar, _healthBarMask},
                {HighlightedPosition.OnePlayerCard, _onePlayerCardMask},
                {HighlightedPosition.OnePlayerToMiddle, _playerToMiddleCardMask},
                {HighlightedPosition.Points, _pointsMask},
            };
            _gameManagerService = gameManagerService;
            _gameManagerService.CardUpdate += OnCardUpdate;
            _gameManagerService.Splashed += OnSplashed;
            _playerModel = playerModel;
            _aiManagerService = aiManagerService;
        }

        private void Start()
        {
            ShowNextScreen();
        }

        private void ShowNextScreen()
        {
            ++_currentScreenIndex;
            var screenConfig = _ftueSequence.ScreenSequence[_currentScreenIndex];
            gameObject.SetActive(true);
            _aiManagerService.PauseAI(true);
            _darkBackground.SetParent(_masks[screenConfig.HighlightedPosition]);
            _darkBackground.SetAsFirstSibling();
            _topText.text = screenConfig.TopText;
            _bottomText.text = screenConfig.BottomText;
            _continueText.text = screenConfig.ContinueText;
            _screenTouchBlocker.raycastTarget = screenConfig.HideScreenCondition == HideScreenCondition.None;
        }

        public void OnScreenTap()
        {
            if (_currentScreenIndex + 1 == _ftueSequence.ScreenSequence.Count)
            {
                if (_missFTUE)
                {
                    _playerModel.MissFTUECompleted = true;
                }
                else
                {
                    _playerModel.FTUECompleted = true;
                }
                _gameManagerService.SetTutorialMode(false);
                _aiManagerService.PauseAI(false);
                Destroy(gameObject);
                return;
            }

            var nextScreenConfig = _ftueSequence.ScreenSequence[_currentScreenIndex + 1];
            if (nextScreenConfig.AwakeTrigger != FTUETrigger.None)
            {
                _aiManagerService.PauseAI(false);
                gameObject.SetActive(false); 
                if (_ftueSequence.ScreenSequence[_currentScreenIndex].AwakeTrigger == FTUETrigger.BeforeUnblocked)
                {
                    UpdatePiles?.Invoke();
                }
            }
            else
            {
                ShowNextScreen();
            }
        }

        public void Trigger(FTUETrigger trigger)
        {
            if (_currentScreenIndex + 1 < _ftueSequence.ScreenSequence.Count &&
                _ftueSequence.ScreenSequence[_currentScreenIndex + 1].AwakeTrigger == trigger)
            {
                ShowNextScreen();
                return;
            }

            if (trigger == FTUETrigger.BeforeUnblocked)
            {
                UpdatePiles?.Invoke();
            }
        }

        private void OnCardUpdate(int fromCardPosition, int toCardPosition, int? newNumber)
        {
            if (fromCardPosition != toCardPosition &&
                _ftueSequence.ScreenSequence[_currentScreenIndex].HideScreenCondition ==
                HideScreenCondition.CardMovement)
            {
                OnScreenTap();
            }
        }

        private void OnSplashed(bool wasHuman, int newLeftNumber, int newRightNumber, int points)
        {
            if (_ftueSequence.ScreenSequence[_currentScreenIndex].HideScreenCondition == HideScreenCondition.Splash)
            {
                OnScreenTap();
            }
        }

        private void OnDestroy()
        {
            OnClose?.Invoke();

            _gameManagerService.CardUpdate -= OnCardUpdate;
            _gameManagerService.Splashed -= OnSplashed;
        }
    }
}