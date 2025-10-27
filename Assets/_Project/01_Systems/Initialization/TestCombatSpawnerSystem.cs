using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Spawns test entities for combat prototype
    /// Only runs once at game start
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct TestCombatSpawnerSystem : ISystem
    {
        private bool hasSpawned;

        public void OnCreate(ref SystemState state)
        {
            hasSpawned = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            // Only spawn once
            if (hasSpawned) return;
            hasSpawned = true;

            var em = state.EntityManager;

            // Get world name for logging
            string worldName = state.WorldUnmanaged.Name.ToString();

            Debug.Log($"[{worldName}] === SPAWNING TEST COMBAT ENTITIES ===");

            // Spawn 2 Players
            for (int i = 0; i < 2; i++)
            {
                var playerEntity = em.CreateEntity();
                em.SetName(playerEntity, $"Player_{i}");

                em.AddComponentData(playerEntity, new HealthComponent
                {
                    Current = 100,
                    Maximum = 100
                });

                em.AddComponentData(playerEntity, new ManaComponent
                {
                    Current = 50,
                    Maximum = 50,
                    RegenPerTurn = 5
                });

                em.AddComponentData(playerEntity, new StatsComponent
                {
                    Strength = 10,
                    Intelligence = 15,
                    Dexterity = 8,
                    Speed = 12,
                    Luck = 10
                });

                em.AddComponentData(playerEntity, new CombatantComponent
                {
                    PositionIndex = i,
                    Team = CombatantTeam.Player,
                    IsAlive = true
                });

                em.AddComponentData(playerEntity, new InitiativeComponent());
                em.AddComponent<PlayerTag>(playerEntity);
                em.AddComponent<InCombatTag>(playerEntity);

                Debug.Log($"[{worldName}] Spawned {em.GetName(playerEntity)}");
            }

            // Spawn 3 Enemies
            for (int i = 0; i < 3; i++)
            {
                var enemyEntity = em.CreateEntity();
                em.SetName(enemyEntity, $"Enemy_{i}");

                em.AddComponentData(enemyEntity, new HealthComponent
                {
                    Current = 80,
                    Maximum = 80
                });

                em.AddComponentData(enemyEntity, new ManaComponent
                {
                    Current = 30,
                    Maximum = 30,
                    RegenPerTurn = 3
                });

                em.AddComponentData(enemyEntity, new StatsComponent
                {
                    Strength = 8,
                    Intelligence = 10,
                    Dexterity = 6,
                    Speed = 10,
                    Luck = 5
                });

                em.AddComponentData(enemyEntity, new CombatantComponent
                {
                    PositionIndex = i,
                    Team = CombatantTeam.Enemy,
                    IsAlive = true
                });

                em.AddComponentData(enemyEntity, new InitiativeComponent());
                em.AddComponent<EnemyTag>(enemyEntity);
                em.AddComponent<InCombatTag>(enemyEntity);

                Debug.Log($"[{worldName}] Spawned {em.GetName(enemyEntity)}");
            }

            // Create singleton entity to trigger combat start
            var combatController = em.CreateEntity();
            em.SetName(combatController, "CombatController");
            em.AddComponent<ActiveCombatTag>(combatController);
            em.AddComponent<BeginCombatTag>(combatController);

            Debug.Log($"[{worldName}] === COMBAT ENTITIES SPAWNED ===");
        }
    }
}