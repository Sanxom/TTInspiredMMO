using Unity.Entities;
using Unity.NetCode;

namespace GameName.ComponentData
{
    /// <summary>
    /// Health component - synchronized over network
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct HealthComponent : IComponentData
    {
        [GhostField(Quantization = 0)]
        public int Current;

        [GhostField(Quantization = 0)]
        public int Maximum;

        public float Percentage => Maximum > 0
            ? (float)Current / Maximum
            : 0f;

        public bool IsAlive => Current > 0;
    }

    /// <summary>
    /// Mana component - synchronized over network
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct ManaComponent : IComponentData
    {
        [GhostField(Quantization = 0)]
        public int Current;

        [GhostField(Quantization = 0)]
        public int Maximum;

        [GhostField(Quantization = 0)]
        public int RegenPerTurn;

        public float Percentage => Maximum > 0
            ? (float)Current / Maximum
            : 0f;
    }

    /// <summary>
    /// Core character statistics
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct StatsComponent : IComponentData
    {
        // Primary stats
        [GhostField] public int Strength;      // Physical damage
        [GhostField] public int Intelligence;  // Magical damage
        [GhostField] public int Dexterity;     // Defense, evasion
        [GhostField] public int Speed;         // Initiative modifier
        [GhostField] public int Luck;          // Critical chance

        // Derived stats (calculated, not synchronized)
        public int PhysicalPower => Strength * 2;
        public int MagicalPower => Intelligence * 2;
        public int Defense => Dexterity;
        public float CritChance => Luck / 100f;
    }

    /// <summary>
    /// Combat positioning and state
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct CombatantComponent : IComponentData
    {
        [GhostField] public int PositionIndex;  // 0-3 players, 0-7 enemies
        [GhostField] public CombatantTeam Team;
        [GhostField] public bool IsAlive;

        public CharacterRole Role; // Not synced, client-side only
    }

    /// <summary>
    /// Initiative for turn order
    /// </summary>
    [GhostComponent(PrefabType = GhostPrefabType.All)]
    public struct InitiativeComponent : IComponentData
    {
        [GhostField] public int D20Roll;        // 1-20
        [GhostField] public int SpeedModifier;  // From stats
        [GhostField] public int Total;          // D20Roll + SpeedModifier

        public void CalculateTotal()
        {
            Total = D20Roll + SpeedModifier;
        }
    }

    /// <summary>
    /// Level and experience
    /// </summary>
    public struct LevelComponent : IComponentData
    {
        public int CurrentLevel;
        public int CurrentXP;
        public int XPToNextLevel;

        public float XPPercentage => XPToNextLevel > 0
            ? (float)CurrentXP / XPToNextLevel
            : 0f;
    }
}