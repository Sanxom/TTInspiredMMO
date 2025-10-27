using Unity.Entities;
using Unity.Mathematics;

namespace GameName.ComponentData
{
    /// <summary>
    /// Equipped ability in hotbar slot
    /// </summary>
    public struct EquippedAbilityElement : IBufferElementData
    {
        public int AbilityId;          // Reference to ability data
        public int SlotIndex;          // 0-9 hotbar position
        public int CurrentCooldown;    // Remaining turns
        public MancerType MancerType;  // Which specialization

        public bool IsReady => CurrentCooldown <= 0;
    }

    /// <summary>
    /// Targeting data for ability execution
    /// </summary>
    public struct AbilityTargetComponent : IComponentData
    {
        public Entity PrimaryTarget;
        public TargetingType Type;
        public int MaxTargets;
        public float Range;
    }

    /// <summary>
    /// How ability selects targets
    /// </summary>
    public enum TargetingType : byte
    {
        SingleTarget = 0,      // One enemy
        MultiHit = 1,          // Same enemy multiple times
        MultiTarget = 2,       // Multiple different enemies
        AllEnemies = 3,        // Every enemy
        Self = 4,              // Caster only
        SingleAlly = 5,        // One party member
        AllAllies = 6          // Entire party
    }

    /// <summary>
    /// Active ability cast in progress
    /// </summary>
    public struct ActiveAbilityCastComponent : IComponentData
    {
        public Entity Caster;
        public int AbilityId;
        public float TimeStarted;
        public bool ExecutionComplete;
    }

    /// <summary>
    /// Mancer specialization progress
    /// </summary>
    public struct MancerProgressComponent : IComponentData
    {
        public MancerType Type;
        public int TotalXP;
        public int MasteryLevel;  // 0=Novice, 1=Adept, 2=Expert, etc.

        public float MasteryBonus => MasteryLevel * 0.05f; // 5% per level
    }
}