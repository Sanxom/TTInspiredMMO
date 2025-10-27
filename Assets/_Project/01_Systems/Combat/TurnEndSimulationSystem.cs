using Unity.Entities;
using UnityEngine;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Simulates turn ending for testing
    /// In real game, this would be player/AI action system
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