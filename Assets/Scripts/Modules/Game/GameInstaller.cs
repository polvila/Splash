using Zenject;

public class GameInstaller : MonoInstaller<GameInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<Presenter<BoardView>>().To<BoardPresenter>().AsTransient();
        Container.Bind<Presenter<LeftBarView>>().To<LeftBarPresenter>().AsTransient();
    }
}