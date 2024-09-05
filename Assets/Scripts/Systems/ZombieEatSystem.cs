using ComponentsAndTags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(ZombieWalkSystem))]
    public partial struct ZombieEatSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BrainTag>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            Entity brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
            float brainScale = SystemAPI.GetComponent<LocalTransform>(brainEntity).Scale;
            float brainRadius = brainScale * 5f + 1f;

            new ZombieEatJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BrainEntity = brainEntity,
                BrainRadiusSq = brainRadius * brainRadius
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ZombieEatJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        public Entity BrainEntity;
        public float BrainRadiusSq;

        [BurstCompile]
        private void Execute(ZombieEatAspect zombie, [ChunkIndexInQuery] int sortKey)
        {
            if (zombie.IsInEatingRange(float3.zero, BrainRadiusSq))
            {
                zombie.Eat(DeltaTime, ECB, sortKey, BrainEntity);
            }
            else
            {
                ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, false);
                ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
            }
        }
    }
}