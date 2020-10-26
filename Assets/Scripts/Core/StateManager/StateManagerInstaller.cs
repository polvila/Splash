using UnityEngine;
using Zenject;

namespace Core.StateManager
{
    public class StateManagerInstaller : MonoInstaller<StateManagerInstaller>
    {
        [SerializeField] private global::StateManager _stateManager;

        public override void InstallBindings()
        {
            //Container.Bind<IStateManager>().FromInstance(_stateManager).AsSingle();
            Container.BindInterfacesTo<global::StateManager>().FromInstance(_stateManager).AsSingle();
        }
    }
}