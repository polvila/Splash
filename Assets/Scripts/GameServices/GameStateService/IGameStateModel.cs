public interface IGameStateModel 
{
	EnemyPlayer EnemyPlayer { get; }
	Board Board { get; }
	HumanPlayer HumanPlayer { get; }

	ModelStateProperty<GameState> State { get; }
	ModelProperty<int> EnemyCounter { get; }
	ModelProperty<int> HumanCounter { get; }
	
	Timer Timer { get; }
	
	ModelProperty<GameResult> Result { get; }
}

public enum GameState
{
	Idle,
	Updated,
	Finished
}

public enum GameResult
{
	HumanWins,
	EnemyWins,
	Draw
}
