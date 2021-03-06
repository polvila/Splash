using UnityEngine;
using Zenject;

namespace Core.ScreenManagement
{
    public class ScreenManagementInstaller : MonoInstaller<ScreenManagementInstaller>
    {
        [SerializeField] private LoadingSpinnerView _spinner;
        [SerializeField] private GameObject _parent;
        [SerializeField] private bool _worldPositionStays;
        [SerializeField] private Camera _renderCamera;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ScreenManager>().AsSingle()
                .WithArguments(new ScreenManager.ScreenManagerConfig
                (
                    _spinner, 
                    _parent, 
                    _worldPositionStays, 
                    _renderCamera
                ));
        }
    }
}