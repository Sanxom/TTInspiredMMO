using Unity.Entities;

namespace GameName.ComponentData
{
    /// <summary>
    /// Request to execute an ability
    /// Created when entity wants to use ability
    /// Processed and destroyed by AbilityExecutionSystem
    /// </summary>
    public struct AbilityExecutionRequest : IComponentData
    {
        public Entity Caster;
        public Entity Target;
        public AbilityType AbilityType;
    }

    public enum AbilityType
    {
        BasicAttack,
        Fireball,
        Heal
    }
}