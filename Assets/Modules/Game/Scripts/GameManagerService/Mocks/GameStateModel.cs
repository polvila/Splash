public enum GameState
{
    Idle,
    Updated,
    Finished
}

public class GameStateModel
{
    public int[] Numbers;
    public GameState State;
    public int HumanPointsCounter;
    public int HumanLifePoints;
    public int SplashPot;

    public GameStateModel(int[] numbers)
    {
        Numbers = numbers;
        State = GameState.Idle;
    }
}
