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
}
