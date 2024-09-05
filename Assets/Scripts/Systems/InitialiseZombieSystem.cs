using ComponentsAndTags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct InitialiseZombieSystem : ISystem
    {
        [BurstCompile] 
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (ZombieWalkAspect zombie in SystemAPI.Query<ZombieWalkAspect>().WithAll<NewZombieTag>())
            {
                ecb.RemoveComponent<NewZombieTag>(zombie.Entity);
                ecb.SetComponentEnabled<ZombieWalkProperties>(zombie.Entity, false);
                ecb.SetComponentEnabled<ZombieEatProperties>(zombie.Entity, false);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}