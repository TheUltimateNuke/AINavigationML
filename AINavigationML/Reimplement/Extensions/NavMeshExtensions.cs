using AINavigationML;

namespace UnityEngine.AI.Extensions;

public static class NavMeshExtensions
{
    public static NavMeshLinkInstance AddLink(NavMeshLinkData link)
    {
        return new NavMeshLinkInstance
        {
            id = AddLinkInternal(link, Vector3.zero, Quaternion.identity)
        };
    }

    public static NavMeshLinkInstance AddLink(NavMeshLinkData link, Vector3 position, Quaternion rotation)
    {
        return new NavMeshLinkInstance
        {
            id = AddLinkInternal(link, position, rotation)
        };
    }
    
    internal static int AddLinkInternal(NavMeshLinkData link, Vector3 position, Quaternion rotation)
    {
        return AddLinkInternal_Injected(ref link, ref position, ref rotation);
    }
    
    public static NavMeshBuildSettings GetSettingsByID(int agentTypeID)
    {
        NavMeshBuildSettings navMeshBuildSettings;
        NavMeshExtensions.GetSettingsByID_Injected(agentTypeID, out navMeshBuildSettings);
        return navMeshBuildSettings;
    }
    
    public static NavMeshDataInstance AddNavMeshData(NavMeshData navMeshData)
    {
        bool flag = navMeshData == null;
        if (flag)
        {
            throw new ArgumentNullException("navMeshData");
        }
        return new NavMeshDataInstance
        {
            id = NavMesh.AddNavMeshDataInternal(navMeshData)
        };
    }

    public static NavMeshDataInstance AddNavMeshData(NavMeshData navMeshData, Vector3 position, Quaternion rotation)
    {
        bool flag = navMeshData == null;
        if (flag)
        {
            throw new ArgumentNullException("navMeshData");
        }
        return new NavMeshDataInstance
        {
            id = NavMesh.AddNavMeshDataTransformedInternal(navMeshData, position, rotation)
        };
    }
    
    // ICalls
    private delegate int AddLinkInternal_InjectedDelegate(ref NavMeshLinkData link, ref Vector3 position, ref Quaternion rotation);

    private static int AddLinkInternal_Injected(ref NavMeshLinkData link, ref Vector3 position, ref Quaternion rotation)
    {
        return ICallManager.GetICall<AddLinkInternal_InjectedDelegate>(
            "UnityEngine.AI.NavMeshBuildSettings::AddLinkInternal_Injected").Invoke(ref link, ref position, ref rotation);
    }
    
    private delegate void GetSettingsByID_InjectedDelegate(int agentTypeID, out NavMeshBuildSettings navMeshBuildSettings);

    private static void GetSettingsByID_Injected(int agentTypeID, out NavMeshBuildSettings ret)
    {
        ICallManager.GetICall<GetSettingsByID_InjectedDelegate>("UnityEngine.AI::NavMeshBindings::GetSettingsByIdD_Injected").Invoke(agentTypeID, out ret);
    }
}