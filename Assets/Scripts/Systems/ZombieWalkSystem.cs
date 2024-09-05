using ComponentsAndTags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(ZombieRiseSystem))]
    public partial struct ZombieWalkSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BrainTag>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
            float brainScale = SystemAPI.GetComponent<LocalTransform>(brainEntity).Scale;
            float brainRadius = brainScale * 5f + 0.5f;
            
            new ZombieWalkJob
            {
                DeltaTime = deltaTime,
                BrainRadiusSq = brainRadius * brainRadius,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()

            }.ScheduleParallel();
        }
    }
    
    [BurstCompile]
    public partial struct ZombieWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float BrainRadiusSq;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(ZombieWalkAspect zombie, [ChunkIndexInQuery]int sortKey)
        {
            zombie.Walk(DeltaTime);
            if (zombie.IsInStoppingRange(float3.zero, BrainRadiusSq))
            {
                ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, false);
                ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, true);
            }
        }
    }
}