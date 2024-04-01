using UnityEngine;

public static class UniverseHelper
{
    public const float GRAVITATIONAL_CONSTANT = 0.0001f;
    public const float PHYSICS_TIME_STEP = 0.01f;
    
    // Function to convert angle (in degrees) to rotation vector (x, y)
    public static Vector2 ConvertAngleToRotation(float angleDegrees)
    {
        // Convert angle from degrees to radians
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Calculate x and y components of the rotation vector
        float x = Mathf.Cos(angleRadians);
        float y = Mathf.Sin(angleRadians);

        return new Vector2(x, y);
    }
    
    public static float GetAngleStep(float deltaTime, float orbitalPeriod)
    {
        float angleStep = 360 / orbitalPeriod;
        return angleStep * deltaTime;
    }

    public static GameObject GetCentre()
    {
        return GameObject.FindGameObjectWithTag($"Centre");
    }
}