using Unity.Entities;

namespace GameName.ComponentData
{
    /// <summary>
    /// Basic melee/ranged attack ability
    /// Damage = Strength + Weapon bonus
    /// </summary>
    public struct BasicAttackAbility : IComponentData
    {
        public int ManaCost;
        public int WeaponDamageBonus;
        public AbilityTargetType TargetType;
    }

    public enum AbilityTargetType
    {
        SingleEnemy,
        SingleAlly,
        AllEnemies,
        AllAllies,
        Self
    }
}