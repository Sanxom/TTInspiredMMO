using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Applies damage from DamageRequests to targets
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AbilityExecutionSystem))]
    public partial struct DamageApplicationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCombatTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var worldName = state.WorldUnmanaged.Name.ToString();

            foreach (var (damageRequest, requestEntity) in
                SystemAPI.Query<RefRO<DamageRequest>>()
                    .WithEntityAccess())
            {
                var target = damageRequest.ValueRO.Target;
                var amount = damageRequest.ValueRO.Amount;
                var damageType = damageRequest.ValueRO.DamageType;

                // Validate target
                if (!state.EntityManager.Exists(target) ||
                    state.EntityManager.HasComponent<DefeatedTag>(target))
                {
                    ecb.DestroyEntity(requestEntity);
                    continue;
                }

                // Get target health
                var health = state.EntityManager.GetComponentData<HealthComponent>(target);
                int oldHealth = health.Current;

                // Apply damage or healing
                if (damageType == DamageType.Healing)
                {
                    health.Current += amount;  // Add for healing
                }
                else
                {
                    health.Current -= amount;  // Subtract for damage
                }

                health.Current = math.clamp(health.Current, 0, health.Maximum);

                ecb.SetComponent(target, health);

                string targetName = state.EntityManager.GetName(target);

                if (damageType == DamageType.Healing)
                {
                    Debug.Log($"[{worldName}] {targetName} healed for {amount}! " +
                        $"HP: {oldHealth}->{health.Current}");
                }
                else
                {
                    Debug.Log($"[{worldName}] {targetName} takes {amount} {damageType} damage! " +
                        $"HP: {oldHealth}->{health.Current}");
                }

                // Destroy request
                ecb.DestroyEntity(requestEntity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}