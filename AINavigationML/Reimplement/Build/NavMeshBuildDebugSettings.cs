namespace UnityEngine.AI;

public struct NavMeshBuildDebugSettings
{
    public NavMeshBuildDebugFlags flags
    {
        get
        {
            return (NavMeshBuildDebugFlags)this.m_Flags;
        }
        set
        {
            this.m_Flags = (byte)value;
        }
    }

    private byte m_Flags;
}