using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class GameManagerServiceMock : IGameManagerService, IAIManagerService
{
    private static readonly int BlockedTimeSec = 2;
    private static readonly string HumanRecordKey = "HumanRecord";

    public const int LeftPilePosition = 4;
    public const int RightPilePosition = 5;
    private GameStateModel _gameStateModel;
    private INumberGeneratorService _numberGeneratorService;
    private CoroutineProxy _coroutineProxy;
    private int _unblockDelayId;
    private AI _ai;
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
        
        Debug.Log("New piles " + numbers[LeftPilePosition] + ":" + numbers[RightPilePosition]);

        _gameStateModel = new GameStateModel(numbers)
        {
            HumanLifePoints = 100, 
            HumanPointsCounter = 0, 
            SplashPot = 2
        };

        LeanTween.delayedCall(1f, () => NewGameReceived?.Invoke(_gameStateModel.Numbers));
    }

    public void StartGame(Mode mode)
    {
        if (mode == Mode.AI)
        {
            _ai = new AI(this, _coroutineProxy);
            _ai.Play();
        }

        if (IsGameBlocked())
        {
            Debug.Log("Unblock by Start Game");
            UnblockIn(BlockedTimeSec);
        }
    }

    public void PlayThisCard(int positionCardSelected)
    {
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
                else //miss
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

    public void TryDoSplash(bool fromAI = false)
    {
        if (_gameStateModel.Numbers[LeftPilePosition] != _gameStateModel.Numbers[RightPilePosition] ||
            _gameStateModel.Numbers[LeftPilePosition] == -1 &&
            _gameStateModel.Numbers[RightPilePosition] == -1) return;

        _gameStateModel.Numbers[LeftPilePosition] = -1;
        _gameStateModel.Numbers[RightPilePosition] = -1;

        var newLeftNumber = _numberGeneratorService.GetNumber();
        var newRightNumber = _numberGeneratorService.GetNumber();

        _unblockDelayId = LeanTween.delayedCall(2f, () =>
        {
            Debug.Log("Update piles " + newLeftNumber + ":" + newRightNumber);
            _gameStateModel.Numbers[LeftPilePosition] = newLeftNumber;
            _gameStateModel.Numbers[RightPilePosition] = newRightNumber;
            _gameStateModel.SplashPot = 2;

            Unblocked?.Invoke(_gameStateModel.Numbers[LeftPilePosition], _gameStateModel.Numbers[RightPilePosition]);

            if (IsGameBlocked())
            {
                Debug.Log("Unblock by splash " + (fromAI ? " from AI " : "from human"));
                UnblockIn(BlockedTimeSec);
            }
        }).id;

        if (fromAI)
        {
            _gameStateModel.HumanLifePoints -= _gameStateModel.SplashPot;
        }
        else
        {
            _gameStateModel.HumanPointsCounter += _gameStateModel.SplashPot;
        }
        
        Splashed?.Invoke(!fromAI, newLeftNumber, newRightNumber, _gameStateModel.SplashPot);

        Debug.Log((fromAI ? "AI Splash!" : "Splash!") + " with " + _gameStateModel.SplashPot + " points");

        if (_gameStateModel.HumanLifePoints <= 0)
        {
            GameOver();
        }
    }

    private void UnblockIn(int seconds)
    {
        StopDelayedUnblock();
        _unblockDelayId = LeanTween.delayedCall(seconds, () =>
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
        }).id;
    }

    public void HumanSplash()
    {
        TryDoSplash();
    }

    public void Exit()
    {
        _ai.Stop();
        StopDelayedUnblock();
    }

    public void PauseAI(bool active)
    {
        _ai?.Pause(active);
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
        }
        else if (IsGameBlocked())
        {
            Debug.Log("Unblock by move card");
            UnblockIn(BlockedTimeSec);
        }
    }

    private void StopDelayedUnblock()
    {
        if (LeanTween.isTweening(_unblockDelayId))
        {
            LeanTween.cancel(_unblockDelayId);
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
        _ai.Stop();
        StopDelayedUnblock();
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