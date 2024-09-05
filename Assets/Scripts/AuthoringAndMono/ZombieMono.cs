using ComponentsAndTags;
using Unity.Entities;
using UnityEngine;

namespace AuthoringAndMono
{
    public class ZombieMono : MonoBehaviour
    {
        public float RiseRate;
        public float WalkSpeed;
        public float WalkAmplitude;
        public float WalkFrequency;

        public float EatDamage;
        public float EatAmplitude;
        public float EatFrequency;
    }

    public class ZombieBaker : Baker<ZombieMono>
    {
        public override void Bake(ZombieMono authoring)
        {
            Entity zombie = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(zombie, new ZombieRiseRate
            {
                Value = authoring.RiseRate
            });
            
            AddComponent(zombie, new ZombieWalkProperties
            {
                WalkSpeed = authoring.WalkSpeed,
                WalkAmplitude = authoring.WalkAmplitude,
                WalkFrequency = authoring.WalkFrequency
            });
            
            AddComponent(zombie, new ZombieEatProperties
            {
                EatDamagePerSecond = authoring.EatDamage,
                EatFrequency = authoring.EatFrequency,
                EatAmplitude = authoring.EatAmplitude
            });
            
            AddComponent<ZombieTimer>(zombie);
            AddComponent<ZombieHeading>(zombie);
            AddComponent<NewZombieTag>(zombie);
        }
    }
}