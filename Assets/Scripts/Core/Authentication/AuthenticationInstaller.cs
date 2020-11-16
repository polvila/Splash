using Zenject;

namespace Core.Authentication
{
    public class AuthenticationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IAuthenticationService>().To<AuthenticationService>().AsSingle();
        }
    }
}