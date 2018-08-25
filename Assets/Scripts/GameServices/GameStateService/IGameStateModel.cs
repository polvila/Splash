using System;

public interface IGameStateModel 
{
	EnemyPlayer EnemyPlayer { get; }
	Board Board { get; }
	HumanPlayer HumanPlayer { get; }

	GameState State { get; set; }
	event Action<GameState> StateChanged;
}

public enum GameState
{
	Idle,
	Updated
}
