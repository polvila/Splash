using Zenject;

namespace Modules.Game
{
    public class GameManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameManagerServiceMock>().AsSingle();
        }
    }
}