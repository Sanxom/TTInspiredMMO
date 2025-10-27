using Unity.Entities;
using UnityEngine;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Checks for victory/defeat conditions
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(HealthDeathCheckSystem))]
    public partial struct VictoryConditionSystem : ISystem
    {
        private bool combatEnded;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCombatTag>();
            combatEnded = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            if (combatEnded) return;

            var worldName = state.WorldUnmanaged.Name.ToString();

            // Count living players and enemies
            int livingPlayers = 0;
            int livingEnemies = 0;

            foreach (var combatant in
                SystemAPI.Query<RefRO<CombatantComponent>>()
                    .WithAll<InCombatTag>()
                    .WithNone<DefeatedTag>())
            {
                if (combatant.ValueRO.Team == CombatantTeam.Player)
                    livingPlayers++;
                else
                    livingEnemies++;
            }

            // Check victory conditions
            if (livingPlayers == 0)
            {
                Debug.Log($"[{worldName}] \n=== DEFEAT! All players defeated! ===");
                combatEnded = true;

                // Remove ActiveCombatTag to stop combat
                var combatController = SystemAPI.GetSingletonEntity<ActiveCombatTag>();
                state.EntityManager.RemoveComponent<ActiveCombatTag>(combatController);
            }
            else if (livingEnemies == 0)
            {
                Debug.Log($"[{worldName}] \n=== VICTORY! All enemies defeated! ===");
                combatEnded = true;

                // Remove ActiveCombatTag to stop combat
                var combatController = SystemAPI.GetSingletonEntity<ActiveCombatTag>();
                state.EntityManager.RemoveComponent<ActiveCombatTag>(combatController);
            }
        }
    }
}