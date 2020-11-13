public class GameStateModel
{
    public int[] Numbers;
    public bool GamePaused;
    public int HumanPointsCounter;
    public int HumanLifePoints;
    public int SplashPot;

    public GameStateModel(int[] numbers)
    {
        Numbers = numbers;
        GamePaused = true;
    }
}
