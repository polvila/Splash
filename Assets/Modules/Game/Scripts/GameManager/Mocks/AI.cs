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
        private bool _checkSplash;
        private bool _paused;

        public AI(GameManagerServiceMock gameManagerService, CoroutineProxy coroutineProxy)
        {
            _gameManagerService = gameManagerService;
            _coroutineProxy = coroutineProxy;
            _positionsToCheck = Enumerable.Range(0, 4).OrderBy(elem => Guid.NewGuid()).ToArray();
            _gameManagerService.CardUpdate += OnCardUpdate;
            _gameManagerService.Unblocked += OnUnblocked;
        }

        public void Play()
        {
            _checkSplash = true;
            _currentCoroutine = _coroutineProxy.StartCoroutine(Update());
        }

        IEnumerator Update()
        {
            while (true)
            {
                for (; _positionsToCheckIndex < _positionsToCheck.Length; ++_positionsToCheckIndex)
                {
                    if (_checkSplash)
                    {
                        _checkSplash = false;
                        yield return new WaitForSeconds(Random.Range(0.4f, 0.6f));
                        yield return new WaitUntil(() => !_paused);
                        _gameManagerService.TryDoSplash(true);
                    }

                    yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
                    yield return new WaitUntil(() => !_paused);
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

            _checkSplash = true;

            if (fromCardPosition < BoardView.LeftStackPosition)
            {
                --_positionsToCheckIndex;
            }
        }

        private void OnUnblocked(int newLeftNumber, int newRightNumber)
        {
            _checkSplash = true;
        }
    }
}