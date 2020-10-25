using UnityEngine;
using Zenject;

public class StateManagerInstaller : MonoInstaller<StateManagerInstaller>
{
    [SerializeField] private StateManager _stateManager;
    public override void InstallBindings()
    {
        Container.Bind<IStateManager>().FromInstance(_stateManager).AsSingle();
    }
}