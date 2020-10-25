using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayScreen : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    
    [Inject]
    private void Init(IStateManager stateManager)
    {
        _playButton.onClick.AddListener(() =>
        {
            stateManager.TriggerEvent(Event.LOAD_GAME);
            Destroy(gameObject);
        });
    }
}