using UnityEngine;
using Zenject;

namespace Modules.Game
{
    public class NumberGeneratorInstaller : MonoInstaller
    {
        [SerializeField] private CardGeneratorMode _generatorMode;
    
        public override void InstallBindings()
        {
            Container.Bind<RandomGenerator>().AsSingle();
            Container.Bind<RandomExcludingGenerator>().AsSingle();
            Container.Bind<FTUEGenerator>().AsSingle();

            Container.Bind<INumberGeneratorService>().To<NumberGeneratorServiceMock>()
                .AsSingle()
                .WithArguments(_generatorMode);
        }
    }
}