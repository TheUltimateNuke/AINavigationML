using AINavigationML.Utilities;
using DotRecast.Core.Numerics;
using DotRecast.Detour.Crowd;
using DotRecast.Detour.Extras.Unity.Astar;
using HarmonyLib;
using Il2CppFemur;
using UniRecast.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

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
        
        __instance.rawDirection = Vector3.zero;
        if (__instance.actor.bodyHandeler.agent.remainingDistance > 0.2f)
        {
            var dir = (__instance.actor.bodyHandeler.Agent.PartTransform.position - __instance.actor.bodyHandeler.Ball.PartTransform.position) * 100f;
            dir = Vector3.Lerp(vector, __instance.direction, 0.2f);
            __instance.direction = Vector3.Normalize(new Vector3(dir.x, 0f, dir.z));
            __instance.rawDirection = __instance.direction;
        }
        __instance.lookDirection = __instance.direction + new Vector3(0f, 0.2f, 0f);
        if (__instance.actor.targetingHandeler.upperIntrest != null)
            __instance.lookDirection = __instance.actor.targetingHandeler.upperIntrest.ClosestPoint(__instance.actor.bodyHandeler.Head.PartCollider.bounds.center) - __instance.actor.bodyHandeler.Head.PartCollider.bounds.center;
        
        var success = dtAgent.SetMoveTarget(vector.ToRightHand(), false);
        Entrypoint.Logger.Msg($"(Agent id: {dtAgent.CrowdAgent.idx}) " + "State: " + dtAgent.CrowdAgent.state + $" TARGET REF: {dtAgent.CrowdAgent.targetRef}");
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

[HarmonyPatch(typeof(GameObject), nameof(GameObject.SetActive))]
internal static class GameObject_SetActive_Patch
{
    private static void Postfix(GameObject __instance)
    {
        var bc = __instance.GetComponentsInChildren<BoxCollider>();
        var capC = __instance.GetComponentsInChildren<CapsuleCollider>();
        foreach (var c in capC)
        {
            capC.
        }
    }
}