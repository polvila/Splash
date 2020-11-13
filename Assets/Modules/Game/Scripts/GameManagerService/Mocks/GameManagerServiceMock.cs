using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class GameManagerServiceMock : IGameManagerService
{
    private static readonly int BlockedTimeSec = 4;
    private static readonly string HumanRecordKey = "HumanRecord";

    private const int LeftPilePosition = 4;
    private const int RightPilePosition = 5;
    private GameStateModel _gameStateModel;
    private INumberGeneratorService _numberGeneratorService;
    private CoroutineProxy _coroutineProxy;
    private IDisposable _blockedTime;
    private IA _ia;
    private int _humanRecord = -1;

    public event Action<int[]> NewGameReceived;
    public event Action<int, int, int?> CardUpdate;
    public event Action<int, bool> GameFinished;
    public event Action<bool, int, int, int> Splashed;
    public event Action<int, int> Unblocked;

    public int HumanRecord
    {
        get
        {
            if (_humanRecord == -1)
            {
                _humanRecord = PlayerPrefs.GetInt(HumanRecordKey);
            }

            return _humanRecord;
        }
        private set
        {
            _humanRecord = value;
            PlayerPrefs.SetInt(HumanRecordKey, _humanRecord);
            PlayerPrefs.Save();
        }
    }

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
        _gameStateModel.HumanLifePoints = 100;
        _gameStateModel.HumanPointsCounter = 0;
        _gameStateModel.SplashPot = 2;

        _coroutineProxy.StartCoroutine(
            DelayedCallback(1, () => NewGameReceived?.Invoke(_gameStateModel.Numbers)));
    }

    private IEnumerator DelayedCallback(int seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }

    public void StartGame(Mode mode)
    {
        _gameStateModel.GamePaused = false;
        if (mode == Mode.IA)
        {
            _ia = new IA(this, _coroutineProxy);
            _ia.StartPlaying();
        }

        if (IsGameBlocked())
        {
            Debug.Log("Unblock by Start Game");
            UnblockIn(BlockedTimeSec);
        }
    }

    public void PlayThisCard(int positionCardSelected)
    {
        if (_gameStateModel.GamePaused) return;

        var selectedNum = _gameStateModel.Numbers[positionCardSelected];
        var leftPileNum = _gameStateModel.Numbers[LeftPilePosition];
        var rightPileNum = _gameStateModel.Numbers[RightPilePosition];

        if (IsACompatibleMove(selectedNum, rightPileNum))
        {
            MoveCard(selectedNum, RightPilePosition, positionCardSelected, CardUpdate);
        }
        else if (IsACompatibleMove(selectedNum, leftPileNum))
        {
            MoveCard(selectedNum, LeftPilePosition, positionCardSelected, CardUpdate);
        }
        else
        {
            if (positionCardSelected > RightPilePosition) //is the main player
            {
                if (SROptions.Current.GodMode)
                {
                    int randomPilePosition = LeftPilePosition + Random.Range(0, 2);
                    MoveCard(selectedNum, randomPilePosition, positionCardSelected, CardUpdate);
                }
                else
                {
                    --_gameStateModel.HumanLifePoints;
                    CardUpdate?.Invoke(positionCardSelected, positionCardSelected, null);
                    
                    if (_gameStateModel.HumanLifePoints <= 0)
                    {
                        GameOver();
                    }
                }
            }
        }
    }

    public void TryDoSplash(bool fromIA = false)
    {
        if (_gameStateModel.GamePaused ||
            _gameStateModel.Numbers[LeftPilePosition] != _gameStateModel.Numbers[RightPilePosition] ||
            _gameStateModel.Numbers[LeftPilePosition] == -1 &&
            _gameStateModel.Numbers[RightPilePosition] == -1) return;

        _gameStateModel.Numbers[LeftPilePosition] = -1;
        _gameStateModel.Numbers[RightPilePosition] = -1;

        var newLeftNumber = _numberGeneratorService.GetNumber();
        var newRightNumber = _numberGeneratorService.GetNumber();

        LeanTween.delayedCall(2f, () =>
        {
            Debug.Log("Update piles " + newLeftNumber + ":" + newRightNumber);
            _gameStateModel.Numbers[LeftPilePosition] = newLeftNumber;
            _gameStateModel.Numbers[RightPilePosition] = newRightNumber;
            Unblocked?.Invoke(_gameStateModel.Numbers[LeftPilePosition], _gameStateModel.Numbers[RightPilePosition]);

            if (IsGameBlocked())
            {
                Debug.Log("Unblock by splash " + (fromIA ? " from IA " : "from human"));
                UnblockIn(BlockedTimeSec);
            }
        });

        if (fromIA)
        {
            _gameStateModel.HumanLifePoints -= _gameStateModel.SplashPot;
            Splashed?.Invoke(false, newLeftNumber, newRightNumber, _gameStateModel.SplashPot);
        }
        else
        {
            _gameStateModel.HumanPointsCounter += _gameStateModel.SplashPot;
            Splashed?.Invoke(true, newLeftNumber, newRightNumber, _gameStateModel.SplashPot);
        }

        Debug.Log((fromIA ? "IA Splash!" : "Splash!") + " with " + _gameStateModel.SplashPot + "points");

        _gameStateModel.SplashPot = 2;

        if (_gameStateModel.HumanLifePoints <= 0)
        {
            GameOver();
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
                _gameStateModel.SplashPot += 2;
                Unblocked?.Invoke(_gameStateModel.Numbers[LeftPilePosition],
                    _gameStateModel.Numbers[RightPilePosition]);
                Debug.Log("Unblocked");
                Debug.Log("Update piles " + _gameStateModel.Numbers[LeftPilePosition] + ":" +
                          _gameStateModel.Numbers[RightPilePosition]);

                if (IsGameBlocked())
                {
                    Debug.Log("Unblock by unblock");
                    UnblockIn(BlockedTimeSec);
                }
            });
    }

    public void HumanSplash()
    {
        TryDoSplash();
    }

    public void Exit()
    {
        _blockedTime?.Dispose();
    }

    private void MoveCard(int selectedNum, int pilePosition, int positionCardSelected,
        Action<int, int, int?> destinationCallback)
    {
        string log = "Move card " + selectedNum + ":" + positionCardSelected +
                     " to " + _gameStateModel.Numbers[pilePosition] + ":" + pilePosition;

        int newNumber = _numberGeneratorService.GetNumber();
        _gameStateModel.Numbers[pilePosition] = selectedNum;
        _gameStateModel.Numbers[positionCardSelected] = newNumber;
        ++_gameStateModel.SplashPot;
        if (positionCardSelected > RightPilePosition)
        {
            _gameStateModel.HumanPointsCounter++;
        }
        else
        {
            _gameStateModel.HumanLifePoints--;
        }

        Debug.Log(log + " -> " +
                  _gameStateModel.Numbers[LeftPilePosition] + " " +
                  _gameStateModel.Numbers[RightPilePosition]);

        destinationCallback?.Invoke(positionCardSelected, pilePosition, newNumber);

        if (_gameStateModel.HumanLifePoints <= 0)
        {
            GameOver();
            return;
        }

        if (IsGameBlocked())
        {
            Debug.Log("Unblock by move card");
            UnblockIn(BlockedTimeSec);
        }
    }

    private void GameOver()
    {
        _ia.Stop();
        _gameStateModel.GamePaused = true;
        _blockedTime?.Dispose();
        bool isNewHumanRecord = _gameStateModel.HumanPointsCounter > HumanRecord;
        if (isNewHumanRecord)
        {
            HumanRecord = _gameStateModel.HumanPointsCounter;
        }

        GameFinished?.Invoke(_gameStateModel.HumanPointsCounter, isNewHumanRecord);
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