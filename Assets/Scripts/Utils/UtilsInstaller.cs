using UnityEngine;
using Zenject;

public class UtilsInstaller : MonoInstaller<UtilsInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<CoroutineProxy>().AsSingle().WithArguments<MonoBehaviour>(this);
    }
}