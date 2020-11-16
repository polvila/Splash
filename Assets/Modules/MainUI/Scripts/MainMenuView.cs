using Core.StateManager;
using UnityEngine;
using Zenject;
using Event = Core.StateManager.Event;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private GameObject _mainButtonsContainer;
    [SerializeField] private GameObject _aboutContainer;

    [Inject] private IStateManager _stateManager;

    private void Start()
    {
        _mainButtonsContainer.SetActive(true);
        _aboutContainer.SetActive(false);
    }

    public void OnPlay()
    {
        _stateManager.TriggerEvent(Event.SHOW_GAME);
    }
}