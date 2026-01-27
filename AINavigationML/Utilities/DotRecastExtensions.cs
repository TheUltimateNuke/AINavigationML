using DotRecast.Core.Numerics;
using UnityEngine;

namespace AINavigationML.Utilities;

public static class DotRecastExtensions
{
    public static RcVec3f ToRc(this Vector3 vector3)
    {
        return new RcVec3f(-vector3.x, vector3.y, vector3.z);
    }
}