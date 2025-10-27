using Unity.Entities;

namespace GameName.ComponentData
{
    /// <summary>
    /// Healing ability
    /// Heal Amount = Intelligence + Healing bonus
    /// </summary>
    public struct HealAbility : IComponentData
    {
        public int ManaCost;
        public int HealingBonus;
        public AbilityTargetType TargetType;
    }
}