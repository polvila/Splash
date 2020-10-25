public enum GameState
{
    Idle,
    Updated,
    Finished
}

public enum GameResult
{
    None,
    HumanWins,
    EnemyWins,
    Draw
}

public class GameStateModel
{
    public int[] Numbers;
    public GameState State;
    public int EnemyCounter;
    public int HumanCounter;
    public GameResult Result;

    public GameStateModel(int[] numbers)
    {
        Numbers = numbers;
        State = GameState.Idle;
    }
}
