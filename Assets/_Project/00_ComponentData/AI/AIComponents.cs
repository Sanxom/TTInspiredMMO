using Unity.Entities;

namespace GameName.ComponentData
{
    /// <summary>
    /// AI controller configuration
    /// </summary>
    public struct AIControllerComponent : IComponentData
    {
        public float BiasFactor;      // 2.0-5.0, higher = more optimal
        public float MinWeight;       // Minimum action selection weight
        public float DecisionDelay;   // Seconds before deciding
        public float TimeSinceLastDecision;

        public AIPersonality Personality;
    }

    /// <summary>
    /// AI personality types
    /// </summary>
    public enum AIPersonality : byte
    {
        Balanced = 0,      // Bias 3.0
        Aggressive = 1,    // Bias 4.0, prefers attacks
        Defensive = 2,     // Bias 3.5, prefers buffs/heals
        Random = 3,        // Bias 2.0, chaotic
        Strategic = 4      // Bias 5.0, near-optimal
    }

    /// <summary>
    /// Available AI action
    /// </summary>
    public struct AIActionElement : IBufferElementData
    {
        public AIActionType ActionType;
        public int AbilityId;         // If action is cast ability
        public float BaseUtility;     // Starting utility value
    }

    /// <summary>
    /// Types of AI actions
    /// </summary>
    public enum AIActionType : byte
    {
        AttackWeakest = 0,
        AttackRandom = 1,
        AttackStrongest = 2,
        BuffSelf = 3,
        BuffAlly = 4,
        Heal = 5,
        Defend = 6,
        UseSpecialAbility = 7
    }

    /// <summary>
    /// Computed action weight (temporary)
    /// </summary>
    public struct AIActionWeightElement : IBufferElementData
    {
        public int ActionIndex;
        public float Utility;
        public float Weight;
    }

    /// <summary>
    /// Final AI decision
    /// </summary>
    public struct AIDecisionComponent : IComponentData
    {
        public int SelectedActionIndex;
        public Entity SelectedTarget;
        public bool DecisionReady;
        public float DecisionTime;
    }
}