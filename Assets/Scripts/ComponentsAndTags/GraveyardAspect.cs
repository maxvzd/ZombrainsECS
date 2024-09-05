﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ComponentsAndTags
{
    public readonly partial struct GraveyardAspect : IAspect
    {
        private const float BRAIN_SAFETY_RADIUS_SQ = 100;
       
        private readonly RefRO<LocalTransform> _transform;
        private LocalTransform Transform => _transform.ValueRO;
        private readonly RefRO<GraveyardProperties> _graveyardProperties;
        private readonly RefRW<GraveyardRandom> _graveyardRandom;
        private readonly RefRW<ZombieSpawnPoints> _zombieSpawnPoints;
        private int ZombieSpawnPointCount => _zombieSpawnPoints.ValueRO.Value.Value.Value.Length;
        private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;

        public readonly Entity Entity;
        public int NumberOfTombStonesToSpawn => _graveyardProperties.ValueRO.NumberOfTombStonesToSpawn;
        public Entity TombStonePrefab => _graveyardProperties.ValueRO.TombStonePrefab;
        public float3 Position => _transform.ValueRO.Position;
        
        public float ZombieSpawnTimer
        {
            get => _zombieSpawnTimer.ValueRO.Value;
            set => _zombieSpawnTimer.ValueRW.Value = value;
        }

        public bool TimeToSpawnZombie => ZombieSpawnTimer <= 0f;

        public BlobArray<float3> ZombieSpawnPoints
        {
            get => _zombieSpawnPoints.ValueRO.Value.Value.Value;
            set => _zombieSpawnPoints.ValueRW.Value.Value.Value = value;
        }

        public float ZombieSpawnRate => _graveyardProperties.ValueRO.ZombieSpawnRate;
        public Entity ZombiePrefab => _graveyardProperties.ValueRO.ZombiePrefab;

        public bool ZombieSpawnPointInitialized()
        {
            return _zombieSpawnPoints.ValueRO.Value.IsCreated && ZombieSpawnPointCount > 0;
        }

        public LocalTransform GetRandomTombStoneTransform()
        {
            return new LocalTransform()
            {
                Position = GetRandomPosition(),
                Rotation = GetRandomRotation(),
                Scale = GetRandomScale(0.5f)
            };
        }
        public LocalTransform GetZombieSpawnPointTransform()
        {
            float3 position = GetRandomZombieSpawnPoint();
            
            return new LocalTransform
            {
                Position = position,
                Rotation = quaternion.RotateY(MathHelpers.GetHeading(position, Transform.Position)),
                Scale = 1f
            };
        }
        
        private float3 GetRandomPosition()
        {
            float3 randomPosition;

            do
            {
                randomPosition = _graveyardRandom.ValueRW.Value.NextFloat3(MinCorner, MaxCorner);
            } while (math.distancesq(_transform.ValueRO.Position, randomPosition) <= BRAIN_SAFETY_RADIUS_SQ);

            return randomPosition;
        }

        private float3 MinCorner => _transform.ValueRO.Position - HalfDimensions;
        private float3 MaxCorner => _transform.ValueRO.Position + HalfDimensions;

        private float3 HalfDimensions => new()
        {
            x = _graveyardProperties.ValueRO.FieldDimensions.x * 0.5f,
            y = 0f,
            z = _graveyardProperties.ValueRO.FieldDimensions.y * 0.5f
        };

        private quaternion GetRandomRotation() => quaternion.RotateY(_graveyardRandom.ValueRW.Value.NextFloat(-0.25f, 0.25f));
        private float GetRandomScale(float min) => _graveyardRandom.ValueRW.Value.NextFloat(min, 1f);
        
        private float3 GetRandomZombieSpawnPoint()
        {
            return GetZombieSpawnPoint(_graveyardRandom.ValueRW.Value.NextInt(ZombieSpawnPointCount));
        }

        private float3 GetZombieSpawnPoint(int i) => _zombieSpawnPoints.ValueRO.Value.Value.Value[i];
    }
}