using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Basic AI - enemies attack random player
    /// DEBUG VERSION with extensive logging
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TurnOrderSystem))]
    [UpdateBefore(typeof(TurnEndSimulationSystem))]
    public partial struct BasicAISystem : ISystem
    {
        private Unity.Mathematics.Random random;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCombatTag>();
            random = Unity.Mathematics.Random.CreateFromIndex(1234);
        }

        public void OnUpdate(ref SystemState state)
        {
            var worldName = state.WorldUnmanaged.Name.ToString();
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Debug.Log($"[{worldName}] BasicAISystem.OnUpdate() running...");

            // Find enemy with active turn who DOESN'T have HasActedThisRoundTag yet
            int foundCount = 0;
            foreach (var (combatant, entity) in
                SystemAPI.Query<RefRO<CombatantComponent>>()
                    .WithEntityAccess()
                    .WithAll<EnemyTag, ActiveTurnTag>()
                    .WithNone<DefeatedTag, HasActedThisRoundTag>())
            {
                foundCount++;
                Debug.Log($"[{worldName}] BasicAI found entity to act: {state.EntityManager.GetName(entity)}");

                // Get all living players
                var players = new NativeList<Entity>(Allocator.Temp);
                foreach (var playerEntity in
                    SystemAPI.QueryBuilder()
                        .WithAll<PlayerTag, InCombatTag>()
                        .WithNone<DefeatedTag>()
                        .Build()
                        .ToEntityArray(Allocator.Temp))
                {
                    players.Add(playerEntity);
                }

                if (players.Length == 0)
                {
                    Debug.Log($"[{worldName}] BasicAI: No living players found!");
                    players.Dispose();
                    ecb.Dispose();
                    return;
                }

                // Pick random player
                int targetIndex = random.NextInt(0, players.Length);
                var target = players[targetIndex];
                players.Dispose();

                // Create basic attack request using ECB
                var requestEntity = ecb.CreateEntity();
                ecb.AddComponent(requestEntity, new AbilityExecutionRequest
                {
                    Caster = entity,
                    Target = target,
                    AbilityType = AbilityType.BasicAttack
                });

                // NOW add HasActedThisRoundTag to mark that we've acted
                ecb.AddComponent<HasActedThisRoundTag>(entity);

                Debug.Log($"[{worldName}] BasicAI: {state.EntityManager.GetName(entity)} " +
                    $"attacks {state.EntityManager.GetName(target)}");
                Debug.Log($"[{worldName}] BasicAI: Adding HasActedThisRoundTag to {state.EntityManager.GetName(entity)}");

                // Only process once per frame
                break;
            }

            if (foundCount == 0)
            {
                Debug.Log($"[{worldName}] BasicAI: No entities found to act (either no ActiveTurnTag or already has HasActedThisRoundTag)");
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            Debug.Log($"[{worldName}] BasicAISystem.OnUpdate() complete");
        }
    }
}