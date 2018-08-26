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
    
    public enum CardGeneratorMode
    {
        Random,
        RandomExcluding,
    }

    public override void InstallBindings()
    {
        BindCardGenerator();

        Container.Bind<IGameStateModel>().To<GameStateModel>()
            .AsSingle()
            .WithArguments(_enemySlots, _boardSlots, _playerSlots);
        
        Container.Bind<Timer>().FromNewComponentOnNewGameObject().AsSingle();
    }

    public void BindCardGenerator()
    {
        switch (_generatorMode)
        {
            case CardGeneratorMode.Random:
                Container.Rebind<ICardGeneratorService>().To<CardGeneratorService>()
                    .AsSingle()
                    .WithArguments(_card, _cardsParent);
                break;
            case CardGeneratorMode.RandomExcluding:
                Container.Rebind<ICardGeneratorService>().To<CardGeneratorExcludingService>()
                    .AsSingle()
                    .WithArguments(_card, _cardsParent);
                break;
        }
    }
}