using UnityEngine;
using Zenject;

public class GameServicesInstaller : MonoInstaller
{
    [SerializeField] private GameObject _card;
    [SerializeField] private Transform _cardsParent;
    
    [Header("Slots")]
    [SerializeField] private Transform[] _enemySlots;
    [SerializeField] private Transform[] _boardSlots;
    [SerializeField] private Transform[] _playerSlots;

    public override void InstallBindings()
    {
        Container.Bind<ICardGeneratorService>().To<CardGeneratorService>()
            .AsSingle()
            .WithArguments(_card, _cardsParent);

        Container.Bind<IGameStateModel>().To<GameStateModel>()
            .AsSingle()
            .WithArguments(_enemySlots, _boardSlots, _playerSlots);
    }
}