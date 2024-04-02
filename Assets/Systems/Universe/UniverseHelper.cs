using UnityEngine;

public static class UniverseHelper
{
    public const float START_CAMERA = 10f;
    public const float CAMERA_STEP = 10f;
    public const float SCALE_STEP = 1f;
    public const float BLACK_HOLE_SPAWN_RATE = 0.1f;
    public const int MAX_MOONS = 5;
    public const int MAX_PLANETS = 3;
    private const float SCALE_MODIFIER = 0.15f;
    private const float TENTACLE_SPEED_MODIFIER = 0.8f;
    public const float FIBONACCI_SCALE = 0.5f;
    private const int RING_THRESHOLD = 10;

    public static float GetTentacleSpeed(float originalSpeed, float scaleFactor)
    {
        return originalSpeed + TENTACLE_SPEED_MODIFIER * scaleFactor * scaleFactor;
    }

    public static float RandomValue(Vector2 randomVector)
    {
        return Random.Range(randomVector.x, randomVector.y);
    }

    public static int GetFibonacci(int n)
    {
        if (n <= 0)
        {
            Debug.LogError("Invalid input. Position of Fibonacci number must be a positive integer.");
            return -1; // Error condition
        }
        else if (n == 1 || n == 2)
        {
            return 1; // First two Fibonacci numbers are 1
        }
        else
        {
            int a = 1;
            int b = 2;
            int result = 0;

            for (int i = 3; i <= n; i++)
            {
                result = a + b;
                a = b;
                b = result;
            }

            return result;
        }
    }

    public static float GetScale(float scaleFactor)
    {
        if (scaleFactor < 50f)
        {
            return scaleFactor;
        }
        return 50f + Mathf.Log(scaleFactor);
    }

    public static float GetRingOrbitalRadius(int ring)
    {
        if (ring < RING_THRESHOLD)
        {
            return GetFibonacci(ring);
        }
        else
        {
            return GetFibonacci(RING_THRESHOLD - 1) + (ring - RING_THRESHOLD) * 20f;
        }
    }

    public static float GetRingScaleFactor(int ring)
    {
        if (ring < RING_THRESHOLD)
        {
            int fibonacci = GetFibonacci(ring);
            return fibonacci * SCALE_MODIFIER;
        }
        else
        {
            return GetFibonacci(RING_THRESHOLD) * SCALE_MODIFIER;
        }
    }

    public static float GetScaleModifier(float distance)
    {
        return distance * SCALE_MODIFIER;
    }

    public static bool ClockwiseRotation()
    {
        return Random.value > 0.5f;
    }
    
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