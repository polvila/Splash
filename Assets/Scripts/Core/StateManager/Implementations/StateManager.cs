using UnityEngine;
using Zenject;

public class StateManager : MonoBehaviour, IStateManager, IInitializable
{
    [SerializeField] private PlayMakerFSM _mainFsm;
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _titleCanvas;
    [SerializeField] private GameObject[] _gameCanvas;
    [SerializeField] private SceneContext _sceneContext;

    private GameObject _gameLoadingCanvasInstance;
    private GameObject[] _gameCanvasInstances;

    private IAuthenticationService _authenticationService;
    private IScreenManager _screenManager;

    [Inject]
    private void Init(
        IAuthenticationService authenticationService, 
        IScreenManager screenManager)
    {
        _authenticationService = authenticationService;
        _screenManager = screenManager;
    }

    public void Initialize()
    {
        TriggerEvent(Event.START_BOOTSTRAP);
    }

    private void Start()
    {
        _gameCanvasInstances = new GameObject[_gameCanvas.Length];
    }

    public void OnBoostrap()
    {
        _authenticationService.InitService(() =>
        {
            _authenticationService.DeviceAuthentication(() =>
            {
                Destroy(_titleCanvas);
                TriggerEvent(Event.SHOW_MAIN_MENU);
            });
        });
    }

    public void OnMainMenu()
    {
        _screenManager.ShowScreen("MainMenuScreen");
        _sceneContext.Container.InstantiatePrefab(_mainMenuCanvas);

        foreach (var gameCanvasInstance in _gameCanvasInstances)
        {
            Destroy(gameCanvasInstance);
        }
    }

    public void OnLoadingGame()
    {
        _screenManager.ShowSpinner();
        //TODO: Move to next state once I find a match using GS
        TriggerEvent(Event.SHOW_GAME);
    }

    public void OnGame()
    {
        _screenManager.HideSpinner();
        for (int i = 0; i < _gameCanvas.Length; ++i)
        {
            _gameCanvasInstances[i] = _sceneContext.Container.InstantiatePrefab(_gameCanvas[i]);
        }
    }

    public void TriggerEvent(string eventKey)
    {
        _mainFsm.SendEvent(eventKey);
    }
}