using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class GameManagerServiceMock : IGameManagerService
{
    private static readonly int GameTimeSec = 120;
    private static readonly int BlockedTimeSec = 4;
    
    private const int LeftPilePosition = 4;
    private const int RightPilePosition = 5;
    private GameStateModel _gameStateModel;
    private INumberGeneratorService _numberGeneratorService;
    private CoroutineProxy _coroutineProxy;
    private IDisposable _gameTime;
    private IDisposable _blockedTime;
    private IA _ia;
    
    public event Action<int[], int> NewGameReceived;
    public event Action<int, int, int?> CardUpdate;
    public event Action<GameResult> GameFinished;
    public event Action<bool, int, int> Splashed;
    public event Action<int, int> Unblocked;

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
        
        _coroutineProxy.StartCoroutine(DelayedCallback(1, () => NewGameReceived?.Invoke(_gameStateModel.Numbers, GameTimeSec)));
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
            _ia.Start();
        }
        
        _gameTime?.Dispose();
        _gameTime = Observable.Timer(TimeSpan.FromSeconds(GameTimeSec))
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
                int randomPilePosition = LeftPilePosition + Random.Range(0, 2);
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
        if (_gameStateModel.Numbers[LeftPilePosition] != _gameStateModel.Numbers[RightPilePosition] && fromIA) return;
        
        _gameStateModel.Numbers[LeftPilePosition] = _numberGeneratorService.GetNumber();
        _gameStateModel.Numbers[RightPilePosition] = _numberGeneratorService.GetNumber();
        
        if (fromIA)
        {
            _gameStateModel.EnemyCounter += 10;
            Splashed?.Invoke(false, _gameStateModel.Numbers[LeftPilePosition], _gameStateModel.Numbers[RightPilePosition]);
        }
        else
        {
            _gameStateModel.HumanCounter += 10;
            Splashed?.Invoke(true, _gameStateModel.Numbers[LeftPilePosition], _gameStateModel.Numbers[RightPilePosition]);
        }
        Debug.Log(fromIA ? "IA Splash!"  : "Splash!");
        Debug.Log("Update piles " + _gameStateModel.Numbers[LeftPilePosition] + ":" + _gameStateModel.Numbers[RightPilePosition]);

        if (IsGameBlocked())
        {
            Debug.Log("Unblock by splash " + (fromIA ? " from IA " : "from human"));
            UnblockIn(BlockedTimeSec);
        }
    }

    private void UnblockIn(int seconds)
    {
        _blockedTime?.Dispose();
        _blockedTime = Observable.Timer(TimeSpan.FromSeconds(seconds))
            .Subscribe(timeSpan =>
            {
                _gameStateModel.Numbers[LeftPilePosition] = _numberGeneratorService.GetNumber();
                _gameStateModel.Numbers[RightPilePosition] = _numberGeneratorService.GetNumber();
                Unblocked?.Invoke(_gameStateModel.Numbers[LeftPilePosition],
                    _gameStateModel.Numbers[RightPilePosition]);
                Debug.Log("Unblocked");
                Debug.Log("Update piles " + _gameStateModel.Numbers[LeftPilePosition] + ":" + _gameStateModel.Numbers[RightPilePosition]);
                
                if (IsGameBlocked())
                {
                    Debug.Log("Unblock by unblock");
                    UnblockIn(BlockedTimeSec);
                }
            });
    }

    public void HumanSplash()
    {
        Splash();
    }

    public void Exit()
    {
        _gameTime?.Dispose();
        _blockedTime?.Dispose();
    }

    private void MoveCardOnGameState(int selectedNum, int pilePosition, int positionCardSelected, 
        Action<int, int, int?> destinationCallback)
    {
        string log = "Move card " + selectedNum + ":" + positionCardSelected +
                     " to " + _gameStateModel.Numbers[pilePosition] + ":" + pilePosition;
        
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
        
        Debug.Log(log + " -> " +
                  _gameStateModel.Numbers[LeftPilePosition] + " " + 
                  _gameStateModel.Numbers[RightPilePosition]);

        destinationCallback?.Invoke(positionCardSelected, pilePosition, newNumber); 
        
        if (IsGameBlocked())
        {
            Debug.Log("Unblock by move card");
            UnblockIn(BlockedTimeSec);
        }
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
        _blockedTime?.Dispose();
        _gameTime.Dispose();
        
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

    private bool IsGameBlocked()
    {
        int firstHumanPosition = RightPilePosition + 1;
        for (int i = 0; i < LeftPilePosition; ++i)
        {
            if (IsACompatibleMove(_gameStateModel.Numbers[i], _gameStateModel.Numbers[LeftPilePosition]) || 
                IsACompatibleMove(_gameStateModel.Numbers[i], _gameStateModel.Numbers[RightPilePosition]) ||
                IsACompatibleMove(_gameStateModel.Numbers[firstHumanPosition + i], 
                    _gameStateModel.Numbers[LeftPilePosition]) ||
                IsACompatibleMove(_gameStateModel.Numbers[firstHumanPosition + i], 
                    _gameStateModel.Numbers[RightPilePosition]) ||
                    _gameStateModel.Numbers[LeftPilePosition] == _gameStateModel.Numbers[RightPilePosition]
                )
            {
                return false;
            }
        }

        return true;
    }
}