using Unity.Entities;
using Unity.Mathematics;

namespace ComponentsAndTags
{
    public struct GraveyardProperties : IComponentData
    {
        public float2 FieldDimensions;
        public int NumberOfTombStonesToSpawn;
        public Entity TombStonePrefab;
        public Entity ZombiePrefab;
        public float ZombieSpawnRate;
    }

    public struct ZombieSpawnTimer : IComponentData
    {
        public float Value;
    }
}