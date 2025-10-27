using Unity.Entities;

namespace GameName.ComponentData
{
    /// <summary>
    /// Team affiliation for combat
    /// </summary>
    public enum CombatantTeam : byte
    {
        Player = 0,
        Enemy = 1
    }

    /// <summary>
    /// Character role archetypes
    /// </summary>
    public enum CharacterRole : byte
    {
        DPS = 0,
        Tank = 1,
        Healer = 2,
        Support = 3
    }

    /// <summary>
    /// All 12 Mancer specializations
    /// </summary>
    public enum MancerType : byte
    {
        None = 0,
        Pyromancer = 1,
        Hydromancer = 2,
        Cryomancer = 3,
        Aeromancer = 4,
        Geomancer = 5,
        Luxomancer = 6,
        Umbramancer = 7,
        Electromancer = 8,
        Biomancer = 9,
        Necromancer = 10,
        Psychomancer = 11,
        Chronomancer = 12
    }

    /// <summary>
    /// Status effect types (buffs/debuffs)
    /// </summary>
    public enum StatusEffectType : byte
    {
        // Damage over time
        Burn = 0,
        Poison = 1,
        Bleed = 2,

        // Healing over time
        Regeneration = 10,

        // Crowd control
        Freeze = 20,
        Stun = 21,
        Slow = 22,

        // Buffs
        AttackUp = 30,
        DefenseUp = 31,
        SpeedUp = 32,

        // Debuffs
        AttackDown = 40,
        DefenseDown = 41,
        SpeedDown = 42,

        // Protection
        Shield = 50,
        Immunity = 51
    }

    /// <summary>
    /// Damage type for resistances
    /// </summary>
    public enum DamageType : byte
    {
        Physical = 0,
        Fire = 1,
        Water = 2,
        Ice = 3,
        Lightning = 4,
        Earth = 5,
        Light = 6,
        Shadow = 7,
        Nature = 8,
        Death = 9,
        Psychic = 10,
        Time = 11,
        Healing = 12
    }

    /// <summary>
    /// Combat constants
    /// </summary>
    public static class CombatConstants
    {
        public const int MaxPlayersInCombat = 4;
        public const int MaxEnemiesInCombat = 8;
        public const int MaxAbilitySlots = 10;
        public const int MaxStatusEffects = 10;
        public const float TurnDecisionTime = 8f;
        public const int InitiativeDiceSize = 20; // d20
    }
}