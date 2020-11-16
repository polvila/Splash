using Zenject;

namespace Core.CloudOnce
{
    public class CloudOnceInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ICloudOnceService>().To<CloudOnceService>().AsSingle();
        }
    }
}