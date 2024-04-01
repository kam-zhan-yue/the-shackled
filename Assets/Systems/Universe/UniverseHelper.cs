using UnityEngine;

public static class UniverseHelper
{
    public const float GRAVITATIONAL_CONSTANT = 0.0001f;
    public const float PHYSICS_TIME_STEP = 0.01f;
    public const float CAMERA_STEP = 5f;
    public const float BLACK_HOLE_SPAWN_RATE = 0.1f;
    public const int MAX_MOONS = 5;
    public const int MAX_PLANETS = 7;
    
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