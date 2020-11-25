using Core.StateManager;
using Modules.Game;
using UnityEngine;
using Zenject;
using Event = Core.StateManager.Event;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private GameObject _mainButtonsContainer;
    [SerializeField] private GameObject _aboutContainer;

    private IStateManager _stateManager;
    private IGameManagerService _gameManagerService;
    private IPlayerModel _playerModel;

    [Inject]
    private void Init(
        IStateManager stateManager,
        IGameManagerService gameManagerService,
        IPlayerModel playerModel)
    {
        _stateManager = stateManager;
        _gameManagerService = gameManagerService;
        _playerModel = playerModel;
    }

    private void Start()
    {
        _mainButtonsContainer.SetActive(true);
        _aboutContainer.SetActive(false);
    }

    public void OnPlay()
    {
        _stateManager.TriggerEvent(_playerModel.FTUECompleted ? Event.SHOW_GAME : Event.SHOW_TUTORIAL);
    }

    public void OnTutorial()
    {
        _stateManager.TriggerEvent(Event.SHOW_TUTORIAL);
    }
}