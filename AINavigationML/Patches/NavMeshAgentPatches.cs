using AINavigationML.Utilities;
using DotRecast.Core.Numerics;
using DotRecast.Detour.Crowd;
using DotRecast.Detour.Extras.Unity.Astar;
using HarmonyLib;
using Il2CppFemur;
using UniRecast.Core;
using UnityEngine;
using UnityEngine.AI;

namespace AINavigationML.Patches;

[HarmonyPatch(typeof(ControlHandeler_Computer), nameof(ControlHandeler_Computer.DirectionCheck))]
public static class ControlHandeler_UpdateAgentDirection_Patch
{
    private static bool Prefix(ControlHandeler_Computer __instance)
    {
        if (__instance.actor.bodyHandeler.agent.isOnNavMesh) return true;
        
        var dtAgent =  __instance.actor.bodyHandeler.agent.GetComponent<UnityDtCrowdAgent>();
        if (dtAgent == null || dtAgent.CrowdAgent == null) return true;
        
        var vector = __instance.actor.bodyHandeler.Ball.PartTransform.position;
        var vector2 = new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0f, UnityEngine.Random.Range(-0.2f, 0.2f));
        if (__instance.actor.actorState != Actor.ActorState.Unconscious)
        {
            var behavior = __instance.actor.targetingHandeler.Behavior;
            if (behavior != TargetingHandeler.TargettingBehavior.targetBeasts)
            {
                if (behavior == TargetingHandeler.TargettingBehavior.targetCustom && __instance.actor.targetingHandeler.TargetOverride != null)
                {
                    vector = __instance.actor.targetingHandeler.TargetOverride.position + vector2;
                }
            }
            else
            {
                __instance.actor.bodyHandeler.agent.enabled = true;
                if (!__instance.actor.bodyHandeler.leftGrabJoint && !__instance.actor.bodyHandeler.rightGrabJoint)
                {
                    if (__instance.actor.targetingHandeler.actorIntrest != null)
                    {
                        var vector3 = __instance.actor.targetingHandeler.actorIntrest.position + vector2;
                        var num = Vector3.Distance(__instance.actor.bodyHandeler.Ball.PartTransform.position, vector3);
                        if ((__instance.armActionDelay <= __instance._punchFollowTime || __instance.lift || num > __instance._punchFollowDistance) && num > __instance._punchreachDistance)
                        {
                            vector = vector3;
                        }
                    }
                }
                else if (__instance.actor.targetingHandeler.throwTarget != null)
                {
                    vector = __instance.actor.targetingHandeler.throwTarget.transform.position + vector2;
                }
            }
        }
        
        var success = dtAgent.SetMoveTarget(vector.ToRightHand(), false);

        __instance.direction =
            -new Vector3(dtAgent.CrowdAgent.npos.X, dtAgent.CrowdAgent.npos.Y, dtAgent.CrowdAgent.npos.Z).normalized;
        Entrypoint.Logger.Msg($"(Agent id: {dtAgent.CrowdAgent.idx}) " + "State: " + dtAgent.CrowdAgent.state);
        //Entrypoint.Logger.Msg($"(Agent id: {dtAgent.CrowdAgent.idx}) " + "SetMoveTarget result: " + success);
        return false;
    }
}

[HarmonyPatch(typeof(Actor), nameof(Actor.Start))]
internal static class Actor_Awake_Patch
{
    private static void Postfix(Actor __instance)
    {
        if (!__instance.IsAI) return;
        
        var agentObj = __instance.bodyHandeler?.Agent?.PartTransform?.gameObject;
        if (agentObj == null) return;
        var agent = agentObj.AddComponent<UnityDtCrowdAgent>();
        agent.actorOwner = __instance;
    }
}