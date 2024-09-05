using ComponentsAndTags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnTombStoneSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GraveyardProperties>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            Entity graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardProperties>();
            GraveyardAspect graveyard = SystemAPI.GetAspect<GraveyardAspect>(graveyardEntity);

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

           // NativeList<float3> spawnPoints = new NativeList<float3>(Allocator.Temp);
            float3 tombStoneOffSet = new float3(0f, -2f, 1f);

            BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
            ref ZombieSpawnPointsBlob spawnPoints = ref blobBuilder.ConstructRoot<ZombieSpawnPointsBlob>();
            BlobBuilderArray<float3> arrayBuilder = blobBuilder.Allocate(ref spawnPoints.Value, graveyard.NumberOfTombStonesToSpawn);
            
            for (int i = 0; i < graveyard.NumberOfTombStonesToSpawn; i++)
            {
                Entity newTombStone = ecb.Instantiate(graveyard.TombStonePrefab);
                LocalTransform newTombStoneTransform = graveyard.GetRandomTombStoneTransform();
                ecb.SetComponent(newTombStone,newTombStoneTransform);
                
                float3 newZombieSpawnPoint = newTombStoneTransform.Position + tombStoneOffSet;
                arrayBuilder[i] = newZombieSpawnPoint;
            }

            var blobAsset = blobBuilder.CreateBlobAssetReference<ZombieSpawnPointsBlob>(Allocator.Persistent);
            ecb.SetComponent(graveyardEntity, new ZombieSpawnPoints
            {
                Value =  blobAsset
            });
            blobBuilder.Dispose();
            
            ecb.Playback(state.EntityManager);
        }
    }
}