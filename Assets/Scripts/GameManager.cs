using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
	[SerializeField] private int _initialTimerSeconds;
	
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
		_gameStateModel.EnemyCounter.Property = 0;
		_gameStateModel.HumanCounter.Property = 0;
		
		_gameStateModel.EnemyPlayer.FillSlotsWithCards();
		_gameStateModel.Board.FillSlotsWithCards();
		_gameStateModel.HumanPlayer.FillSlotsWithCards();
		
		_gameStateModel.Timer.TimerEnded += () => 
			_gameStateModel.State.Property = GameState.Finished;
		_gameStateModel.Timer.Init(_initialTimerSeconds);
	}

	void Start()
	{
		_gameStateModel.EnemyPlayer.UpdateIA();
		_gameStateModel.State.PropertyChanged += OnStateChanged;
		_gameStateModel.Timer.StartTimer();
	}

	void OnStateChanged(GameState gameState)
	{
		switch (gameState)
		{
			case GameState.Updated:
				_gameStateModel.State.Property = GameState.Idle;
				_gameStateModel.EnemyPlayer.UpdateIA();
				break;
			case GameState.Finished:
				_gameStateModel.EnemyPlayer.Playable = false;
				_gameStateModel.HumanPlayer.Playable = false;
				_gameStateModel.Result.Property = GetGameResult();
				break;
		}
	}

	private GameResult GetGameResult()
	{
		if (_gameStateModel.EnemyCounter.Property < _gameStateModel.HumanCounter.Property)
		{
			return GameResult.HumanWins;
		}

		if(_gameStateModel.EnemyCounter.Property > _gameStateModel.HumanCounter.Property)
		{
			return GameResult.EnemyWins;
		}

		return GameResult.Draw;
	}

	private void OnDestroy()
	{
		_gameStateModel.State.PropertyChanged -= OnStateChanged;
	}
}
