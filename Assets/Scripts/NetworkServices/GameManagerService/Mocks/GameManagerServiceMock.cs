using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Zenject;

public class GameManagerServiceMock : IGameManagerService
{
    private static readonly int UpdateTimeMs = 120;
    private const int LeftPilePosition = 4;
    private const int RightPilePosition = 5;
    private GameStateModel _gameStateModel;
    private INumberGeneratorService _numberGeneratorService;
    private CoroutineProxy _coroutineProxy;
    private IDisposable _interval;
    private IA _ia;
    
    public event Action<int[], int> NewGameReceived;
    public event Action<int, int, int?> CardUpdate;
    public event Action<GameResult> GameFinished;
    public event Action<bool> Splashed;

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
        _gameStateModel.EnemyCounter = 0;
        _gameStateModel.HumanCounter = 0;
        
        _coroutineProxy.StartCoroutine(DelayedCallback(1, () => NewGameReceived?.Invoke(_gameStateModel.Numbers, UpdateTimeMs)));
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
            _ia = new IA(this, _coroutineProxy);
        }
        
        _interval = Observable.Interval(TimeSpan.FromSeconds(UpdateTimeMs))
            .Subscribe(timeSpan =>
            {
                _ia.Stop();
                GameFinished?.Invoke(GetGameResult());
            });
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

    public void Splash(bool fromIA = false)
    {
        if (_gameStateModel.Numbers[LeftPilePosition] != _gameStateModel.Numbers[RightPilePosition]) return;
        
        if (fromIA)
        {
            _gameStateModel.EnemyCounter += 10;
            Splashed?.Invoke(false);
        }
        else
        {
            _gameStateModel.HumanCounter += 10;
            Splashed?.Invoke(true);
        }
    }

    public void HumanSplash()
    {
        Splash();
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
    

    private GameResult GetGameResult()
    {
        _interval.Dispose();
        
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
}