using GameName.ComponentData;
using Unity.Entities;
using UnityEngine;

namespace GameName.Systems
{
    /// <summary>
    /// Initializes the random seed singleton at the very start of the game
    /// Runs in InitializationSystemGroup before anything else
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial struct RandomSeedInitSystem : ISystem
    {
        private bool hasInitialized;

        public void OnCreate(ref SystemState state)
        {
            hasInitialized = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            if (hasInitialized) return;
            hasInitialized = true;

            // Get world name for logging
            string worldName = state.WorldUnmanaged.Name.ToString();

            // Check if seed singleton already exists
            if (!SystemAPI.HasSingleton<RandomSeedSingleton>())
            {
                // Create singleton with seed
                var seedEntity = state.EntityManager.CreateEntity();
                state.EntityManager.SetName(seedEntity, "RandomSeed");

                uint seed = (uint)System.DateTime.Now.Ticks;
                state.EntityManager.AddComponentData(seedEntity, new RandomSeedSingleton
                {
                    Seed = seed
                });

                Debug.Log($"[{worldName}] Initialized RandomSeed: {seed}");
            }
            else
            {
                // Seed already exists (created by other world)
                var seed = SystemAPI.GetSingleton<RandomSeedSingleton>().Seed;
                Debug.Log($"[{worldName}] Using existing RandomSeed: {seed}");
            }
        }
    }
}