using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Regenerates mana for all combatants at end of turn
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ManaRegenerationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TurnEndTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Regenerate mana for all living combatants
            foreach (var mana in
                SystemAPI.Query<RefRW<ManaComponent>>()
                    .WithAll<InCombatTag>()
                    .WithNone<DefeatedTag>())
            {
                // Add regen amount
                mana.ValueRW.Current += mana.ValueRO.RegenPerTurn;

                // Clamp to maximum
                mana.ValueRW.Current = math.min(
                    mana.ValueRW.Current,
                    mana.ValueRW.Maximum
                );
            }
        }
    }
}