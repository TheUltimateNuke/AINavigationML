using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UniverseLib.Runtime.Il2Cpp;

namespace UnityEngine.AI
{
    public struct NavMeshBuildSource
    {
        public Matrix4x4 transform
        {
            get
            {
                return this.m_Transform;
            }
            set
            {
                this.m_Transform = value;
            }
        }

        public Vector3 size
        {
            get
            {
                return this.m_Size;
            }
            set
            {
                this.m_Size = value;
            }
        }

        public NavMeshBuildSourceShape shape
        {
            get
            {
                return this.m_Shape;
            }
            set
            {
                this.m_Shape = value;
            }
        }

        public int area
        {
            get
            {
                return this.m_Area;
            }
            set
            {
                this.m_Area = value;
            }
        }

        public Object sourceObject
        {
            get
            {
                return NavMeshBuildSource.InternalGetObject(this.m_InstanceID);
            }
            set
            {
                this.m_InstanceID = ((value != null) ? value.GetInstanceID() : 0);
            }
        }

        public Component component
        {
            get
            {
                return NavMeshBuildSource.InternalGetComponent(this.m_ComponentID);
            }
            set
            {
                this.m_ComponentID = ((value != null) ? value.GetInstanceID() : 0);
            }
        }

        private delegate Component InternalGetComponentDelegate(int instanceID);
        private delegate Component InternalGetObjectDelegate(int instanceID);
        
        private static Component InternalGetComponent(int instanceID)
        {
            return ICallManager.GetICall<InternalGetComponentDelegate>("UnityEngine.AI::NavMeshBuildSource::InternalGetComponent").Invoke(instanceID);
        }

        private static Object InternalGetObject(int instanceID)
        {
            return ICallManager.GetICall<InternalGetObjectDelegate>("UnityEngine.AI::NavMeshBuildSource::InternalGetObject").Invoke(instanceID);
        }

        private Matrix4x4 m_Transform;

        private Vector3 m_Size;

        private NavMeshBuildSourceShape m_Shape;

        private int m_Area;

        private int m_InstanceID;

        private int m_ComponentID;
    }
}