using System.Runtime.InteropServices;
using AINavigationML;
using Il2CppInterop.Common;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace UnityEngine.AI.Extensions;

public static class NavMeshBuilderExtensions
{
    public static AsyncOperation UpdateNavMeshDataAsync(NavMeshData data, NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds)
    {
        bool flag = data == null;
        if (flag)
        {
            throw new ArgumentNullException("data");
        }
        bool flag2 = sources == null;
        if (flag2)
        {
            throw new ArgumentNullException("sources");
        }
        return UpdateNavMeshDataAsyncListInternal(data, buildSettings, sources, localBounds);
    }
    
    private static AsyncOperation UpdateNavMeshDataAsyncListInternal(NavMeshData data, NavMeshBuildSettings buildSettings, object sources, Bounds localBounds)
    {
        return UpdateNavMeshDataAsyncListInternal_Injected(data, ref buildSettings, sources, ref localBounds);
    }
    
    public static NavMeshData BuildNavMeshData(NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds, Vector3 position, Quaternion rotation)
    {
        bool flag = sources == null;
        if (flag)
        {
            throw new ArgumentNullException("sources");
        }
        NavMeshData navMeshData = new NavMeshData(buildSettings.agentTypeID)
        {
            position = position,
            rotation = rotation
        };
        UpdateNavMeshDataListInternal(navMeshData, buildSettings, sources, localBounds);
        return navMeshData;
    }
    
    private static bool UpdateNavMeshDataListInternal(NavMeshData data, NavMeshBuildSettings buildSettings, object sources, Bounds localBounds)
    {
        return NavMeshBuilderExtensions.UpdateNavMeshDataListInternal_Injected(data, ref buildSettings, sources, ref localBounds);
    }
    
    public static void CollectSources(Bounds includedWorldBounds, int includedLayerMask, NavMeshCollectGeometry geometry, int defaultArea, List<NavMeshBuildMarkup> markups, List<NavMeshBuildSource> results)
    {
        bool flag = markups == null;
        if (flag)
        {
            throw new ArgumentNullException("markups");
        }
        bool flag2 = results == null;
        if (flag2)
        {
            throw new ArgumentNullException("results");
        }
        includedWorldBounds.extents = Vector3.Max(includedWorldBounds.extents, 0.001f * Vector3.one);
        NavMeshBuildSource[] array = CollectSourcesInternal(includedLayerMask, includedWorldBounds, null, true, geometry, defaultArea, markups.ToArray());
        results.Clear();
        results.AddRange(array);
    }

    public static void CollectSources(Transform root, int includedLayerMask, NavMeshCollectGeometry geometry, int defaultArea, List<NavMeshBuildMarkup> markups, List<NavMeshBuildSource> results)
    {
        bool flag = markups == null;
        if (flag)
        {
            throw new ArgumentNullException("markups");
        }
        bool flag2 = results == null;
        if (flag2)
        {
            throw new ArgumentNullException("results");
        }
        NavMeshBuildSource[] array = CollectSourcesInternal(includedLayerMask, default(Bounds), root, false, geometry, defaultArea, markups.ToArray());
        results.Clear();
        results.AddRange(array);
    }

    private static NavMeshBuildSource[] CollectSourcesInternal(int includedLayerMask, Bounds includedWorldBounds, Transform root, bool useBounds, NavMeshCollectGeometry geometry, int defaultArea, NavMeshBuildMarkup[] markups)
    {
        return CollectSourcesInternal_Injected(includedLayerMask, ref includedWorldBounds, root, useBounds, geometry, defaultArea, markups);
    }
    
    // ICalls
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate bool UpdateNavMeshDataListInternal_InjectedDelegate(NavMeshData data, ref IntPtr buildSettings, object sources, ref Bounds localBounds);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr UpdateNavMeshDataAsyncListInternal_InjectedDelegate(NavMeshData data, ref IntPtr buildSettings, object sources, ref Bounds localBounds);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr CollectSourcesInternal_InjectedDelegate(int includedLayerMask, ref Bounds includedWorldBounds, Transform root, bool useBounds, NavMeshCollectGeometry geometry, int defaultArea, IntPtr markups);
    
    private static unsafe bool UpdateNavMeshDataListInternal_Injected(NavMeshData data, ref NavMeshBuildSettings buildSettings,
        object sources, ref Bounds localBounds)
    {
        fixed (NavMeshBuildSettings* pBuildSettings = &buildSettings)
        {
            var placeholder = (IntPtr)pBuildSettings;
            var ret = ICallManager.GetICall<UpdateNavMeshDataListInternal_InjectedDelegate>("UnityEngine.AI.NavMeshBuilder::UpdateNavMeshDataListInternal_Injected").Invoke(data, ref placeholder, sources, ref localBounds);
            buildSettings = IL2CPP.PointerToValueGeneric<NavMeshBuildSettings>(placeholder, false, false);
            return ret;
        }
    }

    private static unsafe AsyncOperation UpdateNavMeshDataAsyncListInternal_Injected(NavMeshData data,
        ref NavMeshBuildSettings buildSettings, object sources, ref Bounds localBounds)
    {
        fixed (NavMeshBuildSettings* pBuildSettings = &buildSettings)
        {
            var placeholder = (IntPtr)pBuildSettings;
            var ret = new AsyncOperation(ICallManager.GetICall<UpdateNavMeshDataAsyncListInternal_InjectedDelegate>("UnityEngine.AI.NavMeshBuilder::UpdateNavMeshDataAsyncListInternal_Injected").Invoke(data, ref placeholder, sources, ref localBounds));
            buildSettings = IL2CPP.PointerToValueGeneric<NavMeshBuildSettings>(placeholder, false, false);
            return ret;
        }
    }

    private static unsafe NavMeshBuildSource[] CollectSourcesInternal_Injected(int includedLayerMask,
        ref Bounds includedWorldBounds, Transform root, bool useBounds, NavMeshCollectGeometry geometry,
        int defaultArea, NavMeshBuildMarkup[] markups)
    {
        fixed (NavMeshBuildMarkup* pMarkup = markups)
        {
            var placeholderMarkup = (IntPtr)pMarkup;
            var ret = ICallManager
                .GetICall<CollectSourcesInternal_InjectedDelegate>(
                    "UnityEngine.AI.NavMeshBuilder::CollectSourcesInternal_Injected").Invoke(includedLayerMask, ref includedWorldBounds, root, useBounds, geometry, defaultArea, placeholderMarkup);
            //includedWorldBounds = IL2CPP.PointerToValueGeneric<Bounds>(placeholderWorldBounds, false, false);
            return IL2CPP.PointerToValueGeneric<NavMeshBuildSource[]>(ret, false, false);
        }
    }
}