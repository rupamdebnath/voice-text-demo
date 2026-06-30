using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LightAnimator : MonoBehaviour
{
    public List<Light> lights;
    [SerializeField] private float fadeDuration = 5f;
    void OnEnable()
    {
        foreach (var light in lights)
        {
            if (light != null)
            {
                // Set the starting intensity back to 1 instantly
                light.intensity = 1f;

                // 2. Smoothly transition the light intensity to 0 over your fade duration
                light.DOIntensity(0f, fadeDuration).SetEase(Ease.Linear);
            }
        }
    }

    void OnDisable()
    {
        foreach (var light in lights)
        {
            if (light != null)
            {
                // Reset the light intensity back to 1 when the object is disabled
                light.intensity = 1f;
            }
        }
    }
}
