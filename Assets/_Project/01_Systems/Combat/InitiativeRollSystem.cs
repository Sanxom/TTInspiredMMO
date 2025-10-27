using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Rolls initiative (d20 + Speed) for all combatants at combat start
    /// Uses shared RandomSeedSingleton to ensure Server and Client get same rolls
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct InitiativeRollSystem : ISystem
    {
        private Random random;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // Require both BeginCombatTag and RandomSeedSingleton
            state.RequireForUpdate<BeginCombatTag>();
            state.RequireForUpdate<RandomSeedSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Get seed from singleton (same for Server and Client)
            var seed = SystemAPI.GetSingleton<RandomSeedSingleton>().Seed;

            // Initialize random with shared seed
            random = Random.CreateFromIndex(seed);

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