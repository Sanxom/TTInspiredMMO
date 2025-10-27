using Unity.Entities;

namespace GameName.ComponentData
{
    /// <summary>
    /// Magical fire attack
    /// Damage = Intelligence * 2 + Fire bonus
    /// </summary>
    public struct FireballAbility : IComponentData
    {
        public int ManaCost;
        public int FireDamageBonus;
        public AbilityTargetType TargetType;
    }
}