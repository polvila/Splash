using System;

public enum Mode
{
    AI,
    Local,
    Online
}

public interface IGameManagerService
{
    event Action<int[]> NewGameReceived;
    event Action<int, int, int?> CardUpdate;
    event Action<int, bool> GameFinished;
    event Action<bool, int, int, int> Splashed;
    event Action<int, int> Unblocked;

    int HumanRecord { get; }

    void Initialize();
    void StartGame(Mode mode);
    void PlayThisCard(int positionCardSelected);
    void HumanSplash();
    void Exit();
}
