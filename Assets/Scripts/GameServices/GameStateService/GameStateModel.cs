using UnityEngine;

public class GameStateModel : IGameStateModel 
{
    public EnemyPlayer EnemyPlayer { get; }
    public Board Board { get; }
    public HumanPlayer HumanPlayer { get; }

    public ModelProperty<GameState> State { get; }
    public ModelProperty<int> EnemyCounter { get; }
    public ModelProperty<int> HumanCounter { get; }

    public GameStateModel(Transform[] enemySlots, Transform[] boardSlots, Transform[] humanSlots)
    {
        EnemyPlayer = new EnemyPlayer {Slots = enemySlots};
        Board = new Board {Slots = boardSlots};
        HumanPlayer = new HumanPlayer {Slots = humanSlots};

        State = new ModelProperty<GameState>();
        EnemyCounter = new ModelProperty<int>();
        HumanCounter = new ModelProperty<int>();
        
        State.Property = GameState.Idle;
    }
}
