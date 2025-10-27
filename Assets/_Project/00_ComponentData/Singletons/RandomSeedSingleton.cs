using Unity.Entities;

namespace GameName.ComponentData
{
    /// <summary>
    /// Singleton component that holds the random seed for the current game session
    /// Ensures Server and Client use the same seed for deterministic gameplay
    /// </summary>
    public struct RandomSeedSingleton : IComponentData
    {
        public uint Seed;
    }
}