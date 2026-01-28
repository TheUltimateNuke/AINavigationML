using System.Collections.Generic;
using System.Linq;
using AINavigationML;
using DotRecast.Detour;
using DotRecast.Recast;
using DotRecast.Recast.Toolset;
using Il2CppGB.Game;
using Il2CppGB.Game.PlayArea;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UniRecast.Core
{
    [RegisterTypeInIl2Cpp]
    public class UniRcNavMeshSurface : MonoBehaviour
    {
        private float _cellSize = 1f;

        private float _cellHeight = 0.2f;

        // Agent
        private float _agentHeight = 1.4f;

        private float _agentRadius = 0.4f;

        private float _agentMaxClimb = 0.9f;

        private float _agentMaxSlope = 45f;

        private float _agentMaxAcceleration = 8.0f;

        private float _agentMaxSpeed = 3.5f;

        // Region
        private int _minRegionSize = 8;

        private int _mergedRegionSize = 20;

        // Filtering
        private bool _filterLowHangingObstacles = true;

        private bool _filterLedgeSpans = true;

        private bool _filterWalkableLowHeightSpans = true;

        // Polygonization
        private float _edgeMaxLen = 12f;

        private float _edgeMaxError = 1f;

        private int _vertsPerPoly = 6;

        // Detail Mesh
        private float _detailSampleDist = 6f;

        private float _detailSampleMaxError = 1f;

        // Tiles
        private int _tileSize = 32;

        private UniRcNavMeshData _navMeshData = new();

        private RcNavMeshBuildSettings? ToBuildSettings()
        {
            var bs = new RcNavMeshBuildSettings();
            
            // Rasterization
            bs.cellSize = _cellSize;
            bs.cellHeight = _cellHeight;

            // Agent
            bs.agentHeight = _agentHeight;
            bs.agentHeight = _agentHeight;
            bs.agentRadius = _agentRadius;
            bs.agentMaxClimb = _agentMaxClimb;
            bs.agentMaxSlope = _agentMaxSlope;
            bs.agentMaxAcceleration = _agentMaxAcceleration;
            bs.agentMaxSpeed = _agentMaxSpeed;

            // Region
            bs.minRegionSize = _minRegionSize;
            bs.mergedRegionSize = _mergedRegionSize;

            // Filtering
            bs.filterLowHangingObstacles = _filterLowHangingObstacles;
            bs.filterLedgeSpans = _filterLedgeSpans;
            bs.filterWalkableLowHeightSpans = _filterWalkableLowHeightSpans;

            // Polygonization
            bs.edgeMaxLen = _edgeMaxLen;
            bs.edgeMaxError = _edgeMaxError;
            bs.vertsPerPoly = _vertsPerPoly;

            // Detail Mesh
            bs.detailSampleDist = _detailSampleDist;
            bs.detailSampleMaxError = _detailSampleMaxError;

            // Tiles
            bs.tiled = true;
            bs.tileSize = _tileSize;

            // Use MONOTONE partitioning for performance
            bs.partitioning = RcPartitionType.MONOTONE.Value;

            return bs;
        }

        private void Update()
        {
            _navMeshData.dynamicNavMesh?.Update();
        }

        public void Bake(bool useScene = true)
        {
            var setting = ToBuildSettings();
            if (setting == null) return;
            BakeFrom(setting, useScene);
        }

        private GameObject ConstructDebugCube(Vector3 center)
        {
            var ret = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ret.transform.position = center;
            ret.transform.localScale = Vector3.one * 0.25f;
            Destroy(ret.GetComponent<BoxCollider>());
            return ret;
        }

        public void BakeFrom(RcNavMeshBuildSettings setting, bool useScene = true)
        {
            var currentScene = SceneManager.GetActiveScene();
            var navMeshFilePath = Path.Combine(MelonEnvironment.UserDataDirectory, currentScene.name + ".bytes");

            if (useScene)
            {
                if (File.Exists(navMeshFilePath))
                {
                    try
                    {
                        _navMeshData.dynamicNavMesh.NavMesh(UniRcExtensions.LoadNavMeshFile(navMeshFilePath));
                        return;
                    }
                    catch (Exception e)
                    {
                        Entrypoint.Logger.Error(e);
                        File.Delete(navMeshFilePath);
                    }
                }

                GameObject[] allGameObjects = currentScene.GetRootGameObjects();

                var targets = GetNavMeshSurfaceTargets(allGameObjects);
                if (0 >= targets.Count)
                {
                    Entrypoint.Logger.Msg($"not found navmesh targets");
                    return;
                }

                var combinedTarget = targets.ToCombinedNavMeshSurfaceTarget(currentScene.name);
                var mesh = combinedTarget.ToMesh();
                mesh.SaveFile();
            
                var debugDisplayObj = new GameObject();
                var debugDisplayFilter = debugDisplayObj.AddComponent<MeshFilter>();
                debugDisplayObj.AddComponent<MeshRenderer>();
                debugDisplayFilter.sharedMesh = combinedTarget.GetMesh();

                var navMesh = mesh.Build(setting);
                _navMeshData.dynamicNavMesh = navMesh;

                (_navMeshData.NavMesh ?? throw new NullReferenceException("NavMesh is null!")).SaveNavMeshFile(navMeshFilePath);

                Entrypoint.Logger.Msg($"New NavMesh baked. | Path: {navMeshFilePath} | Max Tiles: {navMesh.NavMesh().GetMaxTiles()}");
            }
            else
            {
                var toTarget = gameObject;
                
            }
        }

        public List<UniRcNavMeshSurfaceTarget> GetNavMeshSurfaceTargets(IList<GameObject> gameObjects)
        {
            /*
            // force terrain
            var terrainTargets = gameObjects
                .SelectMany(x => x.GetComponentsInChildren<Terrain>())
                .Where(x => x.gameObject.activeSelf && x.isActiveAndEnabled)
                .Select(x => x.ToUniRcSurfaceSource());
                */
            
            // all tag objects
            var navmeshTagObjects = gameObjects
                .SelectMany(x => x.ToHierarchyList())
                .Where(x => x.gameObject.activeSelf && PlayAreaCache.InPlayArea(x.transform.position) != null)
                .ToList();
            
            var meshFilterTargets = navmeshTagObjects
                .SelectMany(x => x.GetComponentsInChildren<MeshFilter>().Where(m => m.GetComponent<MeshRenderer>().enabled && m.GetComponentsInChildren<Collider>().Any(c => c.enabled && !c.isTrigger)))
                .Distinct()
                .Select(x => x.ToUniRcSurfaceSource());

            /*var boxColliderTargets = (from obj in navmeshTagObjects
                select obj.GetComponentsInChildren<BoxCollider>()
                into colliders
                from eligibleCollider in colliders.Where<BoxCollider>(c => !c.CompareTag("Helper (Kill Volume)") && c.enabled && !c.isTrigger)
                select eligibleCollider.ToUniRcSurfaceSource()).ToList();*/

            var targets = new List<UniRcNavMeshSurfaceTarget>();
            //targets.AddRange(terrainTargets);
            targets.AddRange(meshFilterTargets);
            //targets.AddRange(boxColliderTargets);

            return targets;
        }

        public void Clear()
        {
            _navMeshData.dynamicNavMesh = null;
        }

        public bool HasNavMeshData()
        {
            return null != _navMeshData.NavMesh;
        }

        public DtNavMesh? GetNavMeshData()
        {
            return _navMeshData.NavMesh;
        }
    }
}