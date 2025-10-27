using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Rolls initiative (d20 + Speed) for all combatants at combat start
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct InitiativeRollSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // Require BeginCombatTag to exist before running
            state.RequireForUpdate<BeginCombatTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Create random seed based on current tick
            var random = Random.CreateFromIndex((uint)state.WorldUnmanaged.Time.ElapsedTime);

            // Roll initiative for all combatants
            foreach (var (initiative, stats) in
                SystemAPI.Query<RefRW<InitiativeComponent>,
                               RefRO<StatsComponent>>()
                    .WithAll<InCombatTag>()
                    .WithNone<DefeatedTag>())
            {
                // Roll d20
                initiative.ValueRW.D20Roll = random.NextInt(1, 21);

                // Add speed modifier
                initiative.ValueRW.SpeedModifier = stats.ValueRO.Speed;

                // Calculate total
                initiative.ValueRW.CalculateTotal();
            }

            // Remove BeginCombatTag so system doesn't run again
            var beginCombatEntity = SystemAPI.GetSingletonEntity<BeginCombatTag>();
            state.EntityManager.RemoveComponent<BeginCombatTag>(beginCombatEntity);
        }
    }
}