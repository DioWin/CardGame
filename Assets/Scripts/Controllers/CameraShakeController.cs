using UnityEngine;
using DG.Tweening;

public class CameraShakeController : Singleton<CameraShakeController>
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float defaultDuration = 0.5f;
    [SerializeField] private float defaultStrength = 1f;
    [SerializeField] private int defaultVibrato = 10;
    [SerializeField] private float defaultRandomness = 90f;

    public void ShakeCamera(float duration = -1f, float strength = -1f, int vibrato = -1, float randomness = -1f)
    {
        duration = duration < 0 ? defaultDuration : duration;
        strength = strength < 0 ? defaultStrength : strength;
        vibrato = vibrato < 0 ? defaultVibrato : vibrato;
        randomness = randomness < 0 ? defaultRandomness : randomness;

        cameraTransform.DOComplete();
        cameraTransform.DOShakePosition(duration, strength, vibrato, randomness);
    }
}
