using Unity.Burst;
using Unity.Entities;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Checks for defeated combatants and marks them
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct HealthDeathCheckSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCombatTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            // Check all living combatants
            foreach (var (health, combatant, entity) in
                SystemAPI.Query<RefRW<HealthComponent>,
                               RefRW<CombatantComponent>>()
                    .WithEntityAccess()
                    .WithAll<InCombatTag>()
                    .WithNone<DefeatedTag>())
            {
                // Check if health dropped to 0 or below
                if (health.ValueRO.Current <= 0)
                {
                    // Clamp health to 0
                    health.ValueRW.Current = 0;

                    // Mark as not alive
                    combatant.ValueRW.IsAlive = false;

                    // Add DefeatedTag
                    ecb.AddComponent<DefeatedTag>(entity);

                    // Remove ActiveTurnTag if they had it
                    if (state.EntityManager.HasComponent<ActiveTurnTag>(entity))
                    {
                        ecb.RemoveComponent<ActiveTurnTag>(entity);
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}