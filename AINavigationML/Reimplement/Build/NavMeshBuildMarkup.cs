using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Runtime;
using UnityEngine.Bindings;
using AINavigationML;

namespace UnityEngine.AI
{
    public struct NavMeshBuildMarkup
    {
        public bool overrideArea
        {
            get
            {
                return this.m_OverrideArea != 0;
            }
            set
            {
                this.m_OverrideArea = (value ? 1 : 0);
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

        public bool ignoreFromBuild
        {
            get
            {
                return this.m_IgnoreFromBuild != 0;
            }
            set
            {
                this.m_IgnoreFromBuild = (value ? 1 : 0);
            }
        }

        public Transform root
        {
            get
            {
                return NavMeshBuildMarkup.InternalGetRootGO(this.m_InstanceID);
            }
            set
            {
                this.m_InstanceID = ((value != null) ? value.GetInstanceID() : 0);
            }
        }

        private delegate Transform InternalGetRootGODelegate(int instanceID);
        
        // ICall
        private static Transform InternalGetRootGO(int instanceID)
        {
            var dlgte = ICallManager.GetICall<InternalGetRootGODelegate>("UnityEngine.AI::NavMeshBuildMarkup::InternalGetRootGO"); 
            return dlgte.Invoke(instanceID);
        }

        private int m_OverrideArea;

        private int m_Area;

        private int m_IgnoreFromBuild;

        private int m_InstanceID;
    }
}