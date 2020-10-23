using GameSparks.Core;
using UnityEngine;
using Zenject;

public class StateManager : MonoBehaviour, IStateManager
{
	[SerializeField] private PlayMakerFSM _mainFsm;
	[SerializeField] private GameObject _playCanvas;
	[SerializeField] private GameObject _titleCanvas;
	[SerializeField] private GameObject _loadingCanvas;
	[SerializeField] private GameObject _gameLoadingCanvas;
	[SerializeField] private GameObject[] _gameCanvas;
	[SerializeField] private SceneContext _sceneContext;

	private GameObject _gameLoadingCanvasInstance;
	private GameObject[] _gameCanvasInstances;
	
	[Inject] private IAuthenticationService _authenticationService;

	private void Start()
	{
		_gameCanvasInstances = new GameObject[_gameCanvas.Length];
		
		GS.GameSparksAvailable += (isAvailable) =>
		{
			if(isAvailable)
			{
				Debug.Log("GameSparks Connected...");
				MoveToNextState();
			}
			else
			{
				Debug.Log("GameSparks Disconnected...");
			}
		};
	}

	public void OnLoading()
	{
		Debug.Log("Loading");
		_authenticationService.DeviceAuthentication(MoveToNextState);
	}
	
	public void OnPlay()
	{
		foreach (var gameCanvasInstance in _gameCanvasInstances)
		{
			Destroy(gameCanvasInstance);
		}
		_titleCanvas.SetActive(true);
		_sceneContext.Container.InstantiatePrefab(_playCanvas);
	}

	public void OnLoadingGame()
	{
		_titleCanvas.SetActive(false);
		Destroy(_loadingCanvas);
		_gameLoadingCanvasInstance = _sceneContext.Container.InstantiatePrefab(_gameLoadingCanvas);
		//TODO: Move to next state once I find a match using GS
		MoveToNextState();
	}

	public void OnGame()
	{
		Destroy(_gameLoadingCanvasInstance);
		for (int i = 0; i < _gameCanvas.Length; ++i)
		{
			_gameCanvasInstances[i] = _sceneContext.Container.InstantiatePrefab(_gameCanvas[i]);
		}
	}

	public void MoveToNextState()
	{
		_mainFsm.SendEvent("next");
	}

	public void TriggerEvent(string eventKey)
	{
		_mainFsm.SendEvent(eventKey);
	}
}
