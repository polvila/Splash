using UnityEngine;
using Zenject;

namespace Core.NetworkServices
{
    public class NetworkServicesInstaller : MonoInstaller
    {
        [Header("Card Generator")]
        [SerializeField] private CardGeneratorMode _generatorMode;
        [SerializeField] private GameObject _card;
    
        public override void InstallBindings()
        {
            Container.Bind<RandomGenerator>().AsSingle();
            Container.Bind<RandomExcludingGenerator>().AsSingle();
        
            Container.Bind<INumberGeneratorService>().To<NumberGeneratorServiceMock>()
                .AsSingle()
                .WithArguments(_generatorMode);

            Container.Bind<IGameManagerService>().To<GameManagerServiceMock>()
                .AsSingle();

            Container.Bind<IAuthenticationService>().To<AuthenticationService>().AsSingle();
        }
    }
}
