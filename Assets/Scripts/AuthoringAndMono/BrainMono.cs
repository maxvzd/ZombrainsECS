using ComponentsAndTags;
using Unity.Entities;
using UnityEngine;

namespace AuthoringAndMono
{
    public class BrainMono : MonoBehaviour
    {
        public float BrainHealth;
    }

    public class BrainBaker : Baker<BrainMono>
    {
        public override void Bake(BrainMono authoring)
        {
            Entity brain = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BrainTag>(brain);
            AddComponent(brain, new BrainHealth
            {
                Max = authoring.BrainHealth,
                Value = authoring.BrainHealth
            });

            AddBuffer<BrainDamageBufferElement>(brain);
        }
    }
}