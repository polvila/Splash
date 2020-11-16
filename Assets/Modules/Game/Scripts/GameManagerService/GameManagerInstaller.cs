using UnityEngine;
using Zenject;

namespace Modules.Game
{
    public class GameManagerInstaller : MonoInstaller
    {
        [SerializeField] private CardGeneratorMode _generatorMode;
    
        public override void InstallBindings()
        {
            Container.Bind<RandomGenerator>().AsSingle();
            Container.Bind<RandomExcludingGenerator>().AsSingle();
        
            Container.Bind<INumberGeneratorService>().To<NumberGeneratorServiceMock>()
                .AsSingle()
                .WithArguments(_generatorMode);

            Container.BindInterfacesTo<GameManagerServiceMock>()
                .AsSingle();
        }
    }
}