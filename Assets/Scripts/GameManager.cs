using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
	private IGameStateModel _gameStateModel;
	
	[Inject]
	void Init(IGameStateModel gameStateModel, DiContainer container)
	{
		_gameStateModel = gameStateModel;
		
		container.Inject(_gameStateModel.EnemyPlayer);
		container.Inject(_gameStateModel.Board);
		container.Inject(_gameStateModel.HumanPlayer);
	}
	
	void Awake()
	{
		_gameStateModel.EnemyPlayer.FillSlotsWithCards();
		_gameStateModel.Board.FillSlotsWithCards();
		_gameStateModel.HumanPlayer.FillSlotsWithCards();
	}

	void Start()
	{
		_gameStateModel.EnemyPlayer.UpdateIA();
		_gameStateModel.StateChanged += OnStateChanged;
	}

	void OnStateChanged(GameState gameState)
	{
		if (gameState == GameState.Updated)
		{
			_gameStateModel.State = GameState.Idle;
			_gameStateModel.EnemyPlayer.UpdateIA();
		}
	}

	private void OnDestroy()
	{
		_gameStateModel.StateChanged -= OnStateChanged;
	}
}
