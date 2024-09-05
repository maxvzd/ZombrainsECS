using Unity.Mathematics;

public static class MathHelpers
{
    public static float GetHeading(float3 objectPosition, float3 targetPosition)
    {
        float x = objectPosition.x - targetPosition.y;
        float y = objectPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }
}