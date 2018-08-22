using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
	private IGameStateModel _gameStateModel;
	
	[Inject]
	void Init(IGameStateModel gameStateModel)
	{
		_gameStateModel = gameStateModel;
	}
	
	void Awake()
	{
		_gameStateModel.EnemyPlayer.FillSlotsWithCards();
		_gameStateModel.Board.FillSlotsWithCards();
		_gameStateModel.HumanPlayer.FillSlotsWithCards();
	}
}
