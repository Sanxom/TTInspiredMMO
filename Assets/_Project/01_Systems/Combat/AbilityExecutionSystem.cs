using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using GameName.ComponentData;

namespace GameName.Systems
{
    /// <summary>
    /// Processes ability execution requests and creates damage requests
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct AbilityExecutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveCombatTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var worldName = state.WorldUnmanaged.Name.ToString();

            foreach (var (request, requestEntity) in
                SystemAPI.Query<RefRO<AbilityExecutionRequest>>()
                    .WithEntityAccess())
            {
                var caster = request.ValueRO.Caster;
                var target = request.ValueRO.Target;

                // Validate entities still exist and are alive
                if (!state.EntityManager.Exists(caster) ||
                    !state.EntityManager.Exists(target))
                {
                    ecb.DestroyEntity(requestEntity);
                    continue;
                }

                if (state.EntityManager.HasComponent<DefeatedTag>(caster) ||
                    state.EntityManager.HasComponent<DefeatedTag>(target))
                {
                    ecb.DestroyEntity(requestEntity);
                    continue;
                }

                // Get caster stats
                var stats = state.EntityManager.GetComponentData<StatsComponent>(caster);
                var mana = state.EntityManager.GetComponentData<ManaComponent>(caster);

                int damageAmount = 0;
                int manaCost = 0;
                DamageType damageType = DamageType.Physical;
                string abilityName = "";

                // Calculate damage based on ability type
                switch (request.ValueRO.AbilityType)
                {
                    case AbilityType.BasicAttack:
                        if (state.EntityManager.HasComponent<BasicAttackAbility>(caster))
                        {
                            var ability = state.EntityManager
                                .GetComponentData<BasicAttackAbility>(caster);
                            damageAmount = stats.Strength + ability.WeaponDamageBonus;
                            manaCost = ability.ManaCost;
                            damageType = DamageType.Physical;
                            abilityName = "Basic Attack";
                        }
                        break;

                    case AbilityType.Fireball:
                        if (state.EntityManager.HasComponent<FireballAbility>(caster))
                        {
                            var ability = state.EntityManager
                                .GetComponentData<FireballAbility>(caster);
                            damageAmount = (stats.Intelligence * 2) + ability.FireDamageBonus;
                            manaCost = ability.ManaCost;
                            damageType = DamageType.Fire;
                            abilityName = "Fireball";
                        }
                        break;

                    case AbilityType.Heal:
                        if (state.EntityManager.HasComponent<HealAbility>(caster))
                        {
                            var ability = state.EntityManager
                                .GetComponentData<HealAbility>(caster);
                            damageAmount = stats.Intelligence + ability.HealingBonus;
                            manaCost = ability.ManaCost;
                            damageType = DamageType.Healing;
                            abilityName = "Heal";
                        }
                        break;
                }

                // Check mana cost
                if (mana.Current < manaCost)
                {
                    Debug.Log($"[{worldName}] {state.EntityManager.GetName(caster)} " +
                        $"not enough mana for {abilityName}");
                    ecb.DestroyEntity(requestEntity);
                    continue;
                }

                // Deduct mana
                mana.Current -= manaCost;
                ecb.SetComponent(caster, mana);

                // Create damage request
                var damageRequestEntity = ecb.CreateEntity();
                ecb.AddComponent(damageRequestEntity, new DamageRequest
                {
                    Source = caster,
                    Target = target,
                    Amount = damageAmount,
                    DamageType = damageType
                });

                if (damageType == DamageType.Healing)
                {
                    Debug.Log($"[{worldName}] {state.EntityManager.GetName(caster)} " +
                        $"used {abilityName} on {state.EntityManager.GetName(target)} " +
                        $"for {damageAmount} healing (MP: {mana.Current + manaCost}->{mana.Current})");
                }
                else
                {
                    Debug.Log($"[{worldName}] {state.EntityManager.GetName(caster)} " +
                        $"used {abilityName} on {state.EntityManager.GetName(target)} " +
                        $"for {damageAmount} damage (MP: {mana.Current + manaCost}->{mana.Current})");
                }

                // Destroy request
                ecb.DestroyEntity(requestEntity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}