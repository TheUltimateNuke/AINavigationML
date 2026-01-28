using System.Collections.Generic;
using System.IO;
using System.Text;
using DotRecast.Core;
using DotRecast.Core.Numerics;
using DotRecast.Detour;
using DotRecast.Detour.Dynamic.Colliders;
using DotRecast.Detour.Io;
using DotRecast.Recast;
using DotRecast.Recast.Toolset.Builder;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace UniRecast.Core
{
    // https://github.com/highfidelity/unity-to-hifi-exporter/blob/master/Assets/UnityToHiFiExporter/Editor/TerrainObjExporter.cs
    public static class UniRcExtensions
    {
        public static DtBoxCollider ToDtCollider(this BoxCollider boxCollider)
        {
            var ret = new DtBoxCollider(boxCollider.transform.TransformPoint(boxCollider.center).ToRightHand(),
                DtBoxCollider.GetHalfEdges(boxCollider.transform.up.ToRightHand(),
                    boxCollider.transform.forward.ToRightHand(), boxCollider.extents.ToRightHand()),
                SampleAreaModifications.SAMPLE_POLYAREA_TYPE_GROUND, 0.9f);
            return ret;
        }
        
        public static DtCapsuleCollider ToDtCollider(this CapsuleCollider capsuleCollider)
        {
            var capsuleOffset = capsuleCollider.height / 2f - capsuleCollider.radius;
            var start = capsuleCollider.center - (capsuleCollider.transform.up * capsuleOffset);
            var end = capsuleCollider.center + (capsuleCollider.transform.up * capsuleOffset);
            var ret = new DtCapsuleCollider(start.ToRightHand(), end.ToRightHand(), capsuleCollider.radius,
                SampleAreaModifications.SAMPLE_POLYAREA_TYPE_GROUND, 0.9f);
            return ret;
        }
        
        public static RcVec3f ToRightHand(this Vector3 v)
        {
            return new RcVec3f(-v.x, v.y, v.z);
        }

        public static UniRcNavMeshSurfaceTarget ToUniRcSurfaceSource(this MeshFilter meshFilter)
        {
            return new UniRcNavMeshSurfaceTarget(meshFilter.name, meshFilter.sharedMesh.GetTopMesh(meshFilter.transform), meshFilter.transform.localToWorldMatrix);
        }
        
        public static UniRcNavMeshSurfaceTarget ToUniRcSurfaceSource(this BoxCollider boxCollider)
        {
            return new UniRcNavMeshSurfaceTarget(boxCollider.name, GenerateMeshFromBoxCollider(boxCollider).GetTopMesh(boxCollider.transform), boxCollider.transform.localToWorldMatrix);
        }

        public static Mesh GetTopMesh(this Mesh mesh, Transform transform)
        {
            var triangles = mesh.triangles;
            var vertices = mesh.vertices;
            var topSurfaceVertices = new List<Vector3>();
            var topSurfaceTriangles = new List<int>();
            
            for (var i = 0; i < triangles.Length; i += 3)
            {
                var v1 = transform.TransformPoint(vertices[triangles[i]]);
                var v2 = transform.TransformPoint(vertices[triangles[i + 1]]);
                var v3 = transform.TransformPoint(vertices[triangles[i + 2]]);

                // Calculate normal of the triangle
                var normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;
                // Check if the normal points generally upward
                if (normal.y > 0.45f) // Adjust 0.7f as needed
                {
                    var indexOffset = topSurfaceVertices.Count;
                    topSurfaceVertices.Add(vertices[triangles[i]]);
                    topSurfaceVertices.Add(vertices[triangles[i + 1]]);
                    topSurfaceVertices.Add(vertices[triangles[i + 2]]);
                    topSurfaceTriangles.Add(indexOffset);
                    topSurfaceTriangles.Add(indexOffset + 1);
                    topSurfaceTriangles.Add(indexOffset + 2);
                }
            }

            var ret = new Mesh
            {
                vertices = topSurfaceVertices.ToArray(),
                triangles = topSurfaceTriangles.ToArray()
            };
            ret.RecalculateNormals();
            ret.RecalculateBounds();
            ret.Optimize();

            return ret;
        }

        private static Mesh GenerateMeshFromBoxCollider(BoxCollider collider)
        {
            var size = collider.size;
            var center = collider.center;

            // Vertices of the cube
            var p0 = center + new Vector3(-size.x * 0.5f, -size.y * 0.5f, size.z * 0.5f);
            var p1 = center + new Vector3(size.x * 0.5f, -size.y * 0.5f, size.z * 0.5f);
            var p2 = center + new Vector3(size.x * 0.5f, -size.y * 0.5f, -size.z * 0.5f);
            var p3 = center + new Vector3(-size.x * 0.5f, -size.y * 0.5f, -size.z * 0.5f);
            var p4 = center + new Vector3(-size.x * 0.5f, size.y * 0.5f, size.z * 0.5f);
            var p5 = center + new Vector3(size.x * 0.5f, size.y * 0.5f, size.z * 0.5f);
            var p6 = center + new Vector3(size.x * 0.5f, size.y * 0.5f, -size.z * 0.5f);
            var p7 = center + new Vector3(-size.x * 0.5f, size.y * 0.5f, -size.z * 0.5f);

            var vertices = new[]
            {
                // Bottom
                p0, p1, p2, p3,
                // Top
                p4, p5, p6, p7,
                // Front
                p4, p5, p1, p0,
                // Back
                p6, p7, p3, p2,
                // Left
                p7, p4, p0, p3,
                // Right
                p5, p6, p2, p1
            };

            // Triangles of the cube
            var triangles = new[]
            {
                // Bottom
                3, 1, 0,
                3, 2, 1,
                // Top
                7, 5, 4,
                7, 6, 5,
                // Front
                4, 1, 0,
                4, 5, 1,
                // Back
                6, 3, 2,
                6, 7, 3,
                // Left
                7, 0, 3,
                7, 4, 0,
                // Right
                5, 2, 1,
                5, 6, 2
            };

            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };
            mesh.Optimize();
            mesh.RecalculateNormals();
            
            return mesh;
        }

        /*
        public static UniRcNavMeshSurfaceTarget ToUniRcSurfaceSource(this Terrain terrain)
        {
            var mesh = terrain.terrainData.ToMesh(terrain.transform.position);
            return new UniRcNavMeshSurfaceTarget(terrain.name, mesh, terrain.transform.localToWorldMatrix);
        }
*/

        public static UniRcNavMeshSurfaceTarget ToCombinedNavMeshSurfaceTarget(this IList<UniRcNavMeshSurfaceTarget> sources, string name)
        {
            CombineInstance[] combineInstances = new CombineInstance[sources.Count];

            for (int i = 0; i < sources.Count; i++)
            {
                combineInstances[i].mesh = sources[i].GetMesh();
                combineInstances[i].transform = sources[i].GetMatrix4();
            }

            // Combine meshes with UInt32 index format
            Mesh combinedMesh = new Mesh();
            combinedMesh.indexFormat = IndexFormat.UInt32;
            combinedMesh.CombineMeshes(combineInstances, true, true, false);

            // // debug code
            // // Create a new GameObject to hold the combined mesh
            // var combinedMeshObject = new GameObject(name);
            // var meshFilter = combinedMeshObject.AddComponent<MeshFilter>();
            // var meshRenderer = combinedMeshObject.AddComponent<MeshRenderer>();
            //
            // // Assign the combined mesh to the new GameObject
            // meshFilter.sharedMesh = combinedMesh;

            return new UniRcNavMeshSurfaceTarget(name, combinedMesh, Matrix4x4.identity);
        }

        /*
        public static Mesh ToMesh(this TerrainData terrainData, Vector3 terrainPos)
        {
            int w = terrainData.heightmapResolution;
            int h = terrainData.heightmapResolution;
            Vector3 meshScale = terrainData.size;
            int tRes = (int)Mathf.Pow(2, 1);
            meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
            float[,] heights = terrainData.GetHeights(0, 0, w, h);

            w = (w - 1) / tRes + 1;
            h = (h - 1) / tRes + 1;

            Vector3[] vertices = new Vector3[w * h];
            int[] triangles = new int[(w - 1) * (h - 1) * 6];

            for (int z = 0; z < h; z++)
            {
                for (int x = 0; x < w; x++)
                {
                    vertices[z * w + x] = Vector3.Scale(meshScale, new Vector3(x, heights[z * tRes, x * tRes], z));
                }
            }

            int index = 0;

            // Build triangle indices: 3 indices into vertex array for each triangle
            for (int z = 0; z < h - 1; z++)
            {
                for (int x = 0; x < w - 1; x++)
                {
                    // For each grid cell output two triangles
                    triangles[index++] = (z * w) + x;
                    triangles[index++] = ((z + 1) * w) + x;
                    triangles[index++] = (z * w) + x + 1;

                    triangles[index++] = ((z + 1) * w) + x;
                    triangles[index++] = ((z + 1) * w) + x + 1;
                    triangles[index++] = (z * w) + x + 1;
                }
            }

            // Create a new mesh
            // Assign vertices and triangles to the mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            // Calculate normals and other mesh attributes if needed
            mesh.RecalculateNormals();

            return mesh;
        }
        */

        public static void SaveNavMeshFile(this DtNavMesh navMesh, string fileName)
        {
            using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            using var bw = new BinaryWriter(fs);

            var writer = new DtMeshSetWriter();
            writer.Write(bw, navMesh, RcByteOrder.LITTLE_ENDIAN, true);
        }
        
        public static DtNavMesh LoadNavMeshFile(string fileName)
        {
            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs);

            var reader = new DtMeshSetReader();
            return reader.Read(br, 6);
        }
    }
}