public interface IGameStateModel 
{
	EnemyPlayer EnemyPlayer { get; }
	Board Board { get; }
	HumanPlayer HumanPlayer { get; }

	ModelStateProperty<GameState> State { get; }
	ModelProperty<int> EnemyCounter { get; }
	ModelProperty<int> HumanCounter { get; }
	
	Timer Timer { get; }
}

public enum GameState
{
	Idle,
	Updated
}
