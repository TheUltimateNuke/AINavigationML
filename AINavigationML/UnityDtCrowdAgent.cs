using System.Collections;
using AINavigationML.Utilities;
using DotRecast.Core.Numerics;
using DotRecast.Detour;
using DotRecast.Detour.Crowd;
using Il2CppFemur;
using MelonLoader;
using UniRecast.Core;
using UnityEngine;
using UnityEngine.AI;

namespace AINavigationML;

[RegisterTypeInIl2Cpp]
public class UnityDtCrowdAgent : MonoBehaviour
{
    private static DtCrowd? crowdManager => Entrypoint.dtCrowdManager;
    
    public Actor? actorOwner;
    private NavMeshAgent? unityAgent => actorOwner?.bodyHandeler?.agent;
    public Transform? ballTransform => actorOwner?.bodyHandeler?.Ball.PartTransform;
    public DtCrowdAgent? CrowdAgent;
    
    private static RcVec3f CalcVel(RcVec3f pos, RcVec3f tgt, float speed)
    {
        RcVec3f vel = RcVec3f.Subtract(tgt, pos);
        vel.Y = 0.0f;
        vel = RcVec3f.Normalize(vel);
        vel *= speed;
        return vel;
    }

    public static bool Query(RcVec3f pos, RcVec3f ext, IDtQueryFilter filter, out long nearestRef, out RcVec3f nearestPt, out bool isOverPoly)
    {
        return crowdManager.GetNavMeshQuery().FindNearestPoly(pos, ext, filter, out nearestRef, out nearestPt, out isOverPoly).Succeeded();
    }

    public bool SetMoveTarget(RcVec3f pos, bool adjust)
    {
        if (crowdManager == null || CrowdAgent == null) return false;
        var ext = crowdManager.GetQueryExtents();
        var filter = crowdManager.GetFilter(0);
        var ret = false;
        var nearestRef = 0L;
        if (adjust)
        {
            if (CrowdAgent == null)
            {
                foreach (var ag in crowdManager.GetActiveAgents())
                {
                    var vel = CalcVel(ag.npos, pos, ag.option.maxSpeed);
                    ret = crowdManager.RequestMoveVelocity(ag, vel);
                }
            }
            else
            {
                var vel = CalcVel(CrowdAgent.npos, pos, CrowdAgent.option.maxSpeed);
                ret = crowdManager.RequestMoveVelocity(CrowdAgent, vel);
            }
        }
        else
        {
            var retPoly = Query(pos, ext, filter, out nearestRef, out var nearestPt, out _);
            Entrypoint.Logger.Msg($"(agent id {CrowdAgent.idx}) FindNearestPoly Success: {retPoly} REF {nearestRef}");
            if (CrowdAgent == null)
            {
                foreach (var ag in crowdManager.GetActiveAgents())
                {
                    ret = crowdManager.RequestMoveTarget(ag, nearestRef, nearestPt);
                }
            }
            else
            {
                ret = crowdManager.RequestMoveTarget(CrowdAgent, nearestRef, nearestPt);
            }
        }

        return ret && nearestRef != 0L;
    }

    private IEnumerator StartCoroutineVersion()
    {
        if (crowdManager == null || unityAgent == null) yield break;
        long nearestRef = 0;
        while (!Query(transform.position.ToRightHand(), crowdManager.GetQueryExtents(), new DtQueryDefaultFilter(),
                   out nearestRef, out _, out _) || nearestRef == 0)
        {
            yield return null;
        }
        CrowdAgent = crowdManager.AddAgent(transform.position.ToRightHand(), new DtCrowdAgentParams
        {
            radius = 0.1f,
            height = 1.4f,
            maxSpeed = 16,
            maxAcceleration = 8,
            pathOptimizationRange = 30f,
            collisionQueryRange = 12f,
            updateFlags = DtCrowdAgentUpdateFlags.DT_CROWD_ANTICIPATE_TURNS | DtCrowdAgentUpdateFlags.DT_CROWD_OBSTACLE_AVOIDANCE
        });
    }
    
    private void Start()
    {
        MelonCoroutines.Start(StartCoroutineVersion());
    }

    private void Update()
    {
        if (CrowdAgent == null || unityAgent == null || actorOwner == null) return;
        actorOwner.bodyHandeler.agent.transform.position = new Vector3(-CrowdAgent.npos.X, CrowdAgent.npos.Y, CrowdAgent.npos.Z);
        //actorOwner.bodyHandeler.agent.velocity = new Vector3(-CrowdAgent.vel.X, CrowdAgent.vel.Y, CrowdAgent.vel.Z);
    }

    private void OnDestroy()
    {
        crowdManager?.RemoveAgent(CrowdAgent);
        CrowdAgent = null;
    }
}