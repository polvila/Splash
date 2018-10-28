using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class GameManagerServiceMock : IGameManagerService
{
    private const int LeftPilePosition = 4;
    private const int RightPilePosition = 5;
    private GameStateModel _gameStateModel;
    private INumberGeneratorService _numberGeneratorService;
    private CoroutineProxy _coroutineProxy;

    public event Action<int[]> NewBoardReceived;
    public event Action<int, int, int?> CardUpdate;

    [Inject]
    void Init(INumberGeneratorService numberGeneratorService, CoroutineProxy coroutineProxy)
    {
        _numberGeneratorService = numberGeneratorService;
        _coroutineProxy = coroutineProxy;
    }

    public void Initialize()
    {
        int[] numbers = new int[10];
        for (int i = 0; i < numbers.Length; ++i)
        {
            numbers[i] = _numberGeneratorService.GetNumber();
        }
        _gameStateModel = new GameStateModel(numbers);
        //_gameStateModel.State.PropertyChanged += OnStateChanged;
        _gameStateModel.EnemyCounter = 0;
        _gameStateModel.HumanCounter = 0;
        
//        _gameStateModel.Timer.TimerEnded += () =>
//            _gameStateModel.State.Property = GameState.Finished;
//        _gameStateModel.Timer.Init(_initialTimerSeconds);

        _coroutineProxy.StartCoroutine(DelayedCallback(1, () => NewBoardReceived?.Invoke(_gameStateModel.Numbers)));
    }

    private IEnumerator DelayedCallback(int seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }

    public void Start(Mode mode)
    {
        if (mode == Mode.IA)
        {
            var ia = new IA(this, _coroutineProxy);
        }
        
        //_gameStateModel.Timer.StartTimer();
    }

    public void PlayThisCard(int positionCardSelected)
    {
        var selectedNum = _gameStateModel.Numbers[positionCardSelected];
        var destinationNum1 = _gameStateModel.Numbers[LeftPilePosition];
        var destinationNum2 = _gameStateModel.Numbers[RightPilePosition];

        if (IsACompatibleMove(selectedNum, destinationNum1))
        {
            MoveCardOnGameState(selectedNum, LeftPilePosition, positionCardSelected, CardUpdate);
        }
        else if (IsACompatibleMove(selectedNum, destinationNum2))
        {
            MoveCardOnGameState(selectedNum, RightPilePosition, positionCardSelected, CardUpdate);
        }
        else
        {
            if (SROptions.Current.GodMode)
            {
                int randomPilePosition = selectedNum % 2 + LeftPilePosition;
                MoveCardOnGameState(selectedNum, randomPilePosition, positionCardSelected, CardUpdate);
            }
            else
            {
                CardUpdate?.Invoke(positionCardSelected, positionCardSelected, null);
            }
        }
    }

    private void MoveCardOnGameState(int selectedNum, int pilePosition, int positionCardSelected, 
        Action<int, int, int?> destinationCallback)
    {
        int newNumber = _numberGeneratorService.GetNumber();
        _gameStateModel.Numbers[pilePosition] = selectedNum;
        _gameStateModel.Numbers[positionCardSelected] = newNumber;
        if (positionCardSelected > RightPilePosition)
        {
            _gameStateModel.HumanCounter++;
        }
        else
        {
            _gameStateModel.EnemyCounter++;
        }
        
        destinationCallback?.Invoke(positionCardSelected, pilePosition, newNumber); 
    }
    
    private bool IsACompatibleMove(int originNum, int destinationNum)
    {
        return originNum == destinationNum + 1 
               || originNum == destinationNum - 1
               || originNum == _numberGeneratorService.GetMaxRange 
               && destinationNum == _numberGeneratorService.GetMinRange
               || originNum == _numberGeneratorService.GetMinRange 
               && destinationNum == _numberGeneratorService.GetMaxRange;
    }
    
    void OnStateChanged(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Updated:
                _gameStateModel.State = GameState.Idle;
                break;
            case GameState.Finished:
                //_gameStateModel.EnemyPlayerView.Playable = false;
                //_gameStateModel.HumanPlayerView.Playable = false;
                _gameStateModel.Result = GetGameResult();
                break;
        }
    }

    private GameResult GetGameResult()
    {
        if (_gameStateModel.EnemyCounter < _gameStateModel.HumanCounter)
        {
            return GameResult.HumanWins;
        }

        if (_gameStateModel.EnemyCounter > _gameStateModel.HumanCounter)
        {
            return GameResult.EnemyWins;
        }

        return GameResult.Draw;
    }

    private void OnDestroy()
    {
        //_gameStateModel.State.PropertyChanged -= OnStateChanged;
    }
}