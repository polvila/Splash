public interface IGameStateService 
{
	EnemyPlayer EnemyPlayer { get; }
	Board Board { get; }
	HumanPlayer HumanPlayer { get; }
}
