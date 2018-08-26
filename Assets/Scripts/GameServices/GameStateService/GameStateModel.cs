using UnityEngine;

public class GameStateModel : IGameStateModel
{
    public EnemyPlayer EnemyPlayer { get; }
    public Board Board { get; }
    public HumanPlayer HumanPlayer { get; }

    public ModelStateProperty<GameState> State { get; }
    public ModelProperty<int> EnemyCounter { get; }
    public ModelProperty<int> HumanCounter { get; }
    
    public Timer Timer { get; }

    public ModelProperty<GameResult> Result { get; }

    public GameStateModel(Transform[] enemySlots, Transform[] boardSlots, Transform[] humanSlots,
        Timer timer)
    {
        EnemyPlayer = new EnemyPlayer {Slots = enemySlots};
        Board = new Board {Slots = boardSlots};
        HumanPlayer = new HumanPlayer {Slots = humanSlots};

        State = new ModelStateProperty<GameState>();;
        EnemyCounter = new ModelProperty<int>();
        HumanCounter = new ModelProperty<int>();

        Timer = timer;

        Result = new ModelProperty<GameResult>();

        State.Property = GameState.Idle;
    }
}
