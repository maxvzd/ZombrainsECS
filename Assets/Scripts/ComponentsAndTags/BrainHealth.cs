using Unity.Entities;

namespace ComponentsAndTags
{
    public struct BrainHealth : IComponentData
    {
        public float Max;
        public float Value;
    }
}