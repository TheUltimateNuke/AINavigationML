
using DotRecast.Detour;
using DotRecast.Detour.Dynamic;

namespace UniRecast.Core
{
    public class UniRcNavMeshData
    {
        public DtNavMesh? NavMesh => dynamicNavMesh?.NavMesh();
        public DtDynamicNavMesh? dynamicNavMesh;
    }
}