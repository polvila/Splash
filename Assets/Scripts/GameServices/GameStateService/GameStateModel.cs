using System;
using UnityEngine;
using Zenject;

public class GameStateModel : IGameStateModel 
{
    private GameState _state;
    public GameState State { 
        get { return _state; }
        set
        {
            if (_state == value) return;
            _state = value;
            StateChanged?.Invoke(_state);
        } 
    }
    
    public EnemyPlayer EnemyPlayer { get; }
    public Board Board { get; }
    public HumanPlayer HumanPlayer { get; }
    public event Action<GameState> StateChanged;

    public GameStateModel(Transform[] enemySlots, Transform[] boardSlots, Transform[] humanSlots)
    {
        EnemyPlayer = new EnemyPlayer {Slots = enemySlots};
        Board = new Board {Slots = boardSlots};
        HumanPlayer = new HumanPlayer {Slots = humanSlots};

        _state = GameState.Idle;
    }
}
