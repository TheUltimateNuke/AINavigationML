using AINavigationML;
using MelonLoader;
using UnityEngine;
using UnityEngine.AI;

[assembly: MelonInfo(typeof(Entrypoint), MyModInfo.Name, MyModInfo.Version, MyModInfo.Author)]
[assembly: MelonGame("Boneloaf", "Gang Beasts")]

namespace AINavigationML;

public class Entrypoint : MelonMod
{
    private const float NavMeshRefreshRateSeconds = 10f;
    
    private static float _navMeshRefreshRateSeconds = NavMeshRefreshRateSeconds;
    private static event Action? OnNavMeshRefresh;
    
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        var navMeshObj = new GameObject("AINavigationML_DynamicNavMesh");
        var surface = navMeshObj.AddComponent<NavMeshSurface>();
        surface.size = Vector3.one * 100f;
        surface.BuildNavMesh();
        OnNavMeshRefresh += surface.BuildNavMesh;
    }

    public override void OnFixedUpdate()
    {
        if (_navMeshRefreshRateSeconds <= 0)
        {
            OnNavMeshRefresh?.Invoke();
            _navMeshRefreshRateSeconds = NavMeshRefreshRateSeconds;

            return;
        }
        
        _navMeshRefreshRateSeconds -= Time.fixedDeltaTime;
    }
}