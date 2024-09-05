using System;
using Unity.Burst;
using Unity.Entities;

namespace ComponentsAndTags
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial struct BrainDamageSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (BrainAspect brain in SystemAPI.Query<BrainAspect>())
            {
                brain.DamageBrain();
            }
        }
    }
}