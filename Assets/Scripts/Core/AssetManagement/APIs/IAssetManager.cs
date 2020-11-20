using UnityEngine;

namespace Core.AssetManagement.APIs
{
    public interface IAssetManager
    {
        GameObject InstantiatePrefab(string name, Transform parent = null);
    }
}