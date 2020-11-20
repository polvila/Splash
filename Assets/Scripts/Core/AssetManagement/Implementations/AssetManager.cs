using System.Collections.Generic;
using Core.AssetManagement.APIs;
using UnityEngine;
using Zenject;

namespace Core.AssetManagement.Implementations
{
    public class AssetManager : IAssetManager
    {
        private Dictionary<string, GameObject> _prefabs;
        private DiContainer _container;

        public AssetManager(GameObject[] prefabs, DiContainer container)
        {
            _container = container;
            _prefabs = new Dictionary<string, GameObject>();
            foreach (var prefab in prefabs)
            {
                _prefabs.Add(prefab.name, prefab);
            }
        }
        
        public GameObject InstantiatePrefab(string name, Transform parent = null)
        {
            return parent ? 
                _container.InstantiatePrefab(_prefabs[name], parent) : 
                _container.InstantiatePrefab(_prefabs[name]);
        }
    }
}