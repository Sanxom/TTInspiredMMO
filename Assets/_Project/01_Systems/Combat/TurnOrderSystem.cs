using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Manages turn order based on initiative
    /// DEBUG VERSION with extensive logging
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(InitiativeRollSystem))]
    public partial struct TurnOrderSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCombatTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var worldName = state.WorldUnmanaged.Name.ToString();

            // If no one has active turn, assign next turn
            if (!SystemAPI.HasSingleton<ActiveTurnTag>())
            {
                // Count total combatants and those who have acted
                int totalCombatants = 0;
                int actedCombatants = 0;

                var allEntities = new NativeList<Entity>(Allocator.Temp);

                foreach (var entity in
                    SystemAPI.QueryBuilder()
                        .WithAll<InCombatTag>()
                        .WithNone<DefeatedTag>()
                        .Build()
                        .ToEntityArray(Allocator.Temp))
                {
                    totalCombatants++;
                    bool hasActed = state.EntityManager.HasComponent<HasActedThisRoundTag>(entity);
                    if (hasActed)
                    {
                        actedCombatants++;
                    }

                    allEntities.Add(entity);

                    Debug.Log($"[{worldName}] TurnOrder Check: {state.EntityManager.GetName(entity)} " +
                        $"HasActed={hasActed}");
                }

                Debug.Log($"[{worldName}] TurnOrder: {actedCombatants}/{totalCombatants} have acted");

                // Check if everyone has acted (new round needed)
                if (actedCombatants >= totalCombatants && totalCombatants > 0)
                {
                    // Everyone has acted - new round!
                    Debug.Log($"[{worldName}] === NEW ROUND: Removing all HasActedThisRoundTag ===");

                    // Remove HasActedThisRoundTag from everyone
                    foreach (var entity in allEntities)
                    {
                        if (state.EntityManager.HasComponent<HasActedThisRoundTag>(entity))
                        {
                            state.EntityManager.RemoveComponent<HasActedThisRoundTag>(entity);
                            Debug.Log($"[{worldName}] Removed HasActedThisRoundTag from {state.EntityManager.GetName(entity)}");
                        }
                    }

                    allEntities.Dispose();

                    // Don't assign new turn yet - let next frame do it
                    Debug.Log($"[{worldName}] Waiting for next frame to assign first turn of new round");
                    return;
                }

                allEntities.Dispose();

                // Find highest initiative without HasActedThisRoundTag
                var highestInitiative = int.MinValue;
                var highestEntity = Entity.Null;

                foreach (var (initiative, entity) in
                    SystemAPI.Query<RefRO<InitiativeComponent>>()
                        .WithEntityAccess()
                        .WithAll<InCombatTag>()
                        .WithNone<DefeatedTag, HasActedThisRoundTag>())
                {
                    Debug.Log($"[{worldName}] Candidate: {state.EntityManager.GetName(entity)} " +
                        $"Initiative={initiative.ValueRO.Total}");

                    if (initiative.ValueRO.Total > highestInitiative)
                    {
                        highestInitiative = initiative.ValueRO.Total;
                        highestEntity = entity;
                    }
                }

                // If found someone, give them active turn
                if (highestEntity != Entity.Null)
                {
                    Debug.Log($"[{worldName}] Assigning turn to {state.EntityManager.GetName(highestEntity)}");

                    state.EntityManager.AddComponent<ActiveTurnTag>(highestEntity);

                    // DO NOT add HasActedThisRoundTag here!
                    // Let the AI/Player system add it AFTER they act

                    var health = state.EntityManager.GetComponentData<HealthComponent>(highestEntity);
                    var mana = state.EntityManager.GetComponentData<ManaComponent>(highestEntity);

                    Debug.Log($"[{worldName}] >>> {state.EntityManager.GetName(highestEntity)}'s Turn " +
                        $"| HP: {health.Current}/{health.Maximum} " +
                        $"| MP: {mana.Current}/{mana.Maximum}");
                }
                else
                {
                    Debug.Log($"[{worldName}] No valid entity found for next turn!");
                }
            }
        }
    }
}