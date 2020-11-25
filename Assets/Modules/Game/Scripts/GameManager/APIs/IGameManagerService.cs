using System;

namespace Modules.Game
{
    public enum Mode
    {
        AI,
        FTUE,
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

        void Initialize(Mode gameMode);
        void StartGame();
        void PlayThisCard(int positionCardSelected);
        void HumanSplash();
        void Exit();
    }
}
