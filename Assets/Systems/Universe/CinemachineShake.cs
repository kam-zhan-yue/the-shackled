using System.Collections;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin shakePerlin;

    private void Awake()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        shakePerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity_multiplier, float duration)
    {
        StartCoroutine(ShakeCoroutine(intensity_multiplier, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity_multiplier, float duration)
    {
        float elapsedTime = 0f;
        float shakeDuration = duration;
        float intensity = cinemachineVirtualCamera.m_Lens.OrthographicSize*intensity_multiplier;

        while (elapsedTime < shakeDuration)
        {
            float shakeAmplitude = Mathf.Lerp(intensity, 0f, elapsedTime / shakeDuration);
            shakePerlin.m_AmplitudeGain = shakeAmplitude;

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Reset shake amplitude after the shake duration
        shakePerlin.m_AmplitudeGain = 0f;
    }
}
