using ComponentsAndTags;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace AuthoringAndMono
{
    public class GraveyardMono : MonoBehaviour
    {
        public float2 fieldDimensions;
        public int numberOfTombStonesToSpawn;
        public GameObject tombStonePrefab;
        public uint randomSeed;
        public GameObject zombiePrefab;
        public float zombieSpawnRate;
    }

    public class GraveyardBaker : Baker<GraveyardMono>
    {
        public override void Bake(GraveyardMono authoring)
        {
            Entity graveyardEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(graveyardEntity, new GraveyardProperties
            {
                FieldDimensions = authoring.fieldDimensions,
                NumberOfTombStonesToSpawn = authoring.numberOfTombStonesToSpawn,
                TombStonePrefab = GetEntity(authoring.tombStonePrefab, TransformUsageFlags.Dynamic),
                ZombiePrefab = GetEntity(authoring.zombiePrefab, TransformUsageFlags.Dynamic),
                ZombieSpawnRate = authoring.zombieSpawnRate
            });

            AddComponent(graveyardEntity, new GraveyardRandom
            {
                Value = Random.CreateFromIndex(authoring.randomSeed)
            });

            AddComponent<ZombieSpawnPoints>(graveyardEntity);
            AddComponent<ZombieSpawnTimer>(graveyardEntity);
        }
    }
}