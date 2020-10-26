using UnityEngine;
using Zenject;

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
        _stateManager.TriggerEvent(Event.LOAD_GAME);
    }
}