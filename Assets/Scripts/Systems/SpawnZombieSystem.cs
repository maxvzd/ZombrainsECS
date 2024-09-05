using ComponentsAndTags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    public partial struct SpawnZombieSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            BeginInitializationEntityCommandBufferSystem.Singleton ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();

            new SpawnZombieJob
            {
                DeltaTime = deltaTime,
                ECB = ecb.CreateCommandBuffer(state.WorldUnmanaged)
            }.Schedule();
        }
    }

    [BurstCompile]
    public partial struct SpawnZombieJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer ECB;

        private void Execute(GraveyardAspect graveyard)
        {
            graveyard.ZombieSpawnTimer -= DeltaTime;

            if (!graveyard.TimeToSpawnZombie) return;
            if (!graveyard.ZombieSpawnPointInitialized()) return;

            graveyard.ZombieSpawnTimer = graveyard.ZombieSpawnRate;
            Entity newZombie = ECB.Instantiate(graveyard.ZombiePrefab);

            LocalTransform newZombieTransform = graveyard.GetZombieSpawnPointTransform();
            ECB.SetComponent(newZombie, newZombieTransform);

            float zombieHeading = MathHelpers.GetHeading(newZombieTransform.Position, graveyard.Position);
            ECB.SetComponent(newZombie, new ZombieHeading
            {
                Value = zombieHeading
            });
        }
    }
}