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
        FullScreen,
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
        EnemyPlay,
        EnemyPlayed,
        PlayerValidPlay,
        PlayerPlayed,
        SplashReady,
        Splash,
        Unblock,
        Unblocked,
        PlayerInvalidPlay,
    }

    public class FTUEView : MonoBehaviour
    {
        [Header("FTUE")] 
        [SerializeField] private FTUESequence _ftueSequence;
        [SerializeField] private FTUESequence _missFtueSequence;
        [SerializeField] private Transform _darkBackground;
        [SerializeField] private Button _fullScreenButton;

        [Header("FTUE Masks")] 
        [SerializeField] private Transform _enemyMask;
        [SerializeField] private Transform _middleMask;
        [SerializeField] private Transform _playerMask;
        [SerializeField] private Transform _healthBarMask;
        [SerializeField] private Transform _onePlayerCardMask;
        [SerializeField] private Transform _playerToMiddleCardMask;
        [SerializeField] private Transform _pointsMask;

        [Header("FTUE Texts")] 
        [SerializeField] private TMP_Text _topText;
        [SerializeField] private TMP_Text _bottomText;
        [SerializeField] private TMP_Text _continueText;

        private Dictionary<HighlightedPosition, Transform> _masks;
        private int _currentScreenIndex = -1;
        private int _nextMovementIndex = -1;
        private int[] _nextMovements;
        private bool _missFtueSelected;

        public Action UnblockAction { get; set; }
        
        [Inject] private IAIManagerService _aiManagerService;

        public event Action<bool> SequenceEnded;

        public int NextPositionToMove => _nextMovements[_nextMovementIndex];

        private void Awake()
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
            _nextMovements = new[] {9, 6, 6, 0, 8, -1, 6, -1, -1};
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
            var showDarkBackground = screenConfig.HighlightedPosition != HighlightedPosition.FullScreen;
            _darkBackground.gameObject.SetActive(showDarkBackground);
            if (showDarkBackground)
            {
                _darkBackground.SetParent(_masks[screenConfig.HighlightedPosition]);
                _darkBackground.SetAsFirstSibling();
            }

            _topText.text = screenConfig.TopText;
            _bottomText.text = screenConfig.BottomText;
            _continueText.text = screenConfig.ContinueText;
            _fullScreenButton.gameObject.SetActive(screenConfig.InputBlocked ||
                                                   screenConfig.HideTrigger == FTUETrigger.None);
            _fullScreenButton.interactable = !screenConfig.InputBlocked;
        }

        public void OnScreenTap()
        {
            if (_currentScreenIndex + 1 == _ftueSequence.ScreenSequence.Count)
            {
                SequenceEnded?.Invoke(_missFtueSelected);
                if (!_missFtueSelected)
                {
                    _ftueSequence = _missFtueSequence;
                    _currentScreenIndex = -1;
                    _missFtueSelected = true;
                }
                gameObject.SetActive(false);
                _aiManagerService.PauseAI(false);
                return;
            }

            var nextScreenConfig = _ftueSequence.ScreenSequence[_currentScreenIndex + 1];
            if (nextScreenConfig.AwakeTrigger != FTUETrigger.None)
            {
                gameObject.SetActive(false);
                _aiManagerService.PauseAI(false);
            }
            else
            {
                ShowNextScreen();
            }

            //if the current screen was waiting for an unblock, trigger the pending animation after close the screen
            if (_ftueSequence.ScreenSequence[_currentScreenIndex].AwakeTrigger == FTUETrigger.Unblock)
            {
                UnblockAction?.Invoke();
            }
        }

        public void Trigger(FTUETrigger trigger)
        {
            if (trigger == FTUETrigger.EnemyPlay ||
                trigger == FTUETrigger.PlayerValidPlay ||
                trigger == FTUETrigger.Unblock)
            {
                ++_nextMovementIndex;
            }

            if (_currentScreenIndex + 1 < _ftueSequence.ScreenSequence.Count &&
                _ftueSequence.ScreenSequence[_currentScreenIndex + 1].AwakeTrigger == trigger)
            {
                ShowNextScreen();
                return;
            }
            
            //Trigger unblock animation as normal if any screen is waiting for it
            if (trigger == FTUETrigger.Unblock)
            {
                UnblockAction?.Invoke();
            }
            
            if (gameObject.activeSelf && _ftueSequence.ScreenSequence[_currentScreenIndex].HideTrigger == trigger)
            {
                OnScreenTap();
            }
        }

        public bool GetNextPositionToMove(out int position)
        {
            if (_nextMovementIndex < _nextMovements.Length)
            {
                position = _nextMovements[_nextMovementIndex];
                return true;
            }

            position = -1;
            return false;
        }
    }
}