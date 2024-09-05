using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ComponentsAndTags
{
    public readonly partial struct ZombieEatAspect : IAspect
    {
        public readonly Entity Entity;
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRW<ZombieTimer> _zombieTimer;
        private readonly RefRO<ZombieEatProperties> _zombieEatProperties;
        private readonly RefRO<ZombieHeading> _heading;

        private float EatDamagePerSecond => _zombieEatProperties.ValueRO.EatDamagePerSecond;
        private float EatAmplitude => _zombieEatProperties.ValueRO.EatAmplitude;
        private float EatFrequency => _zombieEatProperties.ValueRO.EatFrequency;
        private float Heading => _heading.ValueRO.Value;

        private float ZombieTimer
        {
            get => _zombieTimer.ValueRO.Value;
            set => _zombieTimer.ValueRW.Value = value;
        }

        public void Eat(float deltaTime, EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity brain)
        {
            ZombieTimer += deltaTime;

            float eatAngle = EatAmplitude * math.sin(EatFrequency * ZombieTimer);
            _transform.ValueRW.Rotation = quaternion.Euler(eatAngle, Heading, 0);

            float eatDamage = EatDamagePerSecond * deltaTime;
            BrainDamageBufferElement currentBrainDamage = new BrainDamageBufferElement()
            {
                Value = eatDamage
            };
            
            ecb.AppendToBuffer(sortKey, brain, currentBrainDamage);
        }

        public bool IsInEatingRange(float3 brainPosition, float brainRadiusSq)
        {
            return math.distance(brainPosition, _transform.ValueRO.Position) <= brainRadiusSq - 1;
        }

    }
}