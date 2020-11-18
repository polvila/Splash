using System.Collections.Generic;
using Core.ScreenManagement;
using TMPro;
using UnityEngine;
using Zenject;

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
    EnemyPlayed,
    PlayerPlayed,
    SplashReady,
    SplashUnblocked
}

public class FTUEPopupView : PopupScreenView
{
    [SerializeField] private FTUESequence _ftueSequence;
    [SerializeField] private Transform _darkBackground;

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
    
    private void Awake()
    {
        _masks = new Dictionary<HighlightedPosition, Transform>
        {
            {HighlightedPosition.Enemy, _enemyMask},
            {HighlightedPosition.Middle, _middleMask},
            {HighlightedPosition.Player, _playerMask},
            {HighlightedPosition.HealthBar, _healthBarMask},
            {HighlightedPosition.OnePlayerCard, _onePlayerCardMask},
            {HighlightedPosition.OnePlayerToMiddle, _playerToMiddleCardMask},
            {HighlightedPosition.Points, _pointsMask},
        };
    }

    private void Start()
    {
        ShowNextScreen();
    }

    public override void SetParams(object paramsObject)
    {
        
    }

    private void ShowNextScreen()
    {
        ++_currentScreenIndex;
        var screenConfig = _ftueSequence.ScreenSequence[_currentScreenIndex];
        _darkBackground.SetParent(_masks[screenConfig.HighlightedPosition]);
        _topText.text = screenConfig.TopText;
        _bottomText.text = screenConfig.BottomText;
        _continueText.text = screenConfig.ContinueText;
    }

    public void OnScreenTap()
    {
        var currentScreenConfig = _ftueSequence.ScreenSequence[_currentScreenIndex];
        if (currentScreenConfig.ActionRequested)
        {
            gameObject.SetActive(false);
        }
        else
        {
            ShowNextScreen();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}