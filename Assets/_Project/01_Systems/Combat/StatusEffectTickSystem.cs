using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Ticks down status effect durations and removes expired effects
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct StatusEffectTickSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TurnEndTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Process all entities with status effects
            foreach (var statusEffectBuffer in
                SystemAPI.Query<DynamicBuffer<StatusEffectElement>>()
                    .WithAll<InCombatTag>())
            {
                // Create local reference to the buffer
                var localBuffer = statusEffectBuffer;

                // Iterate backwards so we can safely remove expired effects
                for (int i = localBuffer.Length - 1; i >= 0; i--)
                {
                    // Read the element
                    var effect = localBuffer[i];

                    // Decrement remaining turns
                    effect.RemainingTurns--;

                    // Check if expired
                    if (effect.HasExpired)
                    {
                        // Remove expired effect
                        localBuffer.RemoveAt(i);
                    }
                    else
                    {
                        // Write back the modified element using local buffer
                        localBuffer[i] = effect;
                    }
                }
            }

            // Remove TurnEndTag after processing
            var turnEndEntity = SystemAPI.GetSingletonEntity<TurnEndTag>();
            state.EntityManager.RemoveComponent<TurnEndTag>(turnEndEntity);
        }
    }
}