using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class GameServicesInstaller : MonoInstaller
{
    [Header("Card Generator")]
    [SerializeField] private CardGeneratorMode _generatorMode;
    [SerializeField] private GameObject _card;
    [SerializeField] private Transform _cardsParent;
    
    [Header("Slots")]
    [SerializeField] private Transform[] _enemySlots;
    [SerializeField] private Transform[] _boardSlots;
    [SerializeField] private Transform[] _playerSlots;
    
    public override void InstallBindings()
    {
        Container.Bind<RandomGenerator>().AsSingle().WithArguments(_card, _cardsParent);
        Container.Bind<RandomExcludingGenerator>().AsSingle().WithArguments(_card, _cardsParent);
        
        Container.Bind<ICardGeneratorService>().To<CardGeneratorService>()
            .AsSingle()
            .WithArguments(_generatorMode);

        Container.Bind<IGameStateModel>().To<GameStateModel>()
            .AsSingle()
            .WithArguments(_enemySlots, _boardSlots, _playerSlots);
        
        Container.Bind<Timer>().FromNewComponentOnNewGameObject().AsTransient();
    }
}