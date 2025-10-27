using Unity.Entities;
using UnityEngine;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Logs combat state for debugging
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(InitiativeRollSystem))]
    public partial struct CombatDebugSystem : ISystem
    {
        private bool hasLoggedInitiative;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCombatTag>();
            hasLoggedInitiative = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            // Get world name for logging
            string worldName = state.WorldUnmanaged.Name.ToString();

            // Log initiative rolls once
            if (!hasLoggedInitiative &&
                !SystemAPI.HasSingleton<BeginCombatTag>())
            {
                hasLoggedInitiative = true;

                Debug.Log($"\n[{worldName}] === INITIATIVE ROLLS ===");

                foreach (var (initiative, combatant, entity) in
                    SystemAPI.Query<RefRO<InitiativeComponent>,
                                   RefRO<CombatantComponent>>()
                        .WithEntityAccess()
                        .WithAll<InCombatTag>())
                {
                    string name = state.EntityManager.GetName(entity);
                    string team = combatant.ValueRO.Team == CombatantTeam.Player
                        ? "Player"
                        : "Enemy";

                    Debug.Log($"[{worldName}] {name} ({team}): " +
                        $"d20={initiative.ValueRO.D20Roll}, " +
                        $"Speed={initiative.ValueRO.SpeedModifier}, " +
                        $"Total={initiative.ValueRO.Total}");
                }
            }

            // Log active turn
            if (SystemAPI.HasSingleton<ActiveTurnTag>())
            {
                foreach (var entity in
                    SystemAPI.QueryBuilder()
                        .WithAll<ActiveTurnTag>()
                        .Build()
                        .ToEntityArray(Unity.Collections.Allocator.Temp))
                {
                    var health = state.EntityManager
                        .GetComponentData<HealthComponent>(entity);
                    var mana = state.EntityManager
                        .GetComponentData<ManaComponent>(entity);

                    string name = state.EntityManager.GetName(entity);

                    Debug.Log($"\n[{worldName}] >>> {name}'s Turn | " +
                        $"HP: {health.Current}/{health.Maximum} | " +
                        $"MP: {mana.Current}/{mana.Maximum}");
                }
            }
        }
    }
}