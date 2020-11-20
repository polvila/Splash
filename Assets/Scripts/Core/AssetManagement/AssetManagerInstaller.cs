using Core.AssetManagement.Implementations;
using UnityEngine;
using Zenject;

namespace Core.AssetManagement
{
    public class AssetManagerInstaller : MonoInstaller<AssetManagerInstaller>
    {
        [SerializeField] private GameObject[] _prefabs;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<AssetManager>().AsSingle().WithArguments(_prefabs);
        }
    }
}