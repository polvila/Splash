using System.Collections;
using UnityEngine;

public class IA
{
    private IGameManagerService _gameManagerService;
    private CoroutineProxy _coroutineProxy;
    private Coroutine _currentCoroutine;
    
    public IA(IGameManagerService gameManagerService, CoroutineProxy coroutineProxy)
    {
        _gameManagerService = gameManagerService;
        _coroutineProxy = coroutineProxy;
        _currentCoroutine = _coroutineProxy.StartCoroutine(Update());
    }

    IEnumerator Update()
    {
        while (true)
        {
            for (int i = 0; i < 4; ++i)
            {
                yield return new WaitForSeconds(1);
                _gameManagerService.PlayThisCard(i);
            }
        }
    }

    public void Stop()
    {
        _coroutineProxy.StopCoroutine(_currentCoroutine);
    }
}
