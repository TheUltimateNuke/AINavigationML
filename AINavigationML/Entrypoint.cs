using AINavigationML;
using DotRecast.Core;
using DotRecast.Detour.Crowd;
using DotRecast.Recast;
using DotRecast.Recast.Toolset.Builder;
using MelonLoader;
using UniRecast.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using NetworkManager = UnityEngine.Networking.NetworkManager;

[assembly: MelonInfo(typeof(Entrypoint), MyModInfo.Name, MyModInfo.Version, MyModInfo.Author)]
[assembly: MelonGame("Boneloaf", "Gang Beasts")]

namespace AINavigationML;

public class Entrypoint : MelonMod
{
    internal static MelonLogger.Instance Logger => Melon<Entrypoint>.Logger;
    
    private const float NavMeshRefreshRateSeconds = 10f;

    internal static bool isDebugMode => MelonDebug.IsEnabled();
    
    private static float _navMeshRefreshRateSeconds = NavMeshRefreshRateSeconds;

    public static UniRcNavMeshSurface? navMeshSurface { get; private set; }
    public static DtCrowd? dtCrowdManager { get; internal set; }

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        NetworkManager.add_OnNetworkSceneLoaded(new Action(OnSceneWasLoaded));
    }

    private void OnSceneWasLoaded()
    {
        if (SceneManager.GetActiveScene().name == "_bootScene") return;

        var navMeshObj = new GameObject("AINavigationML_DynamicNavMesh")
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset
        };
        navMeshSurface = navMeshObj.AddComponent<UniRcNavMeshSurface>();
        navMeshSurface.Bake();
        
        dtCrowdManager ??= new DtCrowd(new DtCrowdConfig(0.1f), navMeshSurface.GetNavMeshData());
        dtCrowdManager.SetObstacleAvoidanceParams(0, new DtObstacleAvoidanceParams
        {
            velBias = 0.5f,
            adaptiveDepth = 1,
            adaptiveDivs = 5,
            adaptiveRings = 2
        });
    }

    public override void OnUpdate()
    {
        if (navMeshSurface == null) return;
        dtCrowdManager?.Update(Time.deltaTime, new DtCrowdAgentDebugInfo());
    }
}