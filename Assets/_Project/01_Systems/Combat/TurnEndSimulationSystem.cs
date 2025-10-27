using Unity.Entities;
using UnityEngine;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Simulates turn ending for testing
    /// Auto-ends turns for BOTH players and enemies
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CombatDebugSystem))]
    public partial struct TurnEndSimulationSystem : ISystem
    {
        private float timeSinceTurnStart;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCombatTag>();
            timeSinceTurnStart = 0f;
        }

        public void OnUpdate(ref SystemState state)
        {
            // Get world name for logging
            string worldName = state.WorldUnmanaged.Name.ToString();

            // If someone has active turn
            if (SystemAPI.HasSingleton<ActiveTurnTag>())
            {
                timeSinceTurnStart += SystemAPI.Time.DeltaTime;

                // After 1 second, end their turn
                if (timeSinceTurnStart >= 1f)
                {
                    timeSinceTurnStart = 0f;

                    // Find who has active turn
                    foreach (var entity in
                        SystemAPI.QueryBuilder()
                            .WithAll<ActiveTurnTag>()
                            .Build()
                            .ToEntityArray(Unity.Collections.Allocator.Temp))
                    {
                        string name = state.EntityManager.GetName(entity);

                        // Check if it's a player who hasn't acted
                        bool isPlayer = state.EntityManager.HasComponent<PlayerTag>(entity);
                        bool hasActed = state.EntityManager.HasComponent<HasActedThisRoundTag>(entity);

                        // If player hasn't acted yet, add HasActedThisRoundTag
                        // (Enemies already have it from BasicAISystem)
                        if (isPlayer && !hasActed)
                        {
                            state.EntityManager.AddComponent<HasActedThisRoundTag>(entity);
                            Debug.Log($"[{worldName}] {name} (player) auto-acted, added HasActedThisRoundTag");
                        }

                        Debug.Log($"[{worldName}] {name} ended turn\n");

                        // Remove ActiveTurnTag to end turn
                        state.EntityManager.RemoveComponent<ActiveTurnTag>(entity);

                        // Create TurnEndTag for end-of-turn systems
                        var turnEndEntity = state.EntityManager.CreateEntity();
                        state.EntityManager.AddComponent<TurnEndTag>(turnEndEntity);
                    }
                }
            }
        }
    }
}