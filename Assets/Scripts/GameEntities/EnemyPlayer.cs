using System.Collections;
using UnityEngine;
using Zenject;

public class EnemyPlayer : CardsZone
{
    private const float MaxSeconds = 2.0f;
    private const float MinSeconds = 0.5f;
    
    private CoroutineProxy _coroutineProxy;
    private Coroutine _coroutine;
    
    [Inject]
    void Init(CoroutineProxy coroutineProxy)
    {
        _coroutineProxy = coroutineProxy;
    }
    
    public void UpdateIA()
    {
        Debug.Log("UpdateIA");
        if (_coroutine != null)
        {       
            _coroutineProxy.StopCoroutine(_coroutine);
        }
        _coroutine = _coroutineProxy.StartCoroutine(TryACardAfterSomeRandomTime());
    }

    IEnumerator TryACardAfterSomeRandomTime()
    {
        //Should be great a random foreach
        foreach (var card in Cards)
        {
            Debug.Log("IA trying " + card.Num);
            
            yield return new WaitForSeconds(Random.Range(MinSeconds, MaxSeconds+1));
            
            CardView boardCardDestination;
            if (card.CanIPlayAbove(out boardCardDestination))
            {
                int cardIndex = card.Index;
                GetNewCard(cardIndex);
                card.PlayAboveTo(boardCardDestination);
                GameStateModel.EnemyCounter.Property++;
                yield break;
            }
        }
    }
}
