using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Manages turn order based on initiative values
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(InitiativeRollSystem))]
    public partial struct TurnOrderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCombatTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Check if anyone currently has active turn
            var hasActiveTurn = SystemAPI.HasSingleton<ActiveTurnTag>();

            if (!hasActiveTurn)
            {
                // Find combatant with highest initiative who hasn't acted
                Entity highestInitiativeEntity = Entity.Null;
                int highestInitiative = int.MinValue;

                foreach (var (initiative, combatant, entity) in
                    SystemAPI.Query<RefRO<InitiativeComponent>,
                                   RefRO<CombatantComponent>>()
                        .WithEntityAccess()
                        .WithAll<InCombatTag>()
                        .WithNone<ActiveTurnTag, DefeatedTag>())
                {
                    if (initiative.ValueRO.Total > highestInitiative)
                    {
                        highestInitiative = initiative.ValueRO.Total;
                        highestInitiativeEntity = entity;
                    }
                }

                // If we found someone, give them active turn
                if (highestInitiativeEntity != Entity.Null)
                {
                    state.EntityManager.AddComponent<ActiveTurnTag>(
                        highestInitiativeEntity);
                }
                else
                {
                    // No one left to act - round complete
                    // Remove all ActiveTurnTag components to reset
                    var query = SystemAPI.QueryBuilder()
                        .WithAll<ActiveTurnTag>()
                        .Build();
                    state.EntityManager.RemoveComponent<ActiveTurnTag>(query);

                    // Could trigger TurnEndTag here for round end logic
                }
            }
        }
    }
}