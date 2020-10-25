using GameSparks.Core;
using UnityEngine;
using Zenject;

public class StateManager : MonoBehaviour, IStateManager
{
	[SerializeField] private PlayMakerFSM _mainFsm;
	[SerializeField] private GameObject _mainMenuCanvas;
	[SerializeField] private GameObject _titleCanvas;
	[SerializeField] private GameObject[] _gameCanvas;
	[SerializeField] private SceneContext _sceneContext;

	private GameObject _gameLoadingCanvasInstance;
	private GameObject[] _gameCanvasInstances;
	
	[Inject] private IAuthenticationService _authenticationService;

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
		_sceneContext.Container.InstantiatePrefab(_mainMenuCanvas);
		
		foreach (var gameCanvasInstance in _gameCanvasInstances)
		{
			Destroy(gameCanvasInstance);
		}
	}

	public void OnLoadingGame()
	{
		Destroy(_mainMenuCanvas);
		//TODO: Move to next state once I find a match using GS
		TriggerEvent(Event.SHOW_GAME);
	}

	public void OnGame()
	{
		Destroy(_gameLoadingCanvasInstance);
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
