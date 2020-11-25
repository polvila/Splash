using Zenject;

namespace Modules.Game
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<Presenter<IBoardView>>().To<BoardPresenter>().AsTransient();
        }
    }
}