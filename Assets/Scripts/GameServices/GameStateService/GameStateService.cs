using UnityEngine;

public class GameStateService : IGameStateService 
{
    public EnemyPlayer EnemyPlayer { get; }
    public Board Board { get; }
    public HumanPlayer HumanPlayer { get; }

    public GameStateService(Transform[] enemySlots, Transform[] boardSlots, Transform[] humanSlots)
    {
        EnemyPlayer = new EnemyPlayer {Slots = enemySlots};
        Board = new Board {Slots = boardSlots};
        HumanPlayer = new HumanPlayer {Slots = humanSlots};

        EnemyPlayer.FillSlotsWithCards();
        Board.FillSlotsWithCards();
        HumanPlayer.FillSlotsWithCards();
    }
}
