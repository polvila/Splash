using UnityEngine;
using Zenject;

public class GameServicesInstaller : MonoInstaller
{
    [SerializeField] private GameObject _card;
    [SerializeField] private Transform _gameCanvas;

    public override void InstallBindings()
    {
        Container.Bind<ICardGeneratorService>().To<CardGeneratorService>()
            .AsSingle()
            .WithArguments(_card, _gameCanvas);        
    }
}