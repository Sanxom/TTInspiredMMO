using Unity.Entities;
using Unity.NetCode;

namespace GameName.ComponentData
{
    /// <summary>
    /// Individual status effect instance
    /// Dynamic buffer allows multiple effects per entity
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct StatusEffectElement : IBufferElementData
    {
        [GhostField] public StatusEffectType Type;
        [GhostField] public int RemainingTurns;
        [GhostField] public int StackCount;
        [GhostField] public float Magnitude;
        [GhostField] public int SourceEntityId;  // Network ID instead of Entity reference

        public bool HasExpired => RemainingTurns <= 0;
    }

    /// <summary>
    /// Cooldown tracking for abilities
    /// </summary>
    public struct CooldownComponent : IComponentData
    {
        public int AbilityId;
        public int RemainingTurns;

        public bool IsReady => RemainingTurns <= 0;

        public void StartCooldown(int turns)
        {
            RemainingTurns = turns;
        }

        public void Tick()
        {
            if (RemainingTurns > 0)
                RemainingTurns--;
        }
    }
}