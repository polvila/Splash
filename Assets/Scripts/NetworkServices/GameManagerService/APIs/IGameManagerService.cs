using System;
using UnityEngine.Networking.NetworkSystem;

public enum Mode
{
    IA,
    Local,
    Online
}

public interface IGameManagerService
{
    event Action<int[]> NewBoardReceived;
    event Action<int, int, int?> CardUpdate;
    
    void Initialize();
    void Start(Mode mode);
    void PlayThisCard(int positionCardSelected);
}
