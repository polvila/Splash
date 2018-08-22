using UnityEngine;
using Zenject;

public class GameStateModel : IGameStateModel 
{
    public EnemyPlayer EnemyPlayer { get; }
    public Board Board { get; }
    public HumanPlayer HumanPlayer { get; }

    public GameStateModel(Transform[] enemySlots, Transform[] boardSlots, Transform[] humanSlots, DiContainer container)
    {
        EnemyPlayer = new EnemyPlayer {Slots = enemySlots};
        Board = new Board {Slots = boardSlots};
        HumanPlayer = new HumanPlayer {Slots = humanSlots};
        
        container.Inject(EnemyPlayer);
        container.Inject(Board);
        container.Inject(HumanPlayer);
    }
}
