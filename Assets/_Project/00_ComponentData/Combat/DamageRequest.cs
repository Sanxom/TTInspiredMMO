using Unity.Entities;

namespace GameName.ComponentData
{
    /// <summary>
    /// Request to apply damage to an entity
    /// Created by ability/attack systems
    /// Processed by DamageApplicationSystem
    /// </summary>
    public struct DamageRequest : IComponentData
    {
        public Entity Source;
        public Entity Target;
        public int Amount;
        public DamageType DamageType;
    }
}