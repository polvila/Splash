using UnityEngine;
using Zenject;

public partial class SROptions
{
    private DiContainer _container;
    private DiContainer GetContainer()
    {
        _container = GameObject.FindObjectOfType<SceneContext>().Container;
        return _container;
    }    
}
