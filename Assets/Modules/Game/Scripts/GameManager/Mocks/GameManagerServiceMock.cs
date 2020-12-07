using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Game 
{
    public class GameManagerServiceMock : IGameManagerService, IAIManagerService
    {
        private static readonly int BlockedTimeSec = 2;

        private readonly int _leftStackPosition;
        private readonly int _rightStackPosition;
        private GameStateModel _gameStateModel;
        private INumberGeneratorService _numberGeneratorService;
        private CoroutineProxy _coroutineProxy;
        private IPlayerModel _playerModel;
        private int _unblockDelayId;
        private AI _ai;
        private Mode _gameMode;

        public event Action<int[]> NewGameReceived;
        public event Action<int, int, int?> CardUpdate;
        public event Action<int, bool> GameFinished;
        public event Action<bool, int, int, int> Splashed;
        public event Action<int, int> Unblocked;

        public GameManagerServiceMock(
            INumberGeneratorService numberGeneratorService,
            CoroutineProxy coroutineProxy,
            IPlayerModel playerModel)
        {
            _numberGeneratorService = numberGeneratorService;
            _coroutineProxy = coroutineProxy;
            _playerModel = playerModel;
            _leftStackPosition = BoardView.LeftStackPosition;
            _rightStackPosition = BoardView.RightStackPosition;
        }

        public void Initialize(Mode gameMode)
        {
            _gameMode = gameMode;
            if (_gameMode == Mode.FTUE)
            {
                _numberGeneratorService.GeneratorMode = CardGeneratorMode.FTUE;
            }

            int[] numbers = new int[10];
            for (int i = 0; i < numbers.Length; ++i)
            {
                numbers[i] = _numberGeneratorService.GetNumber();
            }

            Debug.Log("New piles " + numbers[_leftStackPosition] + ":" + numbers[_rightStackPosition]);

            _gameStateModel = new GameStateModel(numbers)
            {
                HumanLifePoints = 100,
                HumanPointsCounter = 0,
                SplashPot = 2
            };

            LeanTween.delayedCall(1f, () =>
            {
                NewGameReceived?.Invoke(_gameStateModel.Numbers);
            });
        }

        public void StartGame()
        {
            if (_gameMode == Mode.AI || _gameMode == Mode.FTUE)
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
            var leftPileNum = _gameStateModel.Numbers[_leftStackPosition];
            var rightPileNum = _gameStateModel.Numbers[_rightStackPosition];

            if (IsACompatibleMove(selectedNum, rightPileNum))
            {
                MoveCard(selectedNum, _rightStackPosition, positionCardSelected, CardUpdate);
            }
            else if (IsACompatibleMove(selectedNum, leftPileNum))
            {
                MoveCard(selectedNum, _leftStackPosition, positionCardSelected, CardUpdate);
            }
            else
            {
                if (positionCardSelected > _rightStackPosition) //is the main player
                {
                    if (SROptions.Current.GodMode)
                    {
                        int randomPilePosition = _leftStackPosition + Random.Range(0, 2);
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

        public void TryDoSplash(bool fromAI = false)
        {
            if (_gameStateModel.Numbers[_leftStackPosition] != _gameStateModel.Numbers[_rightStackPosition] ||
                _gameStateModel.Numbers[_leftStackPosition] == -1 && _gameStateModel.Numbers[_rightStackPosition] == -1) 
                return;

            _gameStateModel.Numbers[_leftStackPosition] = -1;
            _gameStateModel.Numbers[_rightStackPosition] = -1;

            var newLeftNumber = _numberGeneratorService.GetNumber();
            var newRightNumber = _numberGeneratorService.GetNumber();

            _unblockDelayId = LeanTween.delayedCall(2f, () =>
            {
                Debug.Log("Update piles " + newLeftNumber + ":" + newRightNumber);
                _gameStateModel.Numbers[_leftStackPosition] = newLeftNumber;
                _gameStateModel.Numbers[_rightStackPosition] = newRightNumber;
                _gameStateModel.SplashPot = 2;

                Unblocked?.Invoke(_gameStateModel.Numbers[_leftStackPosition],
                    _gameStateModel.Numbers[_rightStackPosition]);

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
                _gameStateModel.Numbers[_leftStackPosition] = _numberGeneratorService.GetNumber();
                _gameStateModel.Numbers[_rightStackPosition] = _numberGeneratorService.GetNumber();
                _gameStateModel.SplashPot += 2;
                Unblocked?.Invoke(_gameStateModel.Numbers[_leftStackPosition],
                    _gameStateModel.Numbers[_rightStackPosition]);
                Debug.Log("Unblocked");
                Debug.Log("Update piles " + _gameStateModel.Numbers[_leftStackPosition] + ":" +
                          _gameStateModel.Numbers[_rightStackPosition]);

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
            _ai?.Stop();
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
            if (positionCardSelected > _rightStackPosition)
            {
                _gameStateModel.HumanPointsCounter++;
            }
            else
            {
                _gameStateModel.HumanLifePoints--;
            }

            Debug.Log(log + " -> " +
                      _gameStateModel.Numbers[_leftStackPosition] + " " +
                      _gameStateModel.Numbers[_rightStackPosition]);

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
            _ai?.Stop();
            StopDelayedUnblock();
            bool isNewHumanRecord = _gameStateModel.HumanPointsCounter > _playerModel.HumanRecord;
            if (isNewHumanRecord)
            {
                _playerModel.HumanRecord = _gameStateModel.HumanPointsCounter;
            }

            GameFinished?.Invoke(_gameStateModel.HumanPointsCounter, isNewHumanRecord);
        }

        public static bool IsACompatibleMove(int originNum, int destinationNum)
        {
            return originNum == destinationNum + 1
                   || originNum == destinationNum - 1
                   || originNum == NumberGeneratorServiceMock.MaxRange
                   && destinationNum == NumberGeneratorServiceMock.MinRange
                   || originNum == NumberGeneratorServiceMock.MinRange
                   && destinationNum == NumberGeneratorServiceMock.MaxRange;
        }

        private bool IsGameBlocked()
        {
            int firstHumanPosition = _rightStackPosition + 1;
            for (int i = 0; i < _leftStackPosition; ++i)
            {
                if (IsACompatibleMove(_gameStateModel.Numbers[i], _gameStateModel.Numbers[_leftStackPosition]) ||
                    IsACompatibleMove(_gameStateModel.Numbers[i], _gameStateModel.Numbers[_rightStackPosition]) ||
                    IsACompatibleMove(_gameStateModel.Numbers[firstHumanPosition + i],
                        _gameStateModel.Numbers[_leftStackPosition]) ||
                    IsACompatibleMove(_gameStateModel.Numbers[firstHumanPosition + i],
                        _gameStateModel.Numbers[_rightStackPosition]) ||
                    _gameStateModel.Numbers[_leftStackPosition] == _gameStateModel.Numbers[_rightStackPosition]
                )
                {
                    return false;
                }
            }

            return true;
        }
    }
}