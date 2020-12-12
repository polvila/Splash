using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Game
{
    public class AI
    {
        private GameManagerServiceMock _gameManagerService;
        private CoroutineProxy _coroutineProxy;
        private Coroutine _currentCoroutine;
        private int[] _positionsToCheck;
        private int _positionsToCheckIndex;
        private bool _stacksUpdated;
        private bool _splashed;
        private bool _paused;

        public AI(GameManagerServiceMock gameManagerService, CoroutineProxy coroutineProxy)
        {
            _gameManagerService = gameManagerService;
            _coroutineProxy = coroutineProxy;
            _positionsToCheck = Enumerable.Range(0, 4).OrderBy(elem => Guid.NewGuid()).ToArray();
            _gameManagerService.CardUpdate += OnCardUpdate;
            _gameManagerService.Unblocked += OnUnblocked;
            _gameManagerService.Splashed += OnSplashed;
        }

        public void Play()
        {
            _stacksUpdated = true;
            _currentCoroutine = _coroutineProxy.StartCoroutine(Update());
        }

        IEnumerator Update()
        {
            while (true)
            {
                for (; _positionsToCheckIndex < _positionsToCheck.Length; ++_positionsToCheckIndex)
                {
                    do
                    {
                        _stacksUpdated = false;
                        var time = Random.Range(0.6f, 0.8f);
                        yield return new WaitForSeconds(time);
                        yield return new WaitUntil(() => !_paused);
                    } while (_stacksUpdated);
                    _gameManagerService.TryDoSplash(true);
                    
                    if (_splashed)
                    {
                        _splashed = false;
                        break;
                    }
                    
                    _gameManagerService.PlayThisCard(_positionsToCheck[_positionsToCheckIndex]);
                }

                _positionsToCheck = _positionsToCheck.OrderBy(elem => Guid.NewGuid()).ToArray();
                _positionsToCheckIndex = 0;
            }
        }

        public void Stop()
        {
            if (_currentCoroutine == null) return;
            _coroutineProxy.StopCoroutine(_currentCoroutine);
        }

        public void Pause(bool active)
        {
            _paused = active;
        }

        private void OnCardUpdate(int fromCardPosition, int toCardPosition, int? newNumber)
        {
            if (fromCardPosition == toCardPosition || newNumber == null)
            {
                return;
            }

            _stacksUpdated = true;

            if (fromCardPosition < BoardView.LeftStackPosition)
            {
                --_positionsToCheckIndex;
            }
        }

        private void OnUnblocked(int newLeftNumber, int newRightNumber)
        {
            _stacksUpdated = true;
        }
        
        private void OnSplashed(bool wasHuman, int newLeftNumber, int newRightNumber, int points)
        {
            _splashed = true;
        }
    }
}